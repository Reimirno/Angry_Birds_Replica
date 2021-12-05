/*
WorldData.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using UnityEngine;

namespace Reimirno
{
    [CreateAssetMenu(menuName = "Create Scriptable Objects/WorldData", fileName = "NewWorldData")]
    public class WorldData : ScriptableObject
    {
        public int worldIndex;
        public LevelData[] availableLevels;
        public int unlockCondition;
        public Sprite levelSelectSprite;

        public int nAvailableLevels { get => availableLevels.Length; }
    }
}