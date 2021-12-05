/*
Star.cs

Description: To be filled in.
Author: Yu Long
Created: Thursday, December 02 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public Sprite[] sprites;
    private Image image;
    private ParticleSystem[] particles;
    private void OnEnable()
    {
        image = GetComponent<Image>();
        image.sprite = sprites[0];
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void ShowStar()
    {
        PlayParticleEffect();
        image.sprite = sprites[1];
    }

    private void PlayParticleEffect()
    {
        foreach(var particle in particles)
        {
            particle.Play();
        }
    }

    public void ResetStar()
    {
        image.sprite = sprites[0];
    }
}
