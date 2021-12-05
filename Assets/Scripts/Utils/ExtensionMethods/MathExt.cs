/***
MathExt.cs

Description: Some utility methods for 2D game mathematics
Author: Yu Long
Created: Friday, December 03 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
***/
using UnityEngine;

namespace Reimirno 
{

    public static class MathExt
    {
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}
