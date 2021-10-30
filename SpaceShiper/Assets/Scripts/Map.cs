using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public GameObject player;           // объект игрока
    public Vector3 playerSpawner;       // и точка его появления

    public Tilemap tilemap;             // компоненит тайлмапа

    // набор объектов, из которых составляется уровень
    public Tile[] wayTiles;             // тайлы дороги
    public Tile[] wallTiles;            // тайлы стены
    public RuleTile[] ruleTiles;        // тайлы с правилом размещения

    public GameObject[] singleSideTraps;        // ловушки одиночной направленности
    private string[] singleSideTrapsNames;      // и их имена
    public GameObject[] multySideTraps;         // ловушки множественной направленности
    private string[] multySideTrapsNames;       // и их имена

    // префабы
    public GameObject teleport;                 // телепорт
    public GameObject pusher;                   // толкатель
    public GameObject door;                     // обычная дверь
    public GameObject timeDoor;                 // дверь с таймером

    public Vector3 BuildLevel(int world, int level)
    {
        // загружаем схему карты из Ресурсов
        var mapPlan = 
            JsonUtility.FromJson<MapTiles>(
                (Resources.Load("Levels\\" + world.ToString() + "\\" + level.ToString()) as TextAsset).text
                );

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
                trap.pos, 
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
                trap.pos,
                Quaternion.identity, 
                tilemap.transform
                ).GetComponent<MultySideTrap>().directions = trap.ds;
        }
        for(int i = 0; i < mapPlan.portals.Count; i++)
        {
            // ставим телепорты
            var portal = mapPlan.portals[i];
            Instantiate(
                teleport,
                portal.portals[0].pos, 
                Quaternion.identity, 
                tilemap.transform
                ).transform.GetChild(1).position = portal.portals[1].pos;
        }
        for(int i = 0; i < mapPlan.pushers.Count; i++)
        {
            // размещаем толкатели
            var pusher = mapPlan.pushers[i];
            Instantiate(this.pusher, pusher.pos, Quaternion.identity, tilemap.transform);
        }
        for(int i = 0; i < mapPlan.doors.Count; i++)
        {
            var door = mapPlan.doors[i];
            var temp = Instantiate(
                this.door, 
                door.pos, 
                Quaternion.identity, 
                this.transform
                );
            temp.GetComponent<Door>().switcher.position = door.switcher.pos;
        }
        for (int i = 0; i < mapPlan.timeDoors.Count; i++)
        {
            var door = mapPlan.timeDoors[i];
            var temp = Instantiate(
                this.timeDoor,
                door.pos,
                Quaternion.identity,
                this.transform
                );
            temp.GetComponent<TimeDoor>().timeToClose = door.time;
            temp.GetComponent<TimeDoor>().switcher.position = door.switcher.pos;
        }

        return playerSpawner;
    }

    public void SaveLevel(int world, int level)
    {
        MapTiles map = new MapTiles();      // весь уровень

        // сначала сохраняем тайлы
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
        
        Transform[] child = tilemap.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < child.Length; i++)
        {
            if(child[i].GetComponent<SingleSideTrap>())
            {
                map.singleSideTraps.Add(
                    new MapTiles.SingleSideTrap(
                        child[i].GetComponent<SingleSideTrap>().sourcePos,
                        Array.IndexOf(singleSideTrapsNames, child[i].name),
                        (int)child[i].GetComponent<SingleSideTrap>().singleDirection));
            }
            else if(child[i].GetComponent<MultySideTrap>())
            {
                var trapPos = tilemap.WorldToCell(child[i].position - new Vector3(0, 0, child[i].position.z));
                var directions = child[i].GetComponent<MultySideTrap>().directions;

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
                        child[i].position,
                        Array.IndexOf(multySideTrapsNames, child[i].name),
                        directions));
            }
            else if(child[i].CompareTag("Teleport"))
            {
                map.portals.Add(new MapTiles.Teleporter(new MapTiles.Portal[2] {
                    new MapTiles.Portal(child[i].position),
                    new MapTiles.Portal(child[i].GetChild(1).position)
                }));
            }
            else if(child[i].GetComponent<Pusher>())
            {
                map.pushers.Add(new MapTiles.Pusher(child[i].position));
            }
            else if(child[i].GetComponent<Door>())
            {
                map.doors.Add(new MapTiles.Door(
                    child[i].position, 
                    new MapTiles.Switcher(child[i].GetChild(0).position)
                    ));
            }
            else if(child[i].GetComponent<TimeDoor>())
            {
                map.timeDoors.Add(new MapTiles.TimeDoor(
                    child[i].position, 
                    new MapTiles.Switcher(child[i].GetChild(0).position), 
                    child[i].GetComponent<TimeDoor>().timeToClose
                    ));
            }
        }

        System.IO.File.WriteAllText(
            Application.dataPath + @"\Resources\Levels\" + world.ToString() + "\\" + level.ToString() + ".json", 
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
    }

    public class MapTiles
    {
        // класс описывает каждый элемент карты на сцене
        // необходимо для сохранения уровня в файл Json

        // подробнее про структуру уровня в Json в Концепт-документе

        public List<Way> way = new List<Way>();
        public List<Wall> wall = new List<Wall>();
        public List<RuleTiles> ruleTiles = new List<RuleTiles>();
        public List<SingleSideTrap> singleSideTraps = new List<SingleSideTrap>();
        public List<MultySideTrap> multySideTraps = new List<MultySideTrap>();
        public List<Teleporter> portals = new List<Teleporter>();                     // из массивов по два элемента - портала
        public List<Pusher> pushers = new List<Pusher>();
        public List<Door> doors = new List<Door>();
        public List<TimeDoor> timeDoors = new List<TimeDoor>();


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
            public float x;
            public float y;
            public float z;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }
            public int id;
            public int d;
            public SingleSideTrap(Vector3 pos, int _tileID, int _direction) { x = pos.x; y = pos.y; z = pos.z; id = _tileID; d = _direction; }
        }

        [System.Serializable]
        public class MultySideTrap
        {
            public float x;
            public float y;
            public float z;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }
            public int id;
            public bool[] ds;
            public MultySideTrap(Vector3 pos, int _tileID, bool[] _directions) { x = pos.x; y = pos.y; z = pos.z; id = _tileID; ds = _directions; }
        }

        [System.Serializable]
        public class Teleporter
        {
            public Portal[] portals;
            public Teleporter(Portal[] _portals) { portals = _portals; }
        }

        [System.Serializable]
        public class Portal
        {
            public float x;
            public float y;
            public float z;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }

            public Portal(Vector3 pos) { x = pos.x; y = pos.y; z = pos.z; }
        }

        [System.Serializable]
        public class Pusher
        {
            public float x;
            public float y;
            public float z;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }

            public Pusher(Vector3 pos) { x = pos.x; y = pos.y; z = pos.z; }
        }

        [System.Serializable]
        public class Door
        {
            public float x;
            public float y;
            public float z;
            public Switcher switcher;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }

            public Door(Vector3 pos, Switcher _switcher) { x = pos.x; y = pos.y; z = pos.z; switcher = _switcher;  }
        }

        [System.Serializable]
        public class TimeDoor
        {
            public float x;
            public float y;
            public float z;
            public float time;
            public Switcher switcher;
            public Vector3 pos 
            { 
                get
                {
                    return new Vector3(x, y, z);
                } 
            }

            public TimeDoor(Vector3 pos, Switcher _switcher, float _time) { x = pos.x; y = pos.y; z = pos.z; switcher = _switcher; time = _time; }
        }

        [System.Serializable]
        public class Switcher
        {
            public float x;
            public float y;
            public float z;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x, y, z);
                }
            }

            public Switcher(Vector3 pos) { x = pos.x; y = pos.y; z = pos.z; }
        }

    }

}
