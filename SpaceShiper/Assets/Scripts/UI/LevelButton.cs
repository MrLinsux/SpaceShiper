using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public enum LevelStatus { complete, active, close };        // все возможные состояния кнопки уровня: завершён, активен (незавершённый, но доступный) и закрытый соответственно
    private LevelStatus status;
    public GameObject[] s;  // тут хранятся ссылки на иконки звёзд кнопки
    public int stars = 0;   // число звёзд
    public int world = 0;   // номер мира
    public int level = 0;   // номер уровня в мире
    public GameController controller;
    public MotherController motherController;
    public Image line;          // каждая кнопка имеет свою линию, которая полупрозрачная, если уровень не пройден
    

    public void SetButton(int _world, LevelStatus status, int _stars, int _level)
    {
        // устанавливает параметры кнопки
        world = _world;
        level = _level;
        stars = _stars;
        this.status = status;
        this.name = level.ToString();
        for (int i = 0; i < 3; i++)
        {
            s[i].SetActive(false);
        }
        line.color = MotherController.mainColors[0] - new Color(0f, 0f, 0f, 0.5f);

        // тут раскрашиваем кнопку соответственно её статусу
        if (status == LevelStatus.active)
        {
            this.GetComponent<Image>().color = MotherController.mainColors[7];
            this.transform.localScale = new Vector3(1.1f, 1.1f, 1);

            this.transform.GetChild(0).GetComponent<Text>().text = level.ToString();
            this.GetComponent<Button>().interactable = true;
            this.transform.GetChild(0).GetComponent<Text>().color = new Color(0.5660378f, 0.5660378f, 0.5660378f);
        }
        else if(status == LevelStatus.close)
        {
            this.transform.localScale = Vector3.one;
            var text = this.transform.GetChild(0).GetComponent<Text>();
            this.transform.GetChild(0).GetComponent<Text>().text = "X";

            text.color = MotherController.mainColors[4];

            this.GetComponent<Button>().interactable = false;
            var img = this.GetComponent<Image>();
            img.color = MotherController.mainColors[0] - new Color(0f, 0f, 0f, 0.5f);
        }
        else if(status == LevelStatus.complete)
        {
            this.transform.localScale = Vector3.one;
            line.color = MotherController.mainColors[0];
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

    public void PrepareLevel(GameObject loadLevelPanel)
    {
        loadLevelPanel.transform.GetChild(0).GetComponent<Text>().text = "LVL " + level.ToString();

        for (int i = 0; i < stars; i++)
            loadLevelPanel.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color(0.5411765f, 0.8980393f, 0.5411765f);

        for (int i = stars; i < 3; i++)
            loadLevelPanel.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = new Color(0.4433962f, 0.4433962f, 0.4433962f);


        loadLevelPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { 
            controller.LoadLevel(world, level); 
            loadLevelPanel.SetActive(false);
            controller.isActiveLevel = (status == LevelStatus.active); });

        loadLevelPanel.SetActive(true);
    }

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        motherController = GameObject.Find("MotherController").GetComponent<MotherController>();
        line = GameObject.Find("Line (" + name + ")").GetComponent<Image>();
    }
}
