using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerSpawner;

    public Tilemap tilemap;

    public Tile[] wayTiles;

    public Tile[] wallTiles;

    public PrefabBrush[] staticTraps;

    private Vector3 zNomalizer;                 // нормализирует объект по оси z

    private void BuildLevel(int world, int lvl)
    {
        zNomalizer = Vector3.forward * tilemap.transform.parent.position.z;


    }

    private void SaveLevel()
    {
        TileBase[] allTiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        MapTiles map = new MapTiles();      // весь уровень


        for (int i = 0; i < tilemap.cellBounds.x; i++)
        {
            for (int j = 0; j < tilemap.cellBounds.y; j++)
            {

            }
        }

        Debug.Log(JsonUtility.ToJson(map, true));
    }
    void Start()
    {
        tilemap = this.GetComponent<Tilemap>();
        Debug.Log(tilemap.CellToWorld(new Vector3Int(tilemap.cellBounds.xMax, tilemap.cellBounds.yMax, 0)));
        SaveLevel();
    }

    public class MapTiles
    {
        // класс описывает каждый элемент карты на сцене
        // необходимо для сохранения уровня в файл Json
        public class Way
        {
            public int cellX { get; set; }
            public int cellY { get; set; }
            public int tileID { get; set; }
        }
        public class Wall
        {
            public int cellX { get; set; }
            public int cellY { get; set; }
            public int tileID { get; set; }
        }
        public class SingleSideTrap
        {
            public int cellX { get; set; }
            public int cellY { get; set; }
            public int tileID { get; set; }
            public int Direction { get; set; }
        }
        public class MultySideTrap
        {
            public int cellX { get; set; }
            public int cellY { get; set; }
            public int tileID { get; set; }
            public bool[] Directions { get; set; }
        }
    }

}
