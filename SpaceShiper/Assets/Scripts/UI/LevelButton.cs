using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public enum LevelStatus { complete, active, close };        // все возможные состояния кнопки уровня: завершён, активен (незавершённый, но доступный) и закрытый соответственно
    public GameObject[] s;  // тут хранятся ссылки на иконки звёзд кнопки
    public int stars = 0;   // число звёзд
    public int world = 0;   // номер мира
    public int level = 0;   // номер уровня в мире
    public GameController controller;
    public MotherController motherController;
    public Image line;          // каждая кнопка имеет свою линию, которая полупрозрачная, если уровень не пройден
    

    public void SetButton(int _world, LevelStatus status, int stars, int _level)
    {
        // устанавливает параметры кнопки
        world = _world;
        level = _level;
        this.name = level.ToString();
        for (int i = 0; i < 3; i++)
        {
            s[i].SetActive(false);
        }
        line.color = motherController.mainColors[0] - new Color(0f, 0f, 0f, 0.5f);

        // тут раскрашиваем кнопку соответственно её статусу
        if (status == LevelStatus.active)
        {
            this.GetComponent<Image>().color = Color.yellow;
            this.transform.localScale = new Vector3(1.1f, 1.1f, 1);

            this.transform.GetChild(0).GetComponent<Text>().text = level.ToString();
            this.GetComponent<Button>().interactable = true;
        }
        else if(status == LevelStatus.close)
        {
            var text = this.transform.GetChild(0).GetComponent<Text>();
            this.transform.GetChild(0).GetComponent<Text>().text = "X";

            text.color = motherController.mainColors[4];

            this.GetComponent<Button>().interactable = false;
            var img = this.GetComponent<Image>();
            img.color = motherController.mainColors[0] - new Color(0f, 0f, 0f, 0.5f);
        }
        else if(status == LevelStatus.complete)
        {
            line.color = motherController.mainColors[0];
            this.GetComponent<Button>().interactable = true;
            for (int i = 0; i < stars; i++)
            {
                s[i].SetActive(true);
            }

            level = Convert.ToInt32(this.name);

            this.transform.GetChild(0).GetComponent<Text>().text = level.ToString();
            this.transform.GetChild(0).GetComponent<Text>().color = new Color(0.5660378f, 0.5660378f, 0.5660378f);
            this.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Debug.LogError("Level " + level + " is not load!");
            this.GetComponent<Image>().color = Color.red;
            this.GetComponent<Button>().interactable = false;
        }
    }

    public void LoadLevel()
    {
        //controller.LoadLevel(world, level);
        Debug.Log("Level " + world + " - " + level);
    }

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        motherController = GameObject.Find("MotherController").GetComponent<MotherController>();
        line = GameObject.Find("Line (" + name + ")").GetComponent<Image>();
    }
}
