using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public Player player;
    public GameObject controlPanel;

    #region Play
    public GameObject playerCanvas;
    public GameObject pauseMenu;
    public Text timer;

    public void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
            StartCoroutine(ResumeGame());
    }
    private IEnumerator ResumeGame()
    {
        int time = 3;
        timer.transform.parent.gameObject.SetActive(true);
        pauseMenu.SetActive(false);
        while (time >= 0)
        {
            timer.text = time.ToString();
            yield return new WaitForSecondsRealtime(1f);
            time--;
        }
        Time.timeScale = 1;
        timer.transform.parent.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        gameController.LoadLevel(gameController.world, gameController.level);
        player.enabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        playerCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    #endregion

    #region Main
    public GameObject mainCanvas;
    public Text money;

    public void StartGame()
    {
        mainCanvas.SetActive(false);
        playerCanvas.SetActive(true);

        player.enabled = true;
    }

    public void Shop()
    {

    }
    public void LevelSelector()
    {

    }
    public void Settings()
    {

    }
    #endregion

    #region Dev
    public void SaveLevel()
    {
        gameController.tilemap.GetComponent<Map>().SaveLevel(gameController.world, gameController.level);
    }

    public void ChangeParam(Text playerParamValue)
    {
        switch (playerParamValue.name)
        {
            case "Speed":
                player.moveSpeed = (float)Convert.ToDouble(playerParamValue.text);
                playerParamValue.transform.parent.GetChild(playerParamValue.transform.parent.childCount - 1).GetComponent<Text>().text = "Speed = " + player.moveSpeed;
                break;
            case "MinSpeed":
                player.minSwipeSpeed = (float)Convert.ToDouble(playerParamValue.text);
                break;
            case "MinVector":
                player.minVDirection = (float)Convert.ToDouble(playerParamValue.text);
                break;
            case "MinRMDist":
                player.minDistanceForMR = Convert.ToInt32(playerParamValue.text);
                break;
            case "FixedTime":
                Time.fixedDeltaTime = (float)Convert.ToDouble(playerParamValue.text);
                break;
        }
    }
    public void TimeStop()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    public void ToggleSwitch(Toggle toggle)
    {
        switch (toggle.name)
        {
            case "OnNewWheelToggle":
                player.newWheelOn = toggle.isOn;
                break;
            case "OnStartAnimToggle":
                player.animator.SetBool("onStart", toggle.isOn);
                break;
            case "OnStep1AnimToggle":
                player.animator.SetBool("onStep1", toggle.isOn);
                break;
            case "OnStartDelayToggle":
                player.onStartDelay = toggle.isOn;
                break;
            case "OnStep1DelayToggle":
                player.onStep1Delay = toggle.isOn;
                break;
        }
    }
    #endregion

    private void Start()
    {
        var gameParamsNames = new string[] 
        {
            "TimeStopToggle",
            "PlayerSpeedInput",
            "MinSpeedForStartInput",
            "MinVectorForStartInput",
            "Min Rotate Memory Distance",
            "OnNewWheelToggle",
            "OnStartAnimToggle",
            "OnStep1AnimToggle",
            "OnStep1DelayToggle",
            "OnStartDelayToggle",
            "WheelDeltaAngleInput"
        };
        var gameParams = new Transform[gameParamsNames.Length];

        for (int i = 0; i < gameParams.Length; i++)
        {
            gameParams[i] = GameObject.Find(gameParamsNames[i]).transform;
        }


        gameParams[0].GetComponent<Toggle>().isOn = Time.timeScale != 0;
        gameParams[1].GetChild(0).GetComponent<Text>().text = player.moveSpeed.ToString();
        gameParams[2].GetChild(0).GetComponent<Text>().text = player.minSwipeSpeed.ToString();
        gameParams[3].GetChild(0).GetComponent<Text>().text = player.minVDirection.ToString();
        gameParams[4].GetChild(0).GetComponent<Text>().text = player.minDistanceForMR.ToString();
        gameParams[5].GetComponent<Toggle>().isOn = player.newWheelOn;
        gameParams[6].GetComponent<Toggle>().isOn = player.onStartDelay;
        gameParams[7].GetComponent<Toggle>().isOn = player.onStep1Delay;
        gameParams[8].GetComponent<Toggle>().isOn = player.animator.GetBool("onStep1");
        gameParams[9].GetComponent<Toggle>().isOn = player.animator.GetBool("onStart");
        gameParams[4].GetChild(0).GetComponent<Text>().text = player.minDistanceForMR.ToString();

    }
}
