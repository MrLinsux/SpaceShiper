using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap tilemap;     // основной холст для всех тайлов
    public GameObject player;
    public int world = 0; public int level = 0;
    public bool isEditLevel = false;
    public float worldSpeed = 1f;

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
        try
        {
            // точка появления игрока, которую мы получаем после построения уровня
            tilemap.GetComponent<Map>().BuildLevel(world, level);
        }
        catch (NullReferenceException)
        {
            Debug.LogError($"Level {world}-{level} is not found!");
            isEditLevel = true;
            Debug.LogWarning("Edit Mode On.");
        }
    }

    public void LoadLevel()
    {
        player.SetActive(true);
        LoadLevel(world, level);
    }

    private void Start()
    {
        LoadLevel(world, level);

        if (isEditLevel)
            Time.timeScale = 0;
        //player.GetComponent<Player>().enabled = false;
        Application.targetFrameRate = 60;
    }

    private void FixedUpdate()
    {
        Time.timeScale = worldSpeed;
    }

    public void CompleteLevel()
    {

    }
}
