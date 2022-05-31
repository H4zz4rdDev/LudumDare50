using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game
{
    public class FlowerBedManager : MonoBehaviour
    {
        [Header("Tilemap")] [SerializeField]
        private Tilemap tilemap;

        [Header("Manager Settings")]
        public GameManager gameManager;
        
        [Header("General Settings")] public PlayerController playerController;
        public Vector3 firstBedPosition;
        public int flowerbedCount;
        public int flowerClearCount;
        public int flowerMaxHeight;

        [Header("Tiles")] public TileBase flowerGround;
        public TileBase flowerHead;
        public TileBase weedGround;
        public TileBase weedWarningGround;
        public List<FlowerBed> flowerBeds;

        void Start()
        {
            flowerBeds = new List<FlowerBed>();

            for (int i = 0; i < flowerbedCount; i++)
            {
                flowerBeds.Add(new FlowerBed()
                {
                    Id = i,
                    StartPosition = new Vector3(firstBedPosition.x + i, firstBedPosition.y, firstBedPosition.z),
                    TileBases = new List<TileBase>()
                });
            }

            print("Beete: " + flowerBeds.Count);
        }

        public void CheckFlowerHeight()
        {
            for (int i = 0; i < flowerBeds.Count; i++)
            {
                if (flowerBeds[i].TileBases.Count > flowerMaxHeight)
                {
                    if (flowerBeds[i].TileBases.Count > flowerMaxHeight + 1)
                    {
                        gameManager.gamestate = Gamestate.GameOver;
                    }
                    for (int j = 0; j < flowerMaxHeight + 1; j++)
                    {
                        tilemap.SetTile(
                            new Vector3Int(Mathf.RoundToInt(flowerBeds[i].StartPosition.x),
                                Mathf.RoundToInt(flowerBeds[i].StartPosition.y + j), 0), weedWarningGround);
                    }
                }
            }
        }

        public bool IsWaterRunning()
        {
            if (flowerBeds.Count <= 0)
            {
                return false;
            }

            foreach (FlowerBed flowerBed in flowerBeds)
            {
                if (flowerBed.IsWaterEnabled)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasWaterOnFlowerBed(int flowerBedXPosition)
        {
            var flowerBedPosition = flowerBedXPosition - firstBedPosition.x;
            var flowerBed = flowerBeds[(int) flowerBedPosition];

            return flowerBed.IsWaterEnabled;
        }

        public bool CanHarvest(int flowerBedXPosition)
        {
            var flowerBedPosition = flowerBedXPosition - firstBedPosition.x;
            var flowerBed = flowerBeds[(int) flowerBedPosition];

            //print("H: " + flowerBed.IsHarvestable + " W: " + flowerBed.IsWaterEnabled + " C: " +
            //      flowerBed.TileBases.Count);

            return flowerBed.IsHarvestable && !flowerBed.IsWaterEnabled && flowerBed.TileBases.Count > 0;
        }

        public void Harvest(int flowerBedXPosition)
        {
            var flowerBedPosition = flowerBedXPosition - firstBedPosition.x;
            var flowerBed = flowerBeds[(int) flowerBedPosition];
            
            if (flowerBed.HasFlower)
            {
                int currentScore = Int32.Parse(playerController.flowerUiScore.text);
                int newScore = currentScore + flowerBed.TileBases.Count;

                if (newScore < 10)
                {
                    playerController.flowerUiScore.text = "00" + newScore;
                } else if ( newScore < 100)
                {
                    playerController.flowerUiScore.text = "0" + newScore;
                }
                else
                {
                    playerController.flowerUiScore.text = newScore.ToString();
                }
            }
            else
            {
                int currentScore = Int32.Parse(playerController.weedUiScore.text);
                int newScore = currentScore + flowerBed.TileBases.Count;

                if (newScore < 10)
                {
                    playerController.weedUiScore.text = "00" + newScore;
                }
                else if ( newScore < 100)
                {
                    playerController.weedUiScore.text = "0" + newScore;
                }
                else
                {
                    playerController.weedUiScore.text = newScore.ToString();
                }
            }

            for (int i = 0; i < flowerClearCount; i++)
            {
                tilemap.SetTile(
                    new Vector3Int(Mathf.RoundToInt(flowerBed.StartPosition.x),
                        Mathf.RoundToInt(flowerBed.StartPosition.y + i), 0), null);
            }
            
            playerController.PlayChop();

            flowerBed.IsHarvestable = false;
            flowerBed.HasFlower = false;
            flowerBed.IsWaterEnabled = false;
            flowerBed.TileBases.Clear();
        }

        public void Grow(int flowerBedNumber, bool weed = true)
        {
            var flowerBed = flowerBeds[flowerBedNumber];

            if (flowerBed.HasFlower)
            {
                return;
            }

            var flowerCount = flowerBed.TileBases.Count;
            var tilePosition = new Vector3Int(Mathf.RoundToInt(firstBedPosition.x) + flowerBed.Id,
                Mathf.RoundToInt(firstBedPosition.y + flowerCount), 0);
            var flowerBedStarPosition = new Vector3Int(Mathf.RoundToInt(firstBedPosition.x) + flowerBed.Id,
                Mathf.RoundToInt(firstBedPosition.y), 0);

            if (flowerBed.TileBases.Count == 0 && flowerBed.IsWaterEnabled)
            {
                tilemap.SetTile(tilePosition, flowerHead);
                flowerBed.TileBases.Add(flowerHead);
                flowerBed.HasFlower = true;
                flowerBed.IsHarvestable = true;
                flowerBed.TilePosition = tilePosition;
                flowerBed.StartPosition = flowerBedStarPosition;
                playerController.PlayPlopp();
            }
            else
            {
                tilemap.SetTile(tilePosition, weedGround);
                flowerBed.TileBases.Add(weedGround);
                playerController.PlayPlopp();
                flowerBed.IsHarvestable = true;
            }

            Tile tile = tilemap.GetTile(tilePosition) as Tile;
            if (tile != null)
            {
                tile.color = Color.white;
            }
        }

        public void SetWaterForSpill(int spillXPosition, bool hasWater = false)
        {
            var flowerBedPosition = spillXPosition - firstBedPosition.x;
            var flowerBed = flowerBeds[(int) flowerBedPosition];
            flowerBed.IsWaterEnabled = hasWater;
        }
    }
}