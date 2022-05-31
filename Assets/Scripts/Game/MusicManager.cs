using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game
{
    public class MusicManager : MonoBehaviour
    {
        [Header("Manager")]
        public FlowerBedManager FlowerBedManager;
        public GameManager gameManager;
        
        [Header("General Settings")]
        [SerializeField] private AudioSource bgSource;
        [SerializeField] private bool isBgMusicEnabled;
        [SerializeField] private Image soundIcon;

        [Header("Images")]
        public Sprite offSoundIcon;
        public Sprite onSoundIcon;
        
        void Update()
        {
            if (gameManager.gamestate == Gamestate.Start)
            {
                
            } else if (gameManager.gamestate == Gamestate.Play)
            {
                if (Keyboard.current.mKey.wasPressedThisFrame)
                {
                    if (!isBgMusicEnabled)
                    {
                        bgSource.mute = false;
                        soundIcon.sprite = onSoundIcon;
                        isBgMusicEnabled = true;
                    }
                    else
                    {
                        bgSource.mute = true;
                        soundIcon.sprite = offSoundIcon;
                        isBgMusicEnabled = false;
                    }
                }
            } else if (gameManager.gamestate == Gamestate.GameOver)
            {
                
            }
        }
    }
}