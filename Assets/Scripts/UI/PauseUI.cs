/*
PauseUI.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reimirno
{
    public class PauseUI : EasedUIComponent
    {
        public GameObject pauseButton;
        public Button[] buttons;

        protected override void OnEnable()
        {
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
        }

        public override void Show(int intArgs, params EasedUIComponent[] myfriends)
        {
            gameObject.SetActive(true);

            Time.timeScale = 0;
            GameManager.Instance.curLevelStatus = LevelStatus.Paused;
        }

        public override void HideAndReset()
        {
            Time.timeScale = 1;
            GameManager.Instance.curLevelStatus = LevelStatus.Ongoing;

            pauseButton.SetActive(true);
            gameObject.SetActive(false);

        }


    }
}