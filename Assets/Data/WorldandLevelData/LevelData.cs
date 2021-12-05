/*
LevelData.cs

Description: To be filled in.
Author: Yu Long
Created: Friday, December 03 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/

using UnityEngine;

namespace Reimirno
{
    [CreateAssetMenu(menuName = "Create Scriptable Objects/LevelData", fileName = "NewLevelData")]
    public class LevelData : ScriptableObject
    {
        public int worldIndex;
        public int levelIndex;
        public int TwoStarThreshold;
        public int ThreeStarThreshold;
    }
}