using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public enum LevelStatus { complete, active, close };
    public GameObject[] s;
    public int stars = 0;
    public int world = 0;
    public int level = 0;
    public GameController controller;
    public MotherController motherController;
    public Image line;
    

    public void SetButton(int _world, LevelStatus status, int stars)
    {
        world = _world;
        level = Convert.ToInt32(this.name);
        for (int i = 0; i < 3; i++)
        {
            s[i].SetActive(false);
        }
        line.color = new Color(0.8745099f, 0.8980393f, 0.9058824f, 0.5f);

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

            text.color = new Color(0.7764707f, 0.2588235f, 0.2588235f);
            //text.color = motherController.mainColors[4];

            this.GetComponent<Button>().interactable = false;
            var img = this.GetComponent<Image>();
            img.color = new Color(0.8745099f, 0.8980393f, 0.9058824f, 0.5f);
        }
        else if(status == LevelStatus.complete)
        {
            line.color = new Color(0.8745099f, 0.8980393f, 0.9058824f, 1);
            this.GetComponent<Button>().interactable = true;
            for (int i = 0; i < stars; i++)
            {
                s[i].SetActive(true);
            }

            level = Convert.ToInt32(this.name);

            this.transform.GetChild(0).GetComponent<Text>().text = level.ToString();
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
        controller.LoadLevel(world, level);
    }

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        motherController = GameObject.Find("MotherController").GetComponent<MotherController>();
        line = GameObject.Find("Line (" + name + ")").GetComponent<Image>();
    }
}
