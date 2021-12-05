/*
MapSelectUI.cs

Description: To be filled in.
Author: Yu Long
Created: Friday, December 03 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/

using UnityEngine;
using UnityEngine.UI;

namespace Reimirno 
{
    public class MapSelectUI: MonoBehaviour
    {
        public Button backButtons;
        public MapButtonUI[] mapButtons;

        public void InitializeMaps()
        {
            backButtons.onClick.RemoveAllListeners();
            backButtons.onClick.AddListener(GameManager.Instance.EnterTitle);
            int worldIndex = 0;
            foreach (var b in mapButtons)
            {
                worldIndex++;
                if (worldIndex > DataHub.Instance.HowManyWorldsAvailable())
                    b.DisableWithoutData();
                else
                    b.Initialize(
                        DataHub.Instance.IsWorldUnlocked(worldIndex),
                        DataHub.Instance.HowManyStarsNeededToUnlock(worldIndex),
                        DataHub.Instance.HowManyStarsObtained(worldIndex),
                        DataHub.Instance.HowManyScoresObtained(worldIndex),
                        worldIndex
                        );
            }
        }
    }
}
