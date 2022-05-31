using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("General Settings")] public int tickInSeconds;
        private float _elapsedTime;
        public FlowerBedManager flowerManager;
        public Gamestate gamestate;
        public GameObject gameOverText;
        public Tilemap startTilemap;
        public GameObject tutorialImages;
        public Dictionary<int, Levelconfig> levelConfigs;
        [SerializeField] public Levelconfig currentLevel;
        public TMP_Text newScoreUi;

        [Serializable]
        public struct Levelconfig
        {
            public int LevelId { get; set; }
            public int ScoreNeeded { get; set; }
            public int TickSpeed { get; set; }
        }
        
        private void Start()
        {
            levelConfigs = new Dictionary<int, Levelconfig>();
            
            //Create Level Config
            levelConfigs.Add(1, new Levelconfig()
            {
                LevelId = 1,
                ScoreNeeded = 300,
                TickSpeed = 4
            });
            
            levelConfigs.Add(2, new Levelconfig()
            {
                LevelId = 2,
                ScoreNeeded = 1000,
                TickSpeed = 3
            });
            
            levelConfigs.Add(3, new Levelconfig()
            {
                LevelId = 3,
                ScoreNeeded = 2500,
                TickSpeed = 2
            });
            
            levelConfigs.Add(4, new Levelconfig()
            {
                LevelId = 4,
                ScoreNeeded = 5000,
                TickSpeed = 1
            });

            //Set start level
            currentLevel = levelConfigs[1];
            newScoreUi.text = currentLevel.ScoreNeeded.ToString();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Application.Quit();
            }
            
            if (gamestate == Gamestate.Start)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    GameObject.FindGameObjectWithTag("StartTilemap").SetActive(false);
                    tutorialImages.SetActive(false);
                    gamestate = Gamestate.Play;
                }
            }
            else if (gamestate == Gamestate.Play)
            {
                if (_elapsedTime >= currentLevel.TickSpeed)
                {
                    Tick();
                    _elapsedTime = 0;
                }

                _elapsedTime += Time.deltaTime;
            }
            else if (gamestate == Gamestate.GameOver)
            {
                if (!gameOverText.activeSelf)
                {
                    gameOverText.SetActive(true);
                }

                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }

        private void Tick()
        {
            flowerManager.Grow(Random.Range(0, flowerManager.flowerBeds.Count));
            flowerManager.CheckFlowerHeight();
        }
    }
}