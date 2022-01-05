using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public GameObject[] s;
    public int stars = 0;
    public int world = 0;
    public int level = 0;
    public bool isComplete = false;
    public GameController controller;

    public void SetButton(int _world, int _level)
    {
        for(int i = 0; i < stars; i++)
        {
            s[i].SetActive(true);
        }

        world = _world; level = _level;
    }

    public void LoadLevel()
    {
        controller.LoadLevel(world, level);
    }

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        if(!isComplete)
        {
            this.GetComponent<Image>().color = Color.yellow;
            for (int i = 0; i < s.Length; i++)
            {
                s[i].SetActive(false);
            }
        }
        else
        {

        }
    }
}
