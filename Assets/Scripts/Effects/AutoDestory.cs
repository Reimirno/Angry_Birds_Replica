/*
AutoDestory.cs

Description: Meant to supply an animation event for the object to be destoryed.
Will become obsolete after a pooling system is implemented.
Author: Yu Long
Created: Wednesday, December 01 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using UnityEngine;

namespace Reimirno 
{
    public class AutoDestory: MonoBehaviour
    {
		public void Kill()
        {
            Destroy(gameObject);
        }
    }
}
