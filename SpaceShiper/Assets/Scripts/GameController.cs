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

    private void BuildLevel(int world, int lvl)
    {
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
                    playerSpawner = tilemaps[0].CellToWorld(new Vector3Int(i, j, 0));
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

                Trap trap;
                blockIndex = Array.IndexOf(detWallPrefabs, levelScheme.GetPixel(i, j).gamma.linear);
                if (blockIndex > -1)
                {
                    trap = Instantiate(wallPrefabs[blockIndex], tilemaps[1].CellToWorld(new Vector3Int(i, j, 0)), Quaternion.identity).GetComponent<Trap>();
                    if(trap.type == Trap.TrapType.single)
                    {
                        if (levelScheme.GetPixel(i + 1, j).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.singleDirection = Trap.Direction.Right;
                        }
                        else if (levelScheme.GetPixel(i, j + 1).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.singleDirection = Trap.Direction.Up;
                        }
                        else if (levelScheme.GetPixel(i - 1, j).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.singleDirection = Trap.Direction.Left;
                        }
                        else if (levelScheme.GetPixel(i, j - 1).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.singleDirection = Trap.Direction.Down;
                        }
                    }
                    else if(trap.type == Trap.TrapType.multy)
                    {
                        if (levelScheme.GetPixel(i + 1, j).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.directions[(int)Trap.Direction.Right] = true;
                        }
                        else if (levelScheme.GetPixel(i, j + 1).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.directions[(int)Trap.Direction.Up] = true;
                        }
                        else if (levelScheme.GetPixel(i - 1, j).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.directions[(int)Trap.Direction.Left] = true;
                        }
                        else if (levelScheme.GetPixel(i, j - 1).gamma.linear == detDirectionWallPrefabs[blockIndex])
                        {
                            trap.directions[(int)Trap.Direction.Down] = true;
                        }
                    }

                    trap.SetTrap();
                }

            }
        }
    }

    private void Start()
    {
        BuildLevel(World, Level);
        player.transform.position = playerSpawner + new Vector3(0, 0, player.transform.position.z);
    }
}
