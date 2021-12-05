/*
Ground.cs

Description: Generic class for unmoving parts of the level.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using UnityEngine;

namespace Reimirno 
{
    public class Ground : MonoBehaviour, IDamageDealer 
    {
        [SerializeField] [Range(5, 50)] private int rawDamage = 20;
        public int GetRawDamage()
        {
            return rawDamage;
        }

    }
}
