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

    public void SetButton(int _world, LevelStatus status, int stars)
    {
        world = _world;
        level = Convert.ToInt32(this.name);
        for (int i = 0; i < 3; i++)
        {
            s[i].SetActive(false);
        }

        if (status == LevelStatus.active)
        {
            this.GetComponent<Image>().color = Color.yellow;
            this.transform.localScale = new Vector3(1.1f, 1.1f, 1);

            this.transform.GetChild(0).GetComponent<Text>().text = level.ToString();
            this.GetComponent<Button>().interactable = false;
        }
        else if(status == LevelStatus.close)
        {
            var textColor = this.transform.GetChild(0).GetComponent<Text>().color;
            this.transform.GetChild(0).GetComponent<Text>().text = "x";
            textColor.a = 0.5f;
            this.GetComponent<Button>().interactable = false;
            var color = this.GetComponent<Image>().color;
            color = Color.white;
            color.a = 0.5f;
        }
        else if(status == LevelStatus.complete)
        {
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
    }
}
