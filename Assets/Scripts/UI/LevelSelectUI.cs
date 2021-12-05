/*
LevelSelectUI.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using UnityEngine;
using UnityEngine.UI;

namespace Reimirno 
{
    public class LevelSelectUI: MonoBehaviour
    {
        public Button backButton;
        public GameObject levelButton;
        public Sprite[] starsSprites;

		public void InitializeButtons(WorldData data)
        {
            CleanAllChilren();
            backButton.onClick.AddListener(GameManager.Instance.ExitWorld);
            backButton.onClick.AddListener(() => gameObject.SetActive(false));
            backButton.onClick.AddListener(backButton.onClick.RemoveAllListeners);
            for (int i = 1; i <= data.nAvailableLevels; i++)
            {
                var go = Instantiate(levelButton, transform);
                go.GetComponent<LevelButton>().Initialize(
                    DataHub.Instance.IsLevelUnlocked(data.worldIndex, i),
                    data.levelSelectSprite,
                    starsSprites[DataHub.Instance.
                    HowManyStarsObtained(data.worldIndex, i)],
                    i);
            }
        }

        /// <summary>
        /// May be improved by using pooling.
        /// </summary>
        private void CleanAllChilren()
        {
            //backButton.onClick.RemoveAllListeners();
            foreach (Transform child in transform)
            {
                if (child.name != "Back")
                    Destroy(child.gameObject);
            }
        }
    }
}
