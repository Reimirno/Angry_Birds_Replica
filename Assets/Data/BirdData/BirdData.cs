/*
BirdData.cs

Description: To be filled in.
Author: Yu Long
Created: Friday, December 03 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/

using UnityEngine;

namespace Reimirno
{
    [CreateAssetMenu(menuName = "Create Scriptable Objects/BirdData", fileName = "NewBirdData")]
    public class BirdData : ScriptableObject
    {
        public int rawDamage = 10;
        public float lifeTimeAfterHit = 5;
        public bool hasSkill = false;
        public AnimatorOverrideController controller;
        public Mass heaviness;
    }
}