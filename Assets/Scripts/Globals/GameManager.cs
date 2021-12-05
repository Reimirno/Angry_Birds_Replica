/*
GameManager.cs

Description: The main game loop and game rule checker.
May be refactored into two smaller classes: one for game loop and one for rule checking.
Author: Yu Long
Created: Thursday, December 01 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Reimirno
{
    public enum LevelStatus
    {
        NotStarted,
        Ongoing,
        Paused,
        Finished
    }

    public class GameManager: GenericSingleton<GameManager>
    {
        public LevelStatus curLevelStatus = LevelStatus.NotStarted;
        public int curWorldIndex = 0; //0 means not selected
        public int curLevelIndex = 0; //0 means not selected
        
        private Queue<Bird> aliveBird = new Queue<Bird>();
        private List<Bird> flyingBird = new List<Bird>();
        private List<Destroyable> alivePig = new List<Destroyable>();
        private int curScore = 0;

        public Dictionary<string, AsyncOperationHandle<GameObject>> preloadedWorlds
            = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        public Queue<GameObject> instantiatedWorlds = new Queue<GameObject>();

        Transform worldRoot;

        public Dictionary<string, AsyncOperationHandle<GameObject>> preloadedLevels
            = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        public Queue<GameObject> instantiatedLevels = new Queue<GameObject>();
        public void EnterTitle()
        {
            UIManager.Instance.ToggleMapSelect(false);
            UIManager.Instance.ToggleTitleScreen(true);
            UIManager.Instance.ToggleInLevelGUI(false);
        }

        public void EnterMapSelect()
        {
            this.Log("Enter Map Select");
            //If no worldObject present, Instantiate the world last exited
            //If there is a worldObject (i.e., exit from levelSelect), then do nothing
            if (instantiatedWorlds.Count <= 0) 
            {
                this.Log("No alive world object. Load one..");
                LoadAndInstantiateWorld("World1Parent");
            }
            else
            {
                this.Log("Using existing world object as map select background");
            }
            UIManager.Instance.ToggleTitleScreen(false);
            UIManager.Instance.ToggleMapSelect(true);
        }

        private bool IsWantedWorldObjAlreadyPresent(string key)
        {
            return instantiatedWorlds.Count > 0 &&
                instantiatedWorlds.Peek().name.StartsWith(key);
        }

        public void EnterWorld(int worldNumber)
        {
            this.Log("EnterWorld called with", worldNumber);
            curWorldIndex = worldNumber;
            curLevelStatus = LevelStatus.NotStarted;
            curLevelIndex = 0;
            UIManager.Instance.ToggleTitleScreen(false);
            UIManager.Instance.ToggleMapSelect(false);
            var key = DataHub.Instance.FormatWorldString(worldNumber);
            if (!IsWantedWorldObjAlreadyPresent(key))
            {
                CleanAllWorldObject();
                LoadAndInstantiateWorld(key + "Parent");
            }
            UIManager.Instance.ShowLevelSelectPanel(
                DataHub.Instance.GetWorldData(worldNumber)
                );
            UIManager.Instance.ToggleLoadingScreen(false);
        }

        public void LoadAndInstantiateWorld(string key)
        {
            StartCoroutine(LoadWorld(key));
            StartCoroutine(WaitAndInstantiateWorld(key));
        }

        private IEnumerator WaitAndInstantiateWorld(string key)
        {
            UIManager.Instance.ToggleLoadingScreen(true);
            this.Log("Start waiting for ", key);
            yield return new WaitUntil(() => preloadedWorlds.ContainsKey(key)
            && preloadedWorlds[key].Result != null);
            this.Log("Found loaded world asset!");
            var world = preloadedWorlds[key].Result;
            var worldObj = Instantiate(world,
                Vector3.zero,
                Quaternion.identity
                );
            instantiatedWorlds.Enqueue(worldObj);
            worldRoot = worldObj.transform.GetChild(0);
            this.Log("Instantiated", key);
            UIManager.Instance.ToggleLoadingScreen(false);
            UIManager.Instance.ToggleInLevelGUI(false);
        }

        IEnumerator LoadWorld(string prefabKey)
        {
            this.Log("Try load ", prefabKey);
            if (!preloadedWorlds.ContainsKey(prefabKey))
            {
                this.Log("Loading", prefabKey);

                UIManager.Instance.ToggleLoadingScreen(true);
                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(prefabKey);
                yield return handle;
                if (handle.Result != null)
                {
                    preloadedWorlds[prefabKey] = handle;
                }
                this.Log("loaded", handle.Result.name);
            }
        }

        public void ExitWorld()
        {
            UIManager.Instance.ToggleInLevelGUI(false);
            EnterMapSelect();
            curWorldIndex = 0;
            curLevelStatus = LevelStatus.NotStarted;
            curLevelIndex = 0;
        }

        private IEnumerator LoadLevel(string prefabKey)
        {
            if (!preloadedLevels.ContainsKey(prefabKey))
            {
                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(prefabKey);
                yield return handle;
                if (handle.Result != null)
                {
                    preloadedLevels[prefabKey] = handle;
                }
            }
        }

        public void PrepareNextLevel()
        {
            if (DataHub.Instance.IsLastLevel(curWorldIndex, curLevelIndex))
            {
                return;
            }
            StartCoroutine(LoadLevel(
                DataHub.Instance.FormatLevelString(curWorldIndex, curLevelIndex + 1)));
        }

        public void EnterLevel(int levelNumber)
        {
            this.Log("EnterLevel called with", levelNumber);
            UIManager.Instance.ToggleLoadingScreen(true);
            var key = DataHub.Instance.FormatLevelString(curWorldIndex, levelNumber);
            curLevelIndex = levelNumber;
            StartCoroutine(WaitForEntering(key));
        }

        public void PrepareAndEnterLevel(int levelNumber)
        {
            StartCoroutine(LoadLevel(
                DataHub.Instance.FormatLevelString(curWorldIndex, levelNumber)));
            EnterLevel(levelNumber);
        }

        private IEnumerator WaitForEntering(string key)
        {
            this.Log("Start waiting for ", key);
            yield return new WaitUntil(() => preloadedLevels.ContainsKey(key) && preloadedLevels[key].Result != null);
            this.Log("Found loaded level asset!");
            var level = preloadedLevels[key].Result;
            instantiatedLevels.Enqueue(Instantiate(level, worldRoot));

            SpawnBirdsAndPigs();
            curScore = 0;
            curLevelStatus = LevelStatus.NotStarted;
            UIManager.Instance.ToggleLoadingScreen(false);
            UIManager.Instance.ToggleInLevelGUI(true);
            StartCoroutine(FeedABirdAtStart(1f));
        }

        public void ExitLevel()
        {
            CleanCurrentLevelWithoutOffloading();
            OffloadAllLevels();
            EnterWorld(curWorldIndex);
        }

        private void OffloadCurrentLevel()
        {
            CleanCurrentLevelWithoutOffloading();
            var currentKey = DataHub.Instance.FormatLevelString(curWorldIndex, curLevelIndex);
            Addressables.Release(preloadedLevels[currentKey]);
            preloadedLevels.Remove(currentKey);
        }

        public void OffloadAllLevels()
        {
            foreach (var handle in preloadedLevels.Values)
            {
                Addressables.Release(handle);
            }
            preloadedLevels.Clear();
        }

        public void CleanAllWorldObject()
        {
            while (instantiatedWorlds.Count > 0)
            {
                var level = instantiatedWorlds.Dequeue();
                Addressables.ReleaseInstance(level);
                Destroy(level);
            }
        }

        private void CleanCurrentLevelWithoutOffloading()
        {
            UIManager.Instance.ToggleLoadingScreen(true);
            UIManager.Instance.ToggleInLevelGUI(false);

            while (instantiatedLevels.Count > 0)
            {
                var level = instantiatedLevels.Dequeue();
                Addressables.ReleaseInstance(level);
                Destroy(level);
            }
            UIManager.Instance.ToggleLoadingScreen(false);
        }

        public void NextLevel()
        {
            if(DataHub.Instance.IsLastLevel(curWorldIndex, curLevelIndex))
            {
                ExitLevel();
            }
            else
            {
                OffloadCurrentLevel();
                EnterLevel(curLevelIndex + 1);
            }
        }

        public void ResetCurLevel()
        {
            CleanCurrentLevelWithoutOffloading();
            EnterLevel(curLevelIndex);
        }

        private IEnumerator FeedABirdAtStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            curLevelStatus = LevelStatus.Ongoing;
            TryFeedNextBird();
        }

        private void SaveLevelOutcome()
        {
            if(curLevelStatus == LevelStatus.Finished)
            {
                DataHub.Instance.TryUnlockNextLevel(curWorldIndex, curLevelIndex);
                DataHub.Instance.UpdateLevelHighScore(curScore,
                    curWorldIndex, curLevelIndex);
            }
        }

        private void SpawnBirdsAndPigs()
        {
            alivePig.Clear();
            aliveBird.Clear();
            flyingBird.Clear();
            var allPigs = GameObject.FindGameObjectsWithTag("Pig");
            foreach (var pig in allPigs)
            {
                alivePig.Add(pig.GetComponent<Destroyable>());
            }
            var allBirds = GameObject.FindGameObjectsWithTag("Bird").OrderByDescending(x=>x.transform.position.x);
            foreach (var bird in allBirds)
            {
                aliveBird.Enqueue(bird.GetComponent<Bird>());
            }
        }

        public void CheckLevelFinished()
        {
            if (curLevelStatus == LevelStatus.Ongoing && alivePig.Count <= 0)
            {
                curLevelStatus = LevelStatus.Finished;
                LevelPassed();
            } else if (curLevelStatus == LevelStatus.Ongoing && aliveBird.Count + flyingBird.Count <= 0)
            {
                curLevelStatus = LevelStatus.Finished;
                LevelFailed();
            }
        }

        public bool TryFeedNextBird()
        {
            if (curLevelStatus != LevelStatus.Ongoing)
            {
                this.LogWarning("Level is not ongoing");
                return false;
            }
            if(aliveBird.Count <= 0)
            {
                this.LogWarning("No more birds to feed!");
                return false;
            }
            var bird = aliveBird.Dequeue();
            bird.GetReady();
            flyingBird.Add(bird);
            return true;
        }

        private void LevelFailed()
        {
            this.LogSuccess("You failed");
            UIManager.Instance.ToggleInLevelGUI(false);
            StartCoroutine(PopUpLevelFinishUI(UIManager.Instance.GeneralWaitTime, 0));
        }

        private void LevelPassed()
        {
            this.LogSuccess("You passed");
            UIManager.Instance.ToggleInLevelGUI(false);
            StartCoroutine(CreditRemainingBirds());
            //SaveLevelOutcome();
            //PrepareNextLevel();
            //int nStarsGot = DataHub.Instance.ScoreToStar(curScore, curWorldIndex, curLevelIndex);
            //if(nStarsGot == 0)
            //{
            //    this.LogError("Cannot pass level with 0 star! CurScore = ", curScore);
            //}
            //curScore = 0;
            //StartCoroutine(PopUpLevelFinishUI(UIManager.Instance.GeneralWaitTime, nStarsGot));
        }

        IEnumerator CreditRemainingBirds()
        {
            yield return new WaitForSeconds(UIManager.Instance.GeneralWaitTime);
            foreach (var b in aliveBird.Reverse())
            {
                yield return new WaitForSeconds(UIManager.Instance.GeneralEaseTime);
                EffectHandler.Instance.ShowRemainingBirdCredit(b.transform.position + Vector3.up * 0.5f,
                    b.birdType);
                curScore += 10000;
            }
            if(flyingBird.Count > 0 &&
                (flyingBird[0].curState == BirdState.GettingReady ||
                flyingBird[0].curState == BirdState.Interactable)
                )
            {
                yield return new WaitForSeconds(UIManager.Instance.GeneralEaseTime);
                EffectHandler.Instance.ShowRemainingBirdCredit(flyingBird[0].transform.position + Vector3.up * 0.5f,
                    flyingBird[0].birdType);
                curScore += 10000;
            }

            SaveLevelOutcome();
            PrepareNextLevel();
            int nStarsGot = DataHub.Instance.ScoreToStar(curScore, curWorldIndex, curLevelIndex);
            if (nStarsGot == 0)
            {
                this.LogError("Cannot pass level with 0 star! CurScore = ", curScore);
            }
            curScore = 0;
            StartCoroutine(PopUpLevelFinishUI(UIManager.Instance.GeneralWaitTime, nStarsGot));
        }

        IEnumerator PopUpLevelFinishUI(float waitTime, int finishCode)
        {
            yield return new WaitForSeconds(waitTime);
            UIManager.Instance.ShowLevelFinishedUI(finishCode);
        }

        public void RemovePig(Destroyable target)
        {
            alivePig.Remove(target);
        }

        public void RemoveBird(Bird target)
        {
            flyingBird.Remove(target);
        }

        public void AddScore(int score)
        {
            if(curLevelStatus == LevelStatus.Ongoing)
            {
                this.LogSuccess("score + ", score);
                curScore += score;
            }
        }
    }
}
