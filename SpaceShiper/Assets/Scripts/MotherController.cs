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
        }
    }
    private int orderMoney = 0;
    private float _money = 0;

    public LevelButton[] levels;
    public Text worldIcon;

    public GameObject levelsPanel;

    public Color[] mainColors = new Color[]
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
    }

    public void LoadLevelSelector(LevelSlider.Page page, LevelSlider.Page activePage, int activeLevel)
    {
        Level[] levelsType = new Level[18];
        System.Random rand = new System.Random();
        World = page.W;

        if (page.W < activePage.W)
        {
            // до текущей страницы
            for (int i = 0; i < 18; i++)
                levelsType[i] = new Level(World, i + (18 * page.I), 0, rand.Next(0, 4));
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
                    levelsType[i] = new Level(World, i + (18 * page.I), 0, rand.Next(0, 4));
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
                    levelsType[i] = new Level(World, i + (18 * page.I), 0, rand.Next(0, 4));

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
    class SaveData
    {
        public int money;
        public int world;
        public int level;
    }
}
