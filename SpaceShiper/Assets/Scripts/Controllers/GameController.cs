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
    public MotherController motherController;
    public AudioClip gameAmbient;
    public AudioClip menuAmbient;
    public Text pauseMenuTitle;
    public int world = 0; public int level = 0;
    public bool isEditLevel = false;
    public float worldSpeed = 1f;
    public int stars = 0;
    public int points = 0;
    public int coins = 0;
    public bool isActiveLevel = true;

    public void LoadLevel(int world, int level)
    {
        // функция загрузки уровня
        ClearLevel();
        Time.timeScale = 1;
        stars = 0; points = 0; coins = 0;
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
        if (isEditLevel)
        {
            LoadLevel(world, level);
            Time.timeScale = 0;
            uIController.mainCanvas.SetActive(false);
        }
        else
        {
            uIController.mainCanvas.SetActive(true);
        }
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

        player.GetComponent<Player>().OffPlayer();
        player.SetActive(false);

        uIController.mainCanvas.SetActive(true);
        uIController.playerCanvas.SetActive(false);
        mainCamera.GetComponent<AudioSource>().clip = menuAmbient;
        mainCamera.GetComponent<AudioSource>().Play();
    }
    public void WinLevel()
    {
        // успешное завершение уровня
        ClearLevel();
        if (isActiveLevel)
        {
            // при прохождении нового уровня
            motherController.playerProgress.levels.Add(new MotherController.PlayerProgress.Level(world, level, stars));
            motherController.activeLevel++;
        }
        else
        {
            // при перепрохождении проёденного уровня
            // если набрал больше звёзд
            motherController.playerProgress.levels[(world - 65) * 18 + level].stars =
                (stars > motherController.playerProgress.levels[(world - 65) * 18 + level].stars) ? 
                stars : 
                motherController.playerProgress.levels[(world - 65) * 18 + level].stars;
        }

        motherController.Money = coins;

        motherController.SavePlayerProgress();
        motherController.LoadLevelSelector(new LevelSlider.Page(motherController.cW, motherController.cI));

        player.GetComponent<Player>().OffPlayer();
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
