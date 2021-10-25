using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap tilemap;     // основной холст для всех тайлов
    public GameObject player;
    public int world = 0; public int level = 0;

    public void LoadLevel(int world, int level)
    {
        // функция загрузки уровня
        // отчищает его и строит указанный
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        // тут удаляем
        // тайлы
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
        // игровые объекты
        for(int i = 0; i < tilemap.transform.childCount; i++)
            Destroy(tilemap.transform.GetChild(i).gameObject, 0);

        // точка появления игрока, которую мы получаем после построения уровня
        var playerSpawn = tilemap.GetComponent<Map>().BuildLevel(world, level);

        player.transform.position = playerSpawn;
    }

    private void Start()
    {
        LoadLevel(0, 0);
        //player.GetComponent<Player>().enabled = false;
        Application.targetFrameRate = 30;
    }
}
