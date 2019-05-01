﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioSource sfxSource;                        // use to play sound effect
    public AudioSource bgmSource;                        // use to play bg music
    public AudioClip[] sfxClips; 
    public AudioClip[] bgmClips;
    /*
    ------------------ use this space to tell which index is what sound ------------------ 
        SFX
        0 - walking
        1 - turning
        2 - footsteps
        3 - button click
        4 - pick up flashlight
        5 - place flashlight
        6 - turn off flashlight
        7 - turn on flashlight
        8 - simple ghost move
        9 - simple ghost laugh
        10 - highlight tile 
        11 - clear level
        12 - enter level
        13 - key_pickup
        14 - menu highlight
        15 - tile highlight
         
        BGM
        0 - game title
        1 - ghost stage 1
        2 - ambient
        3 - ghost stage 2
        4 - ghost stage 3
         
    */
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // play once
    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(sfxClips[index]);
    }

    // play loops
    public void PlayBGM(int index)
    {
        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}

