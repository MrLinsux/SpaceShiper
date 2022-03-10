using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MotherController : MonoBehaviour
{
    // Это самый главный файл для все игры
    // тут мы управляем главным меню, определяем настрокий, запускаем уровень и т.д.
    public int World
    {
        get
        {
            return world;
        }
        set
        {
            if (value >= 0)
                world = value;
        }
    }
    private int world = 0;
    public GameObject soundController;
    public GameObject mainCamera;
    public UIController uiController;

    public int cW;  // текущий мир
    public int cI;  // текущий индекс страницы

    public int activeWorld;
    public int activeLevel
    {
        get
        {
            return _activeLevel;
        }
        set
        {
            if(value > 53)
            {
                activeWorld++;
                _activeLevel = 0;
            }
            else if(value < 0)
            {
                activeWorld--;
                _activeLevel = 53;
            }
            else
            {
                _activeLevel = value;
            }
        }
    }
    public int _activeLevel;
    public PlayerProgress playerProgress;

    public int Money
    {
        get
        {
            return (int)(_money * Mathf.Pow(10, orderMoney));
        }
        set
        {
            if(value >= 0)
            {
                _money = value;
                orderMoney = 0;
                while (_money >= 1)
                {
                    orderMoney++;
                    _money /= 10;
                }
            }

            uiController.money.text = Money.ToString();
        }
    }
    private int orderMoney = 0;
    private float _money = 0;

    public LevelButton[] levels;
    public Text worldIcon;

    public GameObject levelsPanel;

    public static Color[] mainColors = new Color[]
    {
        new Color(0.8745099f, 0.8980393f, 0.9058824f),  // светло-серый
        new Color(0.5568628f, 0.8980393f, 0.5411765f),  // светло-зелёный
        new Color(1, 0.9411765f, 0),                    // жёлтый
        new Color(1, 0.5882353f, 0),                    // оранжевый
        new Color(0.7764707f, 0.2588235f, 0.2588235f),  // красный
        new Color(0.7921569f, 0.2745098f, 0.6156863f),  // тёмно-розовый
        new Color(0.5176471f, 0.2745098f, 0.7921569f),  // фиолетовый
        new Color(0.5921569f, 0.8588236f, 1)            // светло-голубой
    };

    private void Start()
    {
        LoadPlayerProgress();
    }

    public void LoadLevelSelector(LevelSlider.Page page)
    {
        Level[] levelsType = new Level[18];
        System.Random rand = new System.Random();
        World = page.W;
        var activePage = new LevelSlider.Page(activeWorld, Mathf.FloorToInt(activeLevel / 18));
        activeWorld = activePage.W;

        if (page.W < activePage.W)
        {
            // до текущей страницы
            for (int i = 0; i < 18; i++)
                levelsType[i] = new Level(World, i + (18 * page.I), 0, playerProgress.levels[(page.W-65+page.I)*18+i].stars);
        }
        else if(page.W > activePage.W)
        {
            // после текущей страницы
            for (int i = 0; i < 18; i++)
                levelsType[i] = new Level(World, i + (18 * page.I), 2, 0);
        }
        else if(page.W == activePage.W)
        {
            if (page.I < activePage.I)
            {
                // до текущей страницы
                for (int i = 0; i < 18; i++)
                    levelsType[i] = new Level(World, i + (18 * page.I), 0, playerProgress.levels[(page.W - 65 + page.I) * 18 + i].stars);
            }
            else if (page.I > activePage.I)
            {
                // после текущей страницы
                for (int i = 0; i < 18; i++)
                    levelsType[i] = new Level(World, i + (18 * page.I), 2, 0);
            }
            else
            {
                // на текущей странице
                for (int i = 0; i < activeLevel; i++)
                    levelsType[i] = new Level(World, i + (18 * page.I), 0, playerProgress.levels[(page.W - 65 + page.I) * 18 + i].stars);

                levelsType[activeLevel] = new Level(World, activeLevel + (18 * page.I), 1, 0);

                for (int i = activeLevel + 1; i < 18; i++)
                    levelsType[i] = new Level(World, i + (18 * page.I), 2, 0);
            }
        }

        levels = new LevelButton[18];

        for (int i = 18; i < 18 + 18; i += 6)
        {
            for (int j = 0; j < 3; j++)
            {
                levels[i + j - 18] = levelsPanel.transform.GetChild(i + j).GetComponent<LevelButton>();
                levels[i + j - 18].SetButton(world, (LevelButton.LevelStatus)levelsType[i + j - 18].type, levelsType[i + j - 18].stars, levelsType[i + j - 18].level);
            }
        }
        for (int i = 21; i < 18 + 18; i += 6)
        {
            for (int j = 2; j >= 0; j--)
            {
                levels[i + j - 18] = levelsPanel.transform.GetChild(i + j).GetComponent<LevelButton>();
                levels[i + j - 18].SetButton(world, (LevelButton.LevelStatus)levelsType[i + 2 - j - 18].type, levelsType[i + 2 - j - 18].stars, levelsType[i + 2 - j - 18].level);
            }
        }
        if ((int)(Screen.currentResolution.width / Screen.currentResolution.height) == (int)(16 / 9))
            levelsPanel.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        worldIcon.text = ((char)page.W).ToString();
        cW = page.W; cI = page.I;
        SaveCurrentPage();
    }

    public void LoadLevelSelector()
    {
        // перезагрузка страницы уровня
        LoadLevelSelector(new LevelSlider.Page(cW, cI));
    }

    [Serializable]
    class Level
    {
        public int world = 0;
        public int level = 0;
        public int type = 0;
        public int stars = 0;

        public Level(int _world, int _level, int _type, int _stars) { world = _world; level = _level; type = _type; stars = _stars; }
    }

    [Serializable]
    public class PlayerProgress
    {
        [Serializable]
        public class Level
        {
            public int world = 0;
            public int level = 0;
            public int stars = 0;

            public Level(int _world, int _level, int _stars) { world = _world; level = _level; stars = _stars; }
        }

        public List<Level> levels = new List<Level>();      // тут храняться все уровни, которые игрок уже прошёл
        public int money;
    }

    public void SavePlayerProgress()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + "/SystemData.dat");
        playerProgress.money = Money;
        formatter.Serialize(fs, playerProgress);
        fs.Close();
        Debug.Log("Player Progress saved");
    }

    public void LoadPlayerProgress()
    {
        if (File.Exists(Application.persistentDataPath + "/SystemData.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + "/SystemData.dat", FileMode.Open);
            playerProgress = (PlayerProgress)formatter.Deserialize(fs);
            fs.Close();
            activeWorld = playerProgress.levels[playerProgress.levels.Count - 1].world;
            activeLevel = playerProgress.levels[playerProgress.levels.Count - 1].level;
            activeLevel++;
            Money = playerProgress.money;
            Debug.Log("Player Progress loaded");
        }
        else
        {
            activeWorld = 65;
            activeLevel = 0;
            Money = 0;
            Debug.LogWarning("Player Progress is not detected");
        }
    }

    public void SaveSettingsPrefs()
    {
        PlayerPrefs.SetInt("onVibration", Vibration.onVibrate ? 1 : 0);
        PlayerPrefs.SetInt("onSound", soundController.GetComponent<AudioSource>().mute ? 1 : 0);
        PlayerPrefs.SetInt("onMusic", mainCamera.GetComponent<AudioSource>().mute ? 1 : 0);

        PlayerPrefs.SetFloat("soundVol", soundController.GetComponent<AudioSource>().volume);
        PlayerPrefs.SetFloat("musicVol", mainCamera.GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();
    }

    public void SaveCurrentPage()
    {
        PlayerPrefs.SetInt("currentWorld", cW);
        PlayerPrefs.SetInt("currentIndex", cI);
        PlayerPrefs.Save();
    }

    public void ResetPlayerProgress()
    {
        playerProgress.levels = new List<PlayerProgress.Level>();
        activeLevel = 0;
        activeWorld = 65;
        cW = 65; cI = 0;
        LoadLevelSelector();
        Money = 0;
        if (File.Exists(Application.persistentDataPath + "/SystemData.dat"))
            File.Delete(Application.persistentDataPath + "/SystemData.dat");
    }
}
