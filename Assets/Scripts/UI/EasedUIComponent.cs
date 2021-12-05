/*
EasedUIComponent.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Reimirno 
{
    public class EasedUIComponent : MonoBehaviour
    {
        public List<EasedUIComponent> friends;
        protected CanvasGroup group;

        protected virtual void OnEnable()
        {
            group = GetComponent<CanvasGroup>();
            group.alpha = 0;
        }

        public virtual void Show(int intArgs = -1, params EasedUIComponent[] myfriends)
        {
            gameObject.SetActive(true);
            Initialize(intArgs);
            DOTween.To(() => group.alpha,
                value => group.alpha = value,
                1,
                UIManager.Instance.GeneralEaseTime);
            foreach (var friend in myfriends)
            {
                friends.Add(friend);
                friend.Show(intArgs);
            }
        }

        protected virtual void Initialize(int intArgs)
        {

        }

        public virtual void HideAndReset()
        {
            group.alpha = 0;
            gameObject.SetActive(false);
            foreach(var friend in friends)
            {
                friend.HideAndReset();
            }
            friends.Clear();
        }
    }
}
