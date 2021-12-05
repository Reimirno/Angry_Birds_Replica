/***
GenericSingleton.cs

Description: Inherit from this class to make a class singleton and not destroyed across scenes.
Author: Yu Long
Created: Monday, November 22 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
***/

using UnityEngine;

namespace Reimirno
{
    public class GenericSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}