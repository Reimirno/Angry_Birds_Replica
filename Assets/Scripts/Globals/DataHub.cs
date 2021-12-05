/*
DataHub.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reimirno
{
    public class DataHub : GenericSingleton<DataHub>
    {
        public WorldData[] worldData;
        public BirdData[] birdData;
        
        public WorldData GetWorldData(int worldIndex)
        {
            return worldData[worldIndex - 1];
        }
        public int HowManyWorldsAvailable()
        {
            return worldData.Length;
        }

        public BirdData GetBirdData(BirdType type)
        {
            return birdData[(int)type];
        }

        public LevelData GetLevelData(int worldIndex, int levelIndex)
        {
            return worldData[worldIndex - 1].availableLevels[levelIndex - 1];
        }

        public bool IsLastLevel(int worldIndex, int levelIndex)
        {
            return GetWorldData(worldIndex).nAvailableLevels <= levelIndex;
        }

        public bool IsLevelUnlocked(int worldIndex, int levelIndex)
        {
            if(levelIndex == 1)
            {
                UnlockLevel(worldIndex, levelIndex);
            }
            return HowManyScoresObtained(worldIndex, levelIndex) > -1;
        }
        // -1 means locked
        // above 0 represents scores obtained for this level
        // if this level is unlocked but never beaten, it is 0

        public int HowManyScoresObtained(int worldIndex, int levelIndex)
        {
            return LoadSaveData(FormatLevelString(worldIndex, levelIndex));
        }

        private int LoadSaveData(string key)
        {
            this.Log("Query:", key, "?Result:", PlayerPrefs.GetInt(key, -1));
            return PlayerPrefs.GetInt(key, -1);
        }
        private void SetSaveData(string key, int value)
        {
            this.Log("Set:", key, "=>Value:", value);
            PlayerPrefs.SetInt(key, value);
        }
        public void UpdateLevelHighScore(int score, int worldIndex, int levelIndex)
        {
            var key = FormatLevelString(worldIndex, levelIndex);
            if(score > LoadSaveData(key))
            {
                SetSaveData(key, score);
                //this.Log(key, "update =>" , LoadSaveData(key));
            }
        }
        public int HowManyScoresObtained(int worldIndex)
        {
            int sum = 0;
            for (int i = 1; i <= worldData[worldIndex - 1].nAvailableLevels; i++)
            {
                var levelScore = HowManyScoresObtained(worldIndex, i);
                if (levelScore > 0)
                    sum += levelScore;
            }
            return sum;
        }

        public int ScoreToStar(int score, int worldIndex, int levelIndex)
        {
            var levelData = GetLevelData(worldIndex, levelIndex);
            if (score >= levelData.ThreeStarThreshold) return 3;
            if (score >= levelData.TwoStarThreshold) return 2;
            if (score > 0) return 1;
            return 0;
        }

        public int HowManyStarsObtained(int worldIndex, int levelIndex)
        {
            return ScoreToStar(
                HowManyScoresObtained(worldIndex, levelIndex), 
                worldIndex, levelIndex);
        }

        public int HowManyStarsObtained(int worldIndex)
        {
            int sum = 0;
            for (int i = 1; i <= worldData[worldIndex - 1].nAvailableLevels; i++)
            {
                sum += HowManyStarsObtained(worldIndex, i);
            }
            return sum;
        }

        public int HowManyStarsNeededToUnlock(int worldIndex)
        {
            return GetWorldData(worldIndex).unlockCondition;
        }

        public int HowManyStarsObtainedInTotal()
        {
            int sum = 0;
            for(int i = 0; i < worldData.Length; i++)
            {
                sum += HowManyStarsObtained(i + 1);
            }
            return sum;
        }

        public bool IsWorldUnlocked(int worldIndex)
        {
            return HowManyStarsObtainedInTotal() >= 
                HowManyStarsNeededToUnlock(worldIndex);
        }

        public void UnlockLevel(int worldIndex, int levelIndex)
        {
            this.Log(FormatLevelString(worldIndex, levelIndex), "Unlocked!");
            UpdateLevelHighScore(0, worldIndex, levelIndex);
        }

        public void TryUnlockNextLevel(int worldIndex, int levelIndex)
        {
            if (IsLastLevel(worldIndex, levelIndex)) return;
            UnlockLevel(worldIndex, levelIndex + 1);
        }

        public string FormatLevelString(int worldIndex, int levelIndex)
        {
            return "World" + worldIndex + "Level" + levelIndex;
        }

        public string FormatWorldString(int worldIndex)
        {
            return "World" + worldIndex;
        }

        public string NumberToFormattedString(int num)
        {
            string formatted = "";
            if (num < 0)
            {
                this.LogError("Parse is asked to parse a negative num!");
            }
            if (num == 0) return "<sprite index=0>";
            while (num > 0)
            {
                formatted = string.Format("<sprite index={0}>", num % 10)
                    + formatted;
                num /= 10;
            }
            return formatted;
        }

        public List<int> GetThresholdsByMaterial(DestroyableType type)
        {
            switch (type)
            {
                case DestroyableType.Glass:
                    return new List<int> { 20, 50, 100 };
                case DestroyableType.Pig_Weak:
                    return new List<int> { 70, 100, 120 };
                case DestroyableType.Pig_Standard:
                    return new List<int> { 100, 150, 200 };
                case DestroyableType.Wood:
                    return new List<int> { 150, 200, 350 };
                case DestroyableType.Pig_Strong:
                    return new List<int> { 170, 250, 450 };
                case DestroyableType.Pig_VeryStrong:
                    return new List<int> { 200, 330, 550 };
                case DestroyableType.Stone:
                    return new List<int> { 600, 800, 1050 };
                case DestroyableType.Pig_Ultimate:
                    return new List<int> { 300, 450, 650 };
            }
            this.LogError("You shouldn't reach here!");

            return null;
        }

        public int GetScoreByheavinessAndType(DestroyableType type, Mass heaviness)
        {
            if ((int)type < 100) return 5000;
            switch (heaviness)
            {
                case Mass.VeryLight:
                    return 500;
                case Mass.Light:
                    return 1000;
                case Mass.Medium:
                    return 1500;
                case Mass.Heavy:
                    return 2500;
                case Mass.VeryHeavy:
                    return 3000;
            }
            this.LogError("You shouldn't reach here!");
            return 0;
        }

        public int GetMassByheaviness(Mass heaviness)
        {
            switch (heaviness)
            {
                case Mass.VeryLight:
                    return 1;
                case Mass.Light:
                    return 2;
                case Mass.Medium:
                    return 5;
                case Mass.Heavy:
                    return 8;
                case Mass.VeryHeavy:
                    return 12;
            }
            this.LogError("You shouldn't reach here!");
            return 0;
        }
        
        public int GetDamageByheaviness(Mass heaviness)
        {
            switch (heaviness)
            {
                case Mass.VeryLight:
                    return 5;
                case Mass.Light:
                    return 10;
                case Mass.Medium:
                    return 15;
                case Mass.Heavy:
                    return 20;
                case Mass.VeryHeavy:
                    return 30;
            }
            this.LogError("You shouldn't reach here!");
            return 0;
        }

        

    } 
}