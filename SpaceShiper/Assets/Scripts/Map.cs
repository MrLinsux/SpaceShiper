using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public GameObject player;           // объект игрока
    public GameObject observer;

    public Tilemap tilemap;             // компоненит тайлмапа

    // набор объектов, из которых составляется уровень
    public TileBase wayTile;             // тайлы дороги
    public TileBase coinTile;           // тоже дорога, но с точками
    //public TileBase wallTile;            // тайлы стены

    public GameObject[] singleSideTraps;        // ловушки одиночной направленности
    private string[] singleSideTrapsNames;      // и их имена
    public GameObject[] multySideTraps;         // ловушки множественной направленности
    private string[] multySideTrapsNames;       // и их имена

    // префабы
    public GameObject teleport;                 // телепорт
    public GameObject pusher;                   // толкатель
    public GameObject door;                     // обычная дверь
    public GameObject timeDoor;                 // дверь с таймером
    public GameObject playerSpawner;            // объект начала уровня
    public GameObject ender;                    // выход с уровня
    public GameObject thicker;                  // ловушка-бумеранг
    public GameObject coin;                     // монетка в виде денежного знака
    public GameObject point;                    // точка для отслеживания прогресса прохождения уровня
    public GameObject star;                     // звёзды
    public int totalPoints = 0;                 // общее число точек, нужно для отслеживания прогресса уровня

    public void BuildLevel(int world, int level)
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
            SetWayTile(way.pos, wayTile);
        }
        for (int i = 0; i < mapPlan.pointWay.Count; i++)
        {
            // ставим дорогу
            var coinWay = mapPlan.pointWay[i];
            SetCoinWayTile(coinWay.pos, coinTile);
        }
        for(int i = 0; i < mapPlan.stars.Count; i++)
        {
            // размещаем звёзды
            var stars = mapPlan.stars[i];

            tilemap.SetTile(stars.pos, wayTile);
            Instantiate(this.star, tilemap.CellToWorld(stars.pos), Quaternion.identity, tilemap.transform);
        }
        for (int i = 0; i < mapPlan.singleSideTraps.Count; i++)
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
            tilemap.SetTile(pusher.pos, wayTile);
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
        for(int i = 0; i < mapPlan.thickers.Count; i++)
        {
            var thicker = mapPlan.thickers[i];
            var rotate = 45;

            if(thicker.posA.x == thicker.posB.x)
            {
                rotate = 0;
            }
            else if(thicker.posA.y == thicker.posB.y)
            {
                rotate = 90;
            }
            else
            {
                Debug.LogError("Rotate of thicker is not straight: " + thicker);
            }

            Instantiate(this.thicker, thicker.pos, Quaternion.Euler(0, 0, rotate), this.transform).GetComponent<Thicker>().SetPoints(thicker.posA, thicker.posB);
        }

        var ps = Instantiate(playerSpawner, mapPlan.playerSpawner.pos, Quaternion.identity, this.transform).GetComponent<PlayerSpawner>();
        ps.player = player;
        ps.observer = observer;
        Instantiate(ender, mapPlan.ender.pos, Quaternion.identity, this.transform);
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

    private void SetWayTile(Vector3Int pos, TileBase tile)
    {
        // что-то вроде кисти, которая ставит дорогу, а вокруг стены

        //for (int i = -1; i <= 1; i++)
        //    for (int j = -1; j <= 1; j++)
        //        tilemap.SetTile(pos + i * Vector3Int.right + j * Vector3Int.up + Vector3Int.back, wallTile);

        tilemap.SetTile(pos, tile);
    }
    private void SetCoinWayTile(Vector3Int pos, TileBase tile)
    {
        // что-то вроде кисти, которая ставит дорогу, а вокруг стены

        //for (int i = -1; i <= 1; i++)
        //    for (int j = -1; j <= 1; j++)
        //        tilemap.SetTile(pos + i * Vector3Int.right + j * Vector3Int.up + Vector3Int.back, wallTile);

        tilemap.SetTile(pos, coinTile);
        if (UnityEngine.Random.value <= 0.11f)
        {
            Instantiate(coin, pos, Quaternion.identity, this.transform);
        }
        else
        {
            Instantiate(point, pos, Quaternion.identity, this.transform);
            totalPoints++;
        }
    }

    public class MapTiles
    {
        // класс описывает каждый элемент карты на сцене
        // необходимо для сохранения уровня в файл Json

        // подробнее про структуру уровня в Json в Концепт-документе

        public List<Way> way = new List<Way>();
        public List<PointWay> pointWay = new List<PointWay>();
        public List<RuleTiles> ruleTiles = new List<RuleTiles>();
        public List<SingleSideTrap> singleSideTraps = new List<SingleSideTrap>();
        public List<MultySideTrap> multySideTraps = new List<MultySideTrap>();
        public List<Teleporter> portals = new List<Teleporter>();                     // из массивов по два элемента - портала
        public List<Pusher> pushers = new List<Pusher>();
        public List<Door> doors = new List<Door>();
        public List<TimeDoor> timeDoors = new List<TimeDoor>();
        public List<Thicker> thickers = new List<Thicker>();
        public List<Star> stars = new List<Star>();
        public Spawner playerSpawner;
        public Ender ender;


        [System.Serializable]
        public class Way
        {
            public int x;
            public int y;
            public int z;
            public Vector3Int pos
            {
                get
                {
                    return new Vector3Int(x, y, z);
                }
            }
            public Way(int _cellX, int _cellY, int _cellZ) { x = _cellX; y = _cellY; z = _cellZ; }
        }

        [System.Serializable]
        public class PointWay
        {
            public int x;
            public int y;
            public int z;
            public Vector3Int pos
            {
                get
                {
                    return new Vector3Int(x, y, z);
                }
            }
            public PointWay(int _cellX, int _cellY, int _cellZ) { x = _cellX; y = _cellY; z = _cellZ; }
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
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
                    return new Vector3(x, y, z +50);
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
                }
            }

            public Portal(Vector3 pos) { x = pos.x; y = pos.y; z = pos.z; }
        }

        [System.Serializable]
        public class Pusher
        {
            public int x;
            public int y;
            public int z;
            public Vector3Int pos
            {
                get
                {
                    return new Vector3Int(x, y, z);
                }
            }

            public Pusher(Vector3 pos) { x = (int)Math.Round(pos.x); y = (int)Math.Round(pos.y); z = (int)Math.Round(pos.z); }
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
                }
            }

            public Door(Vector3 pos, Switcher _switcher) { x = pos.x; y = pos.y; z = pos.z; switcher = _switcher; }
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
                }
            }

            public Switcher(Vector3 pos) { x = pos.x; y = pos.y; z = pos.z; }
        }

        [System.Serializable]
        public class Spawner
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

            public Spawner(Vector3 pos) { x = pos.x; y = pos.y; z = pos.z; }
        }

        [System.Serializable]
        public class Ender
        {
            public int x;
            public int y;
            public int z;
            public Vector3Int pos
            {
                get
                {
                    return new Vector3Int(x, y, z);
                }
            }
            public Ender(Vector3 pos) { x = (int)Math.Round(pos.x); y = (int)Math.Round(pos.y); z = (int)Math.Round(pos.z); }
        }

        [System.Serializable]
        public class Thicker
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

            public float xA;
            public float yA;
            public Vector3 posA
            { get { return new Vector3(xA, yA, 0); } }

            public float xB;
            public float yB;
            public Vector3 posB
            { get { return new Vector3(xB, yB, 0); } }

            public Thicker(Vector3 pos, Vector3 _posA, Vector3 _posB)
            {
                x = (int)Math.Round(pos.x - 0.5f); y = (int)Math.Round(pos.y - 0.5f); z = (int)Math.Round(pos.z);
                xA = (int)Math.Round(_posA.x - 0.5f); yA = (int)Math.Round(_posA.y - 0.5f);
                xB = (int)Math.Round(_posB.x - 0.5f); yB = (int)Math.Round(_posB.y - 0.5f);
            }
        }

        [System.Serializable]
        public class Star
        {
            public int x;
            public int y;
            public int z;
            public Vector3Int pos
            {
                get
                {
                    return new Vector3Int(x, y, z);
                }
            }
            public Star(Vector3 pos) { x = (int)Math.Round(pos.x); y = (int)Math.Round(pos.y); z = (int)Math.Round(pos.z); }
        }
    }

}
