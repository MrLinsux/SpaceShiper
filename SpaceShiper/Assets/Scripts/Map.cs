using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerSpawner;

    public Tilemap tilemap;

    public Tile[] wayTiles;
    public Tile[] wallTiles;
    public RuleTile[] ruleTiles;

    public GameObject[] singleSideTraps;
    private string[] singleSideTrapsNames;
    public GameObject[] multySideTraps;
    private string[] multySideTrapsNames;

    public int World = 0; public int Level = 0;

    private Vector3 zNomalizer;                 // нормализирует объект по оси z

    public Vector3 BuildLevel(int world, int lvl)
    {
        zNomalizer = Vector3.forward * tilemap.transform.parent.position.z;

        var mapPlan = JsonUtility.FromJson<MapTiles>((Resources.Load("Levels\\" + World.ToString() + "\\" + Level.ToString()) as TextAsset).text);

        for(int i = 0; i < mapPlan.way.Count; i++)
        {
            // ставим дорогу
            var way = mapPlan.way[i];
            tilemap.SetTile(new Vector3Int(way.x, way.y, way.z), wayTiles[way.id]);
            if (way.id == 1)
                playerSpawner = new Vector3(way.x, way.y, player.transform.position.z);
        }
        for (int i = 0; i < mapPlan.wall.Count; i++)
        {
            // строим стены
            var wall = mapPlan.wall[i];
            tilemap.SetTile(new Vector3Int(wall.x, wall.y, wall.z), wallTiles[wall.id]);
        }
        for(int i = 0; i < mapPlan.ruleTiles.Count; i++)
        {
            // для ruleTile, например для рамки у стен
            var rTile = mapPlan.ruleTiles[i];
            tilemap.SetTile(new Vector3Int(rTile.x, rTile.y, rTile.z), ruleTiles[rTile.id]);
        }
        for(int i = 0; i < mapPlan.singleSideTraps.Count; i++)
        {
            // расставляем однонаправленные ловушки
            var trap = mapPlan.singleSideTraps[i];
            Instantiate(
                singleSideTraps[trap.id], 
                new Vector3(trap.x, trap.y, zNomalizer.z), 
                Quaternion.identity, 
                tilemap.transform
                ).GetComponent<SingleSideTrap>().singleDirection = (SingleSideTrap.Direction)trap.d;
        }
        for (int i = 0; i < mapPlan.multySideTraps.Count; i++)
        {
            // расставляем многонаправленные ловушки
            var trap = mapPlan.multySideTraps[i];       
            Instantiate(
                multySideTraps[trap.id],
                new Vector3(trap.x, trap.y, zNomalizer.z),
                Quaternion.identity, 
                tilemap.transform
                ).GetComponent<MultySideTrap>().directions = trap.ds;
        }

        return playerSpawner;
    }

    public void SaveLevel()
    {
        //tilemap.SetTiles(new Vector3Int[] { new Vector3Int(tilemap.cellBounds.xMin, tilemap.cellBounds.yMin, 0), new Vector3Int(tilemap.cellBounds.xMax, tilemap.cellBounds.yMax, 0) }, new TileBase[] { wayTiles[1], wayTiles[1] });
        MapTiles map = new MapTiles();      // весь уровень

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                for (int z = tilemap.cellBounds.zMin; z < tilemap.cellBounds.zMax; z++) 
                {
                    var tile = tilemap.GetTile(new Vector3Int(x, y, z));
                    if (Array.IndexOf(wayTiles, tile as Tile) > -1)
                    {
                        map.way.Add(new MapTiles.Way(x, y, z, Array.IndexOf(wayTiles, tile)));
                    }
                    else if (Array.IndexOf(wallTiles, tile as Tile) > -1)
                    {
                        map.wall.Add(new MapTiles.Wall(x, y, z, Array.IndexOf(wallTiles, tile)));
                    }
                    if(Array.IndexOf(ruleTiles, tile as RuleTile) > -1)
                    {
                        map.ruleTiles.Add(new MapTiles.RuleTiles(x, y, z, Array.IndexOf(ruleTiles, tile as RuleTile)));
                    }
                }
            }
        }

        Transform[] traps = tilemap.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < traps.Length; i++)
        {
            if(traps[i].GetComponent<SingleSideTrap>())
            {
                map.singleSideTraps.Add(
                    new MapTiles.SingleSideTrap(
                        (int)traps[i].position.x,
                        (int)traps[i].position.y,
                        (int)traps[i].position.z,
                        Array.IndexOf(singleSideTrapsNames, traps[i].name),
                        (int)traps[i].GetComponent<SingleSideTrap>().singleDirection));
            }
            else if(traps[i].GetComponent<MultySideTrap>())
            {
                var trapPos = tilemap.WorldToCell(traps[i].position - new Vector3(0, 0, traps[i].position.z));
                var directions = traps[i].GetComponent<MultySideTrap>().directions;

                if (Array.IndexOf(wallTiles, tilemap.GetTile(trapPos + Vector3Int.right)) > -1) 
                    directions[0] = false;
                if (Array.IndexOf(wallTiles, tilemap.GetTile(trapPos + Vector3Int.up) as Tile) > -1) 
                    directions[1] = false;
                if (Array.IndexOf(wallTiles, tilemap.GetTile(trapPos + Vector3Int.left)) > -1)
                    directions[2] = false;
                if (Array.IndexOf(wallTiles, tilemap.GetTile(trapPos + Vector3Int.down)) > -1)
                    directions[3] = false;

                map.multySideTraps.Add(
                    new MapTiles.MultySideTrap(
                        (int)traps[i].position.x,
                        (int)traps[i].position.y,
                        (int)traps[i].position.z,
                        Array.IndexOf(multySideTrapsNames, traps[i].name),
                        directions));
            }
        }

        System.IO.File.WriteAllText(
            Application.dataPath + 
            @"\Resources\Levels\" + 
            World.ToString() + 
            "\\" + 
            Level.ToString() + 
            ".json", 
            JsonUtility.ToJson(map)
            );
        Debug.Log("Level saved");
    }

    void Start()
    {
        singleSideTrapsNames = new string[singleSideTraps.Length];
        multySideTrapsNames = new string[multySideTraps.Length];

        for (int i = 0; i < singleSideTraps.Length; i++)
            singleSideTrapsNames[i] = singleSideTraps[i].name;
        for (int i = 0; i < multySideTraps.Length; i++)
            multySideTrapsNames[i] = multySideTraps[i].name;

        tilemap = this.GetComponent<Tilemap>();

        BuildLevel(World, Level);
    }

    public class MapTiles
    {
        // класс описывает каждый элемент карты на сцене
        // необходимо для сохранения уровня в файл Json

        public List<Way> way = new List<Way>();
        public List<Wall> wall = new List<Wall>();
        public List<RuleTiles> ruleTiles = new List<RuleTiles>();
        public List<SingleSideTrap> singleSideTraps = new List<SingleSideTrap>();
        public List<MultySideTrap> multySideTraps = new List<MultySideTrap>();


        [System.Serializable]
        public class Way
        {
            public int x;
            public int y;
            public int z;
            public int id;
            public Way(int _cellX, int _cellY, int _cellZ, int _tileID) { x = _cellX; y = _cellY; z = _cellZ; id = _tileID; }
        }

        [System.Serializable]
        public class Wall
        {
            public int x;
            public int y;
            public int z;
            public int id;
            public Wall(int _cellX, int _cellY, int _cellZ, int _tileID) { x = _cellX; y = _cellY; z = _cellZ; id = _tileID; }
        }

        [System.Serializable]
        public class RuleTiles
        {
            public int x;
            public int y;
            public int z;
            public int id;
            public RuleTiles(int _cellX, int _cellY, int _cellZ, int _tileID) { x = _cellX; y = _cellY; z = _cellZ; id = _tileID; }
        }

        [System.Serializable]
        public class SingleSideTrap
        {
            public int x;
            public int y;
            public int z;
            public int id;
            public int d;
            public SingleSideTrap(int _cellX, int _cellY, int _cellZ, int _tileID, int _direction) { x = _cellX; y = _cellY; z = _cellZ; id = _tileID; d = _direction; }
        }

        [System.Serializable]
        public class MultySideTrap
        {
            public int x;
            public int y;
            public int z;
            public int id;
            public bool[] ds;
            public MultySideTrap(int _cellX, int _cellY, int _cellZ, int _tileID, bool[] _directions) { x = _cellX; y = _cellY; z = _cellZ; id = _tileID; ds = _directions; }
        }
    }

}
