using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public Player player;

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
    #endregion
}
