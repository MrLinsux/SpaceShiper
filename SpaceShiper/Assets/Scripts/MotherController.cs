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

    private void Start()
    {
        Level[] levelsType = new Level[18];

        for (int i = 0; i < 13; i++)
            levelsType[i] = new Level(0, 3);

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
        for (int i = 22; i < 18 + 18; i += 3)
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
