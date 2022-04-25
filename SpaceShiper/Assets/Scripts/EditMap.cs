using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EditMap : MonoBehaviour
{
    private Tilemap tilemap;             // компоненит тайлмапа

    // набор объектов, из которых составляется уровень
    public TileBase wayTile;                    // тайлы дороги
    public TileBase pointWay;                   // тоже дорога, но с точками
    public TileBase starWay;                    // тоже дорога, но со звёздами
    public TileBase ender;                      // выход с уровня
    public TileBase playerSpawner;            // объект начала уровня
    public TileBase pusher;                   // толкатель

    public GameObject[] singleSideTraps;        // ловушки одиночной направленности
    private string[] singleSideTrapsNames;      // и их имена
    public GameObject[] multySideTraps;         // ловушки множественной направленности
    private string[] multySideTrapsNames;       // и их имена

    // префабы
    public GameObject teleport;                 // телепорт
    public GameObject door;                     // обычная дверь
    public GameObject timeDoor;                 // дверь с таймером
    public GameObject thicker;                  // ловушка-бумеранг
    public int world = 65;
    public int level = 0;

    public void BuildLevel(int world, int level)
    {
        // загружаем схему карты из Ресурсов
        var mapPlan =
            JsonUtility.FromJson<MapTiles>(
                (Resources.Load("Levels\\" + world.ToString() + "\\" + level.ToString()) as TextAsset).text
                );

        tilemap.SetTile(tilemap.WorldToCell(mapPlan.playerSpawner.pos), playerSpawner);
        tilemap.SetTile(tilemap.WorldToCell(mapPlan.ender.pos), ender);

        this.world = world; this.level = level;

        for (int i = 0; i < mapPlan.way.Count; i++)
        {
            // ставим дорогу
            var way = mapPlan.way[i];
            tilemap.SetTile(way.pos, wayTile);
        }
        for (int i = 0; i < mapPlan.pointWay.Count; i++)
        {
            // ставим точки
            var pointWay = mapPlan.pointWay[i];
            tilemap.SetTile(pointWay.pos, this.pointWay);
        }
        for (int i = 0; i < mapPlan.stars.Count; i++)
        {
            // ставим звёзды
            var starWay = mapPlan.stars[i];
            tilemap.SetTile(tilemap.WorldToCell(starWay.pos), this.starWay);
        }
        for (int i = 0; i < mapPlan.pushers.Count; i++)
        {
            // размещаем толкатели
            var pusher = mapPlan.pushers[i];
            tilemap.SetTile(tilemap.WorldToCell(pusher.pos), this.pusher);
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
        for (int i = 0; i < mapPlan.portals.Count; i++)
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
        for (int i = 0; i < mapPlan.doors.Count; i++)
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
        for (int i = 0; i < mapPlan.thickers.Count; i++)
        {
            var thicker = mapPlan.thickers[i];
            var rotate = 45;

            if (thicker.posA.x == thicker.posB.x)
            {
                rotate = 0;
            }
            else if (thicker.posA.y == thicker.posB.y)
            {
                rotate = 90;
            }
            else
            {
                Debug.LogError("Rotate of thicker is not straight: " + thicker);
            }

            var t = Instantiate(this.thicker, thicker.pos, Quaternion.Euler(0, 0, rotate), this.transform);
            //TODO: доделать корректировку лаунчеров
            t.GetComponent<Thicker>().SetPoints(thicker.posA, thicker.posB);
            t.GetComponent<Thicker>().enabled = false;
        }

        //Instantiate(playerSpawner, mapPlan.playerSpawner.pos, Quaternion.identity, this.transform).GetComponent<PlayerSpawner>().player = player;
        Instantiate(ender, mapPlan.ender.pos, Quaternion.identity, this.transform);
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
                    if (tile == wayTile)
                    {
                        map.way.Add(new MapTiles.Way(x, y, z));
                    }
                    else if (tile == pointWay)
                    {
                        map.pointWay.Add(new MapTiles.PointWay(x, y, z));
                    }
                    else if (tile == starWay)
                    {
                        map.stars.Add(new MapTiles.Star(tilemap.CellToWorld(new Vector3Int(x, y, z))));
                    }
                    else if (tile == playerSpawner)
                    {
                        map.playerSpawner = new MapTiles.Spawner(tilemap.CellToWorld(new Vector3Int(x, y, z)));
                    }
                    else if(tile == ender)
                    {
                        map.ender = new MapTiles.Ender(tilemap.CellToWorld(new Vector3Int(x, y, z)));
                    }
                    else if(tile == pusher)
                    {
                        map.pushers.Add(new MapTiles.Pusher(tilemap.CellToWorld(new Vector3Int(x, y, z))));
                    }
                }
            }
        }

        Transform[] child = tilemap.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < child.Length; i++)
        {
            if (child[i].GetComponent<SingleSideTrap>())
            {
                map.singleSideTraps.Add(
                    new MapTiles.SingleSideTrap(
                        child[i].GetComponent<SingleSideTrap>().sourcePos,
                        Array.IndexOf(singleSideTrapsNames, child[i].name),
                        (int)child[i].GetComponent<SingleSideTrap>().singleDirection));
            }
            else if (child[i].GetComponent<MultySideTrap>())
            {
                var trapPos = tilemap.WorldToCell(child[i].position - new Vector3(0, 0, child[i].position.z));
                var directions = child[i].GetComponent<MultySideTrap>().directions;

                if (!IsWay(trapPos + Vector3Int.right))
                    directions[0] = false;
                if (!IsWay(trapPos + Vector3Int.up))
                    directions[1] = false;
                if (!IsWay(trapPos + Vector3Int.left))
                    directions[2] = false;
                if (!IsWay(trapPos + Vector3Int.down))
                    directions[3] = false;
                Debug.Log(tilemap.GetTile(trapPos + Vector3Int.up));

                map.multySideTraps.Add(
                    new MapTiles.MultySideTrap(
                        child[i].position,
                        Array.IndexOf(multySideTrapsNames, child[i].name),
                        directions));
            }
            else if (child[i].CompareTag("Teleport"))
            {
                map.portals.Add(new MapTiles.Teleporter(new MapTiles.Portal[2] {
                    new MapTiles.Portal(child[i].position),
                    new MapTiles.Portal(child[i].GetChild(1).position)
                }));
            }
            else if (child[i].GetComponent<Door>())
            {
                map.doors.Add(new MapTiles.Door(
                    child[i].position,
                    new MapTiles.Switcher(child[i].GetChild(0).position)
                    ));
            }
            else if (child[i].GetComponent<TimeDoor>())
            {
                map.timeDoors.Add(new MapTiles.TimeDoor(
                    child[i].position,
                    new MapTiles.Switcher(child[i].GetChild(0).position),
                    child[i].GetComponent<TimeDoor>().timeToClose
                    ));
            }
            else if (child[i].GetComponent<Thicker>())
            {
                map.thickers.Add(new MapTiles.Thicker(child[i].position, child[i].GetChild(1).position, child[i].GetChild(2).position));
            }
            else if (child[i].GetComponent<Star>())
            {
                map.stars.Add(new MapTiles.Star(child[i].position));
            }
        }

        if (!File.Exists(Application.dataPath + @"\Resources\Levels\" + world.ToString() + "\\" + level.ToString() + ".json"))
            File.Create(Application.dataPath + @"\Resources\Levels\" + world.ToString() + "\\" + level.ToString() + ".json");
        File.WriteAllText(
            Application.dataPath + @"\Resources\Levels\" + world.ToString() + "\\" + level.ToString() + ".json",
            JsonUtility.ToJson(map)
            );
        Debug.Log("Level saved");
    }
    public void SaveLevel()
    {
        SaveLevel(this.world, this.level);
    }

    public bool IsWay(Vector3Int pos)
    {
        return (wayTile == tilemap.GetTile(pos)) || (starWay == tilemap.GetTile(pos)) || (pointWay == tilemap.GetTile(pos));
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
            public SingleSideTrap(Vector3 pos, int _tileID, int _direction) { x = (int)Math.Round(pos.x - 0.5f); y = (int)Math.Round(pos.y - 0.5f); z = (int)Math.Round(pos.z); id = _tileID; d = _direction; }
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
                }
            }
            public int id;
            public bool[] ds;
            public MultySideTrap(Vector3 pos, int _tileID, bool[] _directions) { x = (int)Math.Round(pos.x - 0.5f); y = (int)Math.Round(pos.y - 0.5f); z = (int)Math.Round(pos.z); id = _tileID; ds = _directions; }
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

            public Portal(Vector3 pos) { x = (int)Math.Round(pos.x - 0.5f); y = (int)Math.Round(pos.y - 0.5f); z = (int)Math.Round(pos.z); }
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

            public Spawner(Vector3 pos) { x = (int)Math.Round(pos.x); y = (int)Math.Round(pos.y); z = (int)Math.Round(pos.z); }
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
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
                }
            }

            public float xA;
            public float yA;
            public float zA;
            public Vector3 posA
            { get { return new Vector3(xA + 0.5f, yA + 0.5f, zA); } }

            public float xB;
            public float yB;
            public float zB;
            public Vector3 posB
            { get { return new Vector3(xB + 0.5f, yB + 0.5f, zB); } }

            public Thicker(Vector3 pos, Vector3 _posA, Vector3 _posB) { 
                x = (int)Math.Round(pos.x-0.5f); y = (int)Math.Round(pos.y - 0.5f); z = (int)Math.Round(pos.z); 
                xA = (int)Math.Round(_posA.x - 0.5f); yA = (int)Math.Round(_posA.y - 0.5f); zA = (int)Math.Round(_posA.z); 
                xB = (int)Math.Round(_posB.x - 0.5f); yB = (int)Math.Round(_posB.y - 0.5f); zB = (int)Math.Round(_posB.z); }
        }

        [System.Serializable]
        public class Star
        {
            public int x;
            public int y;
            public int z;
            public Vector3 pos
            {
                get
                {
                    return new Vector3(x + 0.5f, y + 0.5f, z - 1);
                }
            }
            public Star(Vector3 pos) { x = (int)Math.Round(pos.x); y = (int)Math.Round(pos.y); z = (int)Math.Round(pos.z); }
        }
    }

}
