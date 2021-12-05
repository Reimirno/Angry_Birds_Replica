/*
LevelButton.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Reimirno 
{
    public class LevelButton: MonoBehaviour
    {
        private Button button;
        public Image stars;
        public Image self;
        public TMP_Text levelNumText;

        private void OnEnable()
        {
            button = GetComponent<Button>();
            //stars = GetComponentInChildren<Image>();
            //self = GetComponent<Image>();
            //levelNumText = GetComponentInChildren<TMP_Text>();
        }

        public void Initialize(
            bool unlocked, 
            Sprite activeStateSprite, 
            Sprite starSprite, 
            int levelIndex)
        {
            if (!unlocked)
            {
                stars.gameObject.SetActive(false);
                levelNumText.gameObject.SetActive(false);
                button.interactable = false;
                return;
            }
            button.interactable = true;
            stars.gameObject.SetActive(true);
            levelNumText.gameObject.SetActive(true);
            button.onClick.AddListener(() => GameManager.Instance.PrepareAndEnterLevel(levelIndex));
            button.onClick.AddListener(() => transform.parent.gameObject.SetActive(false));
            button.onClick.AddListener(button.onClick.RemoveAllListeners);
            SwapSprite(activeStateSprite);
            SetUpStars(starSprite);
            SetUpLevelIndex(levelIndex);
        }

        private void SetUpLevelIndex(int levelIndex)
        {
            levelNumText.text = DataHub.Instance.NumberToFormattedString(levelIndex);
        }

        private void SetUpStars(Sprite starSprite)
        {
            stars.sprite = starSprite;
        }

        private void SwapSprite(Sprite sprite)
        {
            SpriteState spriteState = new SpriteState();
            spriteState = button.spriteState;
            spriteState.pressedSprite = sprite;
            spriteState.selectedSprite = sprite;
            spriteState.highlightedSprite = sprite;
            button.spriteState = spriteState;
            self.sprite = sprite;
        }

    }
}
