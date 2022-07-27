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
    public Image[] pageIndexes;
    public LevelSlider levelSlider;
    public Player player;
    public SkinButton currentSkin;

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

    public Skin[] skins;

    public GameObject levelsPanel;
    public GameObject skinsPanel;
    public GameObject skinsSelector;

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

        // загружаем скины
        string[] list = ((TextAsset)Resources.Load("Skins\\list")).text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);        // список скинов, которые будут подгружаться в магазин
        skins = new Skin[list.Length];
        for (int i = 0; i < list.Length; i++)
        {
            if (Resources.Load("Skins\\" + list[i] + "\\conf") != null)
            {
                skins[i] = new Skin(((TextAsset)Resources.Load("Skins\\" + list[i] + "\\conf")).text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries), list[i], i);
                skins[i].isUnlock = playerProgress.openSkins.Exists(s => s == list[i]);
            }
            else
                skins[i] = new Skin("darkAvatar");
        }                                        // конфиги всех скинов в магазине
        skinsSelector.GetComponent<SkinSlider>().maxPage = skins.Length / 12;
    }

    public void LoadLevelSelector(LevelSlider.Page page)
    {
        Debug.Log(page.W + "-"+page.I);
        Level[] levelsType = new Level[18];
        World = page.W;
        for(int i = 0; i < 3; i++)
        {
            pageIndexes[i].color = new Color(0.5254902f, 0.5254902f, 0.5254902f);
        }
        pageIndexes[page.I].color = MotherController.mainColors[0];
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
        levelSlider.currentPage = page;
        SaveCurrentPage();
    }

    public void LoadLevelSelector()
    {
        // перезагрузка страницы уровня
        LoadLevelSelector(new LevelSlider.Page(cW, cI));
    }

    public void LoadActiveLevelPage()
    {
        LoadLevelSelector(new LevelSlider.Page(activeWorld, Mathf.FloorToInt(activeLevel/18)));
    }
    public void LoadSkinSelector(int page)
    {
        Debug.Log("Skin Menu Page: " + page);

        if (page > (skins.Length / 12)) // не пускаем страницу, которая слишком далеко зашла
            return;


        var buttons = skinsPanel.transform.GetComponentsInChildren<SkinButton>(true);
        int skinsNumOnPage = (page == (skins.Length / 12)) ? skins.Length - page * 12 : 12;
        for (int i = 0; i < skinsNumOnPage; i++)
        {
            buttons[i].gameObject.SetActive(true);
            var skin = skins[page * 12 + i];
            buttons[i].SetButton(skin);
            buttons[i].GetComponent<Animator>().SetBool("isSelect", false);
        }
        for(int i = skinsNumOnPage; i < 12; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        currentSkin = null;
        for(int i = 0; i < skinsNumOnPage; i++)
        {
            // включаем скин игрока
            if(buttons[i].Name == player.GetComponent<SkinRender>().Name)
            {
                buttons[i].SetActive(true);
                currentSkin = buttons[i];
                break;
            }
        }
    }

    public class Skin
    {
        public int id;
        public string name;
        public bool isUnlock;
        public int cost;
        public SkinButton.SkinType type;

        public Skin(string[] conf, string name, int id)
        {
            this.name = name;
            this.id = id;
            // конструктор разбирает конфиг и записывает необходимые данные
            for(int i = 0; i < conf.Length; i++)
            {
                var confStr = conf[i].Split('=');
                switch (confStr[0])
                {
                    case "cost":
                        cost = Convert.ToInt32(confStr[1]);
                        break;
                    case "type":
                        switch(confStr[1])
                        {
                            case "normal":
                                type = SkinButton.SkinType.normal;
                                break;
                            case "rare":
                                type = SkinButton.SkinType.rare;
                                break;
                            case "unique":
                                type = SkinButton.SkinType.unique;
                                break;
                        }
                        break;
                }
            }
        }

        public Skin(string name)
        {
            this.name = name;
        }
        public Skin(string name, int cost, bool isUnlock, SkinButton.SkinType type, int id)
        {
            this.name = name;
            this.cost = cost;
            this.isUnlock = isUnlock;
            this.type = type;
            this.id = id;
        }
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

        public List<Level> levels = new List<Level>();          // тут храняться все уровни, которые игрок уже прошёл
        public int money;
        public string currentSkin;
        public List<string> openSkins = new List<string>();     // скины, которые есть у игрока
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
            try
            {
                activeWorld = playerProgress.levels[playerProgress.levels.Count - 1].world;
                activeLevel = playerProgress.levels[playerProgress.levels.Count - 1].level;
                activeLevel++;
            }
            catch
            {
                Debug.LogError("There isn't active level in player save");
                activeWorld = 65;
                activeLevel = 0;
            }
            Money = playerProgress.money;
            player.GetComponent<SkinRender>().Name = playerProgress.currentSkin == "" ? "on" : playerProgress.currentSkin;
            if (playerProgress.openSkins == null || playerProgress.openSkins.Count < 0)
                playerProgress.openSkins = new List<string>() { "on" };

            Debug.Log("Player Progress loaded");
        }
        else
        {
            Debug.LogWarning("Player Progress is not detected");
            activeWorld = 65;
            activeLevel = 0;
            _money = 0;
            player.GetComponent<SkinRender>().Name = "on";
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
        PlayerPrefs.SetInt("currentWorld", activeWorld);
        PlayerPrefs.SetInt("currentIndex", Mathf.FloorToInt(activeLevel / 18));
        PlayerPrefs.Save();
    }

    public void ResetPlayerProgress()
    {
        playerProgress.levels = new List<PlayerProgress.Level>();
        playerProgress.currentSkin = "on";
        playerProgress.openSkins = new List<string>() { "on" };
        activeLevel = 0;
        activeWorld = 65;
        cW = 65; cI = 0;
        _money = 0;
        if (File.Exists(Application.persistentDataPath + "/SystemData.dat"))
            File.Delete(Application.persistentDataPath + "/SystemData.dat");

        SavePlayerProgress();
        Application.Quit();
    }
}
