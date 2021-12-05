/*
UIManager.cs

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
    public class UIManager : GenericSingleton<UIManager>
    {
        
        public EasedUIComponent levelFinishedUI;
        public EasedUIComponent blindUI;
        public EasedUIComponent pauseMenu;
        public GameObject pauseButton;
        public GameObject loadingScreen;
        public GameObject levelSelectPanel;
        public GameObject titleScreen;
        public GameObject mapSelectPanel;
        public float GeneralEaseTime = 1f;
        public float GeneralWaitTime = 2f;

        public void ToggleTitleScreen(bool toggle)
        {
            titleScreen.SetActive(toggle);
        }

        public void ToggleMapSelect(bool toggle)
        {
            mapSelectPanel.SetActive(toggle);
            if (toggle)
                mapSelectPanel.GetComponent<MapSelectUI>().InitializeMaps();
        }

        public void ShowLevelFinishedUI(int finishCode)
        {
            levelFinishedUI.Show(finishCode, blindUI);
        }

        public void ShowPauseMenu()
        {
            pauseMenu.Show();
        }

        public void ToggleInLevelGUI(bool toggle)
        {
            pauseButton.SetActive(toggle);
        }

        public void ToggleLoadingScreen(bool toggle)
        {
            
            if(toggle == false)
            {
                StartCoroutine(DelayTurnOffLoadingScreen(GeneralEaseTime / 3));
            }
            else
            {
                loadingScreen.SetActive(toggle);
            }
        }
        IEnumerator DelayTurnOffLoadingScreen(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            loadingScreen.SetActive(false);
        }

        public void ShowLevelSelectPanel(WorldData data)
        {
            levelSelectPanel.SetActive(true);
            levelSelectPanel.GetComponent<LevelSelectUI>().InitializeButtons(data);
        }
    }
}