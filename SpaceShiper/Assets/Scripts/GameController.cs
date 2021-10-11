using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap[] tilemaps;     // основной холст для всех тайлов

    // тайлы

    public Texture2D levelScheme;

    public int World = 0; public int Level = 0;

    public GameObject player;
    public Vector3 playerSpawner;

    // это массивы для тайлов и цветов
    // i цвет пикселя определяет i тайл
    public Tile[] wayTiles;
    public GameObject[] wayPrefabs;
    public Color[] detWayTiles;
    public Color[] detWayPrefabs;

    public Tile[] wallTiles;
    public GameObject[] wallPrefabs;
    public Color[] detWallTiles;
    public Color[] detWallPrefabs;              // определяет тип ловушки
    public Color[] detDirectionWallPrefabs;     // определяет направление ловушки

    private Vector3 zNomalizer;                 // нормализирует объект по оси z

    private void BuildLevel(int world, int lvl)
    {
        zNomalizer = Vector3.forward * tilemaps[0].transform.parent.position.z;

        // это для tilemap "дороги"
        levelScheme = Resources.Load<Texture2D>("Levels\\" + world.ToString() + "\\" + lvl.ToString() + "_way");
        for (int i = 0; i < levelScheme.width; i++)
        {
            for (int j = 0; j < levelScheme.height; j++)
            {
                var blockIndex = Array.IndexOf(detWayTiles, levelScheme.GetPixel(i, j).gamma.linear);
                if (blockIndex > -1)
                    tilemaps[0].SetTile(new Vector3Int(i, j, 0), wayTiles[blockIndex]);

                if (levelScheme.GetPixel(i, j).gamma.linear == Color.green)
                    playerSpawner = 
                        tilemaps[0].CellToWorld(new Vector3Int(i, j, 0)) - zNomalizer;
            }
        }

        // это для tilemap "стены"
        levelScheme = Resources.Load<Texture2D>("Levels\\" + world.ToString() + "\\" + lvl.ToString() + "_wall");
        for (int i = 0; i < levelScheme.width; i++)
        {
            for (int j = 0; j < levelScheme.height; j++)
            {
                var blockIndex = Array.IndexOf(detWallTiles, levelScheme.GetPixel(i, j).gamma.linear);
                if (blockIndex > -1) 
                {
                    tilemaps[1].SetTile(new Vector3Int(i, j, 0), wallTiles[blockIndex]);
                    continue;
                }

                GameObject trap;
                blockIndex = Array.IndexOf(detWallPrefabs, levelScheme.GetPixel(i, j).gamma.linear);
                if (blockIndex > -1)
                {
                    trap = Instantiate(
                        wallPrefabs[blockIndex], 
                        tilemaps[1].CellToWorld(new Vector3Int(i, j, 0)) - zNomalizer, 
                        Quaternion.identity
                        );

                    if (trap.GetComponent<Trap>())
                    {
                        if (trap.GetComponent<Trap>().type == Trap.TrapType.single)
                        {
                            if (IsThisPixelDirection(i, 1, j, 0, blockIndex))
                            {
                                trap.GetComponent<Trap>().singleDirection = Trap.Direction.Right;
                            }
                            else if (IsThisPixelDirection(i, 0, j, 1, blockIndex))
                            {
                                trap.GetComponent<Trap>().singleDirection = Trap.Direction.Up;
                            }
                            else if (IsThisPixelDirection(i, -1, j, 0, blockIndex))
                            {
                                trap.GetComponent<Trap>().singleDirection = Trap.Direction.Left;
                            }
                            else if (IsThisPixelDirection(i, 0, j, -1, blockIndex))
                            {
                                trap.GetComponent<Trap>().singleDirection = Trap.Direction.Down;
                            }
                        }
                        else if (trap.GetComponent<Trap>().type == Trap.TrapType.multy)
                        {
                            if (levelScheme.GetPixel(i + 1, j).gamma.linear == detDirectionWallPrefabs[blockIndex])
                            {
                                trap.GetComponent<Trap>().directions[(int)Trap.Direction.Right] = true;
                            }
                            else if (levelScheme.GetPixel(i, j + 1).gamma.linear == detDirectionWallPrefabs[blockIndex])
                            {
                                trap.GetComponent<Trap>().directions[(int)Trap.Direction.Up] = true;
                            }
                            else if (levelScheme.GetPixel(i - 1, j).gamma.linear == detDirectionWallPrefabs[blockIndex])
                            {
                                trap.GetComponent<Trap>().directions[(int)Trap.Direction.Left] = true;
                            }
                            else if (levelScheme.GetPixel(i, j - 1).gamma.linear == detDirectionWallPrefabs[blockIndex])
                            {
                                trap.GetComponent<Trap>().directions[(int)Trap.Direction.Down] = true;
                            }
                        }

                        trap.GetComponent<Trap>().SetTrap();
                    }
                    else if(trap.GetComponent<Thicker>())
                    {
                        int k = 1;
                        trap.GetComponent<Thicker>().startPos = tilemaps[1].CellToWorld(new Vector3Int(i, j, 0)) - zNomalizer;

                        if (IsThisPixelDirection(i, 1, j, 0, blockIndex))
                        {
                            while (levelScheme.GetPixel(i + k + 1, j).gamma.linear != Color.black)
                                k++;

                            trap.GetComponent<Thicker>().endPos = tilemaps[1].CellToWorld(new Vector3Int(i + k, j, 0)) - zNomalizer;

                        }
                        else if (IsThisPixelDirection(i, 0, j, 1, blockIndex))
                        {
                            while (levelScheme.GetPixel(i, j + k + 1).gamma.linear != Color.black)
                                k++;

                            trap.GetComponent<Thicker>().endPos = tilemaps[1].CellToWorld(new Vector3Int(i, j + k, 0)) - zNomalizer;
                        }
                        else if (IsThisPixelDirection(i, -1, j, 0, blockIndex))
                        {
                            while (levelScheme.GetPixel(i - k - 1, j).gamma.linear != Color.black)
                                k--;

                            trap.GetComponent<Thicker>().endPos = tilemaps[1].CellToWorld(new Vector3Int(i - k, j, 0)) - zNomalizer;
                        }
                        else if (IsThisPixelDirection(i, 0, j, -1, blockIndex))
                        {
                            while (levelScheme.GetPixel(i, j - k - 1).gamma.linear != Color.black)
                                k--;

                            trap.GetComponent<Thicker>().endPos = tilemaps[1].CellToWorld(new Vector3Int(i, j - k, 0)) - zNomalizer;
                        }
                    }
                }

            }
        }
    }

    private bool IsThisPixelDirection(int i, int di, int j, int dj, int blockIndex)
    {
        return levelScheme.GetPixel(i + di, j + dj).gamma.linear == detDirectionWallPrefabs[blockIndex];
    }

    private void Start()
    {
        BuildLevel(World, Level);
        player.transform.position = playerSpawner;
    }
}
