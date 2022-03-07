using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Tilemap tilemap;     // основной холст для всех тайлов
    public GameObject player;
    public UIController uIController;
    public GameObject mainCamera;
    public SoundController soundController;
    public AudioClip gameAmbient;
    public AudioClip menuAmbient;
    public Text pauseMenuTitle;
    public int world = 0; public int level = 0;
    public bool isEditLevel = false;
    public float worldSpeed = 1f;

    public void LoadLevel(int world, int level)
    {
        // функция загрузки уровня
        ClearLevel();
        Time.timeScale = 1;
        pauseMenuTitle.text = "LVL " + ((char)world).ToString() + "-" + level.ToString();
        try
        {
            // точка появления игрока, которую мы получаем после построения уровня
            this.world = world; this.level = level;
            player.SetActive(true);
            tilemap.GetComponent<Map>().BuildLevel(world, level);
            uIController.mainCanvas.SetActive(false);
            uIController.playerCanvas.SetActive(true);
            mainCamera.GetComponent<AudioSource>().clip = gameAmbient;
            mainCamera.GetComponent<AudioSource>().Play();


            if (isEditLevel) Debug.LogWarning("Edit Mode On.");
        }
        catch (NullReferenceException)
        {
            Debug.LogError($"Level {world}-{level} is not found!");
            CompleteLevel();
        }
    }

    public void LoadLevel()
    {
        player.SetActive(true);
        LoadLevel(world, level);
    }

    private void Start()
    {
        //LoadLevel(world, level);

        //if (isEditLevel)
        //    Time.timeScale = 0;
        ////player.GetComponent<Player>().enabled = false;
        Application.targetFrameRate = 60;
    }

    private void FixedUpdate()
    {
        Time.timeScale = worldSpeed;
    }

    public void CompleteLevel()
    {
        ClearLevel();

        // точка появления игрока, которую мы получаем после построения уровня
        player.SetActive(false);

        uIController.mainCanvas.SetActive(true);
        uIController.playerCanvas.SetActive(false);
        mainCamera.GetComponent<AudioSource>().clip = menuAmbient;
        mainCamera.GetComponent<AudioSource>().Play();
    }

    public void ClearLevel()
    {
        // отчищает уровень
        BoundsInt bounds = tilemap.cellBounds;

        // тут удаляем
        // тайлы
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for(int z = bounds.zMin; z <= bounds.zMax; z++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, z), null);
                }
            }
        }
        // игровые объекты
        for (int i = 0; i < tilemap.transform.childCount; i++)
            Destroy(tilemap.transform.GetChild(i).gameObject, 0);
    }
}
