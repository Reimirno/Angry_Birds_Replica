/*
LevelFinishedUI.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reimirno
{
    public class LevelFinishedUI : EasedUIComponent
    {
        public Star[] Win_stars;
        public GameObject Win_birdSmile;
        public GameObject Lose_pigSmile;
        public Button[] buttons;

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (var b in buttons)
            {
                b.onClick.RemoveAllListeners();
            }
            buttons[0].onClick.AddListener(HideAndReset);
            buttons[1].onClick.AddListener(HideAndReset);
            buttons[1].onClick.AddListener(() => UIManager.Instance.ToggleInLevelGUI(false));
            buttons[2].onClick.AddListener(HideAndReset);
            buttons[0].onClick.AddListener(GameManager.Instance.ResetCurLevel);
            buttons[1].onClick.AddListener(GameManager.Instance.ExitLevel);
            buttons[2].onClick.AddListener(GameManager.Instance.NextLevel);
        }

        protected override void Initialize(int intArgs)
        {
            if (intArgs == 0)
            {
                Win_birdSmile.SetActive(false);
                Lose_pigSmile.SetActive(true);
                ToggleStarGameObject(false);
            }
            else
            {
                Win_birdSmile.SetActive(true);
                Lose_pigSmile.SetActive(false);
                ToggleStarGameObject(true);
                StartCoroutine(PopStars(intArgs));
            }
            if(intArgs == 0 || DataHub.Instance.IsLastLevel(
                GameManager.Instance.curWorldIndex,
                GameManager.Instance.curLevelIndex
                ))
            {
                buttons[2].gameObject.SetActive(false);
            }
            else
            {
                buttons[2].gameObject.SetActive(true);
            }
        }

        float intervalBetweenStars = 0.5f;
        private IEnumerator PopStars(int finishCode)
        {
            yield return new WaitForSeconds(intervalBetweenStars);
            for (int i = 0; i < finishCode; i++)
            {
                yield return new WaitForSeconds(intervalBetweenStars);
                Win_stars[i].ShowStar();
            }
        }

        public override void HideAndReset()
        {
            foreach (var s in Win_stars)
            {
                s.ResetStar();
            }
            //foreach (var b in buttons)
            //{
            //    b.onClick.RemoveAllListeners();
            //}
            base.HideAndReset();
        }

        private void ToggleStarGameObject(bool toggle)
        {
            foreach(var s in Win_stars)
            {
                s.gameObject.SetActive(toggle);
            }
        }
    }
}
