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

    public int Level
    {
        get 
        {
            return level;
        }
        set
        {
            if (value >= 0)
                level = value;
        }
    }
    private int level = 0;

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

    private void Start()
    {
        
    }

    [Serializable]
    class SaveData
    {
        public int money;
        public int world;
        public int level;
    }
}
