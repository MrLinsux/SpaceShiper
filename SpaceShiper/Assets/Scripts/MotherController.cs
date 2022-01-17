using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

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

    public GameObject levelsPanel;

    public Color[] mainColors = new Color[]
    {
        new Color(0.8745099f, 0.8980393f, 0.9058824f),
        new Color(0.5568628f, 0.8980393f, 0.5411765f),
        new Color(1, 0.9411765f, 0),
        new Color(1, 0.5882353f, 0),
        new Color(0.7764707f, 0.2588235f, 0.2588235f),
        new Color(0.7921569f, 0.2745098f, 0.6156863f),
        new Color(0.5176471f, 0.2745098f, 0.7921569f),
        new Color(0.5921569f, 0.8588236f, 1)
    };

    private void Start()
    {
        Level[] levelsType = new Level[18];
        System.Random rand = new System.Random();

        for (int i = 0; i < 13; i++)
            levelsType[i] = new Level(0, rand.Next(0, 4));

        levelsType[13] = new Level(1, 0);

        for (int i = 14; i < 18; i++)
            levelsType[i] = new Level(2, 0);

        levels = new LevelButton[18];

        for (int i = 18; i < 18 + 18; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                levels[i + j - 18] = levelsPanel.transform.GetChild(i + j).GetComponent<LevelButton>();
                levels[i + j - 18].SetButton(world, (LevelButton.LevelStatus)levelsType[i + j - 18].type, levelsType[i + j - 18].stars);
            }
        }
        for (int i = 21; i < 18 + 18; i += 3)
        {
            for (int j = 2; j >= 0; j--)
            {
                levels[i + j - 18] = levelsPanel.transform.GetChild(i + j).GetComponent<LevelButton>();
                levels[i + j - 18].SetButton(world, (LevelButton.LevelStatus)levelsType[i + j - 18].type, levelsType[i + j - 18].stars);
            }
        }
        if ((int)(Screen.currentResolution.width/Screen.currentResolution.height) == (int)(16/9))
            levelsPanel.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
    }

    [Serializable]
    class Level
    {
        public int type = 0;
        public int stars = 0;

        public Level(int _type, int _stars)
        {
            type = _type; stars = _stars;
        }
    }
    class SaveData
    {
        public int money;
        public int world;
        public int level;
    }
}
