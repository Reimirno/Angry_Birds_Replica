/*
MapButtonUI.cs

Description: To be filled in.
Author: Yu Long
Created: Friday, December 03 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Reimirno
{
    public class MapButtonUI : MonoBehaviour
    {
        public Button button;
        public GameObject stats;
        public TMP_Text starsObtained;
        public TMP_Text scoresObtained;
        public GameObject locks;
        public TMP_Text starsNeeded;


        public void Initialize(
            bool unlocked, 
            int starNeeded,
            int starObtained,
            int scoreObtained, 
            int worldIndex)
        {
            if (unlocked)
            {
                button.interactable = true;
                stats.SetActive(true);
                locks.SetActive(false);
                starsObtained.text = DataHub.Instance.NumberToFormattedString(starObtained);
                scoresObtained.text = DataHub.Instance.NumberToFormattedString(scoreObtained);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => GameManager.Instance.EnterWorld(worldIndex));
            }
            else
            {
                button.interactable = false;
                stats.SetActive(false);
                locks.SetActive(true);
                starsNeeded.text = DataHub.Instance.NumberToFormattedString(starNeeded);
            }
        }

        public void DisableWithoutData()
        {
            button.interactable = false;
            stats.SetActive(false);
            locks.SetActive(true);
            starsNeeded.text = DataHub.Instance.NumberToFormattedString(999);
        }
    }
}