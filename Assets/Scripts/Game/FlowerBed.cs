using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    [Serializable]
    public class FlowerBed
    {
        public int Id { get; set; }
        public Vector3 StartPosition { get; set; }
        public List<TileBase> TileBases { get; set; }
        public bool IsWaterEnabled { get; set; }
        public bool IsHarvestable { get; set; }
        public bool HasFlower { get; set; }
        public Vector3Int TilePosition { get; set; }
    }
}