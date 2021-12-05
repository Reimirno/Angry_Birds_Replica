/*
PauseButton.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using UnityEngine;

namespace Reimirno 
{
    public class PauseButton: MonoBehaviour
    {
        public void Pause()
        {
            gameObject.SetActive(false);
            UIManager.Instance.ShowPauseMenu();
        }
    }
}
