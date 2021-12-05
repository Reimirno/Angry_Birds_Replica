/*
EffectHandler.cs

Description: To be filled in.
Author: Yu Long
Created: Wednesday, December 01 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using UnityEngine;

namespace Reimirno 
{
    public class EffectHandler: GenericSingleton<EffectHandler>
    {
        public GameObject score5000;
        public GameObject[] score10000;
        public GameObject bigSmoke;
        public GameObject woodShatterEffect;
        public GameObject glassShatterEffect;
        public GameObject stoneShatterEffect;


        public void ShowScore(Vector3 pos, int score = 5000)
        {
            switch (score)
            {
                case 2000:
                    break;
                case 3000:
                    break;
                case 5000:
                    Instantiate(score5000, pos, Quaternion.identity);
                    break;
                case 10000:
                    Instantiate(score10000[0], pos, Quaternion.identity);
                    break;
                default:
                    ShowScoreWithDigits(pos, score);
                    break;
            }
        }

        public void ShowRemainingBirdCredit(Vector3 pos, BirdType type)
        {
            Instantiate(score10000[(int)type], pos, Quaternion.identity);
        }

        private void ShowScoreWithDigits(Vector3 pos, int score)
        {
           
        }

        public void ShowDieEffect(Vector3 pos, DestroyableType type)
        {
            switch (type)
            {
                case DestroyableType.Pig_Standard:
                case DestroyableType.Pig_Strong:
                case DestroyableType.Pig_VeryStrong:
                    ShowSmoke(pos);
                    break;
                case DestroyableType.Pig_Ultimate:
                    ShowSmoke(pos, SmokeEffectType.VeryBig);
                    break;
                case DestroyableType.Pig_Weak:
                    ShowSmoke(pos, SmokeEffectType.Small);
                    break;
                case DestroyableType.Wood:
                    Instantiate(woodShatterEffect, pos, Quaternion.identity);
                    break;
                case DestroyableType.Glass:
                    Instantiate(glassShatterEffect, pos, Quaternion.identity);
                    break;
                case DestroyableType.Stone:
                    Instantiate(stoneShatterEffect, pos, Quaternion.identity);
                    break;
            }
        }
        public void ShowDieEffect(Vector3 pos, DieEffectType type)
        {
            switch (type)
            {
                case DieEffectType.BigSmoke:
                    ShowSmoke(pos);
                    break;
                case DieEffectType.SmallSmoke:
                    ShowSmoke(pos, SmokeEffectType.Small);
                    break;
                case DieEffectType.WoodShatter:
                    Instantiate(woodShatterEffect, pos, Quaternion.identity);
                    break;  
            }
        }

        public void ShowSmoke(Vector3 pos, SmokeEffectType type = SmokeEffectType.Big)
        {
            switch (type)
            {
                case SmokeEffectType.Small:
                    var go = Instantiate(bigSmoke, pos, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                    go.transform.localScale *= 0.5f;
                    break;
                case SmokeEffectType.Big:
                    Instantiate(bigSmoke, pos, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                    break;
                case SmokeEffectType.VeryBig:
                    var go2 = Instantiate(bigSmoke, pos, Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)));
                    go2.transform.localScale *= 1.6f;
                    break;
            }
        }
    }
    public enum ScoreEffectType
    {
        Score3000,
        Score5000,
    }
    public enum SmokeEffectType
    {
        Small,
        Big,
        VeryBig
    }
    public enum DieEffectType
    {
        BigSmoke,
        SmallSmoke,
        WoodShatter,
        GlassShatter,
        StoneShatter
    }
}
