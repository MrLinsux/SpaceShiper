using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController gameController;
    public Player player;
    public GameObject controlPanel;
    public bool isTest = false;
    public AudioClip buttonClick;
    public GameObject soundController;
    public GameObject mainCamera;
    public MotherController motherController;

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
        int time = 2;
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

    public GameObject vibrBut;
    public GameObject soundBut;
    public GameObject musicBut;
    public GameObject soundSlider;
    public GameObject musicSlider;

    public void StartGame()
    {
        mainCanvas.SetActive(false);
        playerCanvas.SetActive(true);

        player.enabled = true;
    }

    public void ReverseVibrate()
    {
        Vibration.onVibrate = !Vibration.onVibrate;
    }
    public void ReverseButton(GameObject button)
    {
        if (button.GetComponent<Image>().color == MotherController.mainColors[2])
        {
            // отключаем
            button.GetComponent<Image>().color = new Color(0.4941177f, 0.5294118f, 0.5490196f);
        }
        else
        {
            // включаем
            button.GetComponent<Image>().color = MotherController.mainColors[2];
        }
    }
    public void ReverseSlider(GameObject slider)
    {
        if (slider.transform.GetChild(0).GetComponent<Image>().color == MotherController.mainColors[2] - new Color(0, 0, 0, 1-0.572549f))
        {
            // отключаем
            slider.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4941177f, 0.5294118f, 0.5490196f);
            slider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(0.4941177f, 0.5294118f, 0.5490196f);
            slider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(0.4941177f, 0.5294118f, 0.5490196f);
            slider.GetComponent<Slider>().interactable = false;

            EventTrigger trig;
            if (slider.TryGetComponent<EventTrigger>(out trig))
            {
                trig.triggers = new List<EventTrigger.Entry>();
            }
        }
        else
        {
            // включаем
            slider.transform.GetChild(0).GetComponent<Image>().color = MotherController.mainColors[2] - new Color(0, 0, 0, 1 - 0.572549f);
            slider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = MotherController.mainColors[2];
            slider.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = MotherController.mainColors[2];
            slider.GetComponent<Slider>().interactable = true;

            EventTrigger trig;
            if (slider.TryGetComponent<EventTrigger>(out trig))
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.EndDrag;
                entry.callback.AddListener((data) => { soundController.GetComponent<AudioSource>().PlayOneShot(buttonClick); });
                trig.triggers.Add(entry);
            }
        }
    }
    public void Vibrate(int time)
    {
        Vibration.Vibrate(time);
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
            case "OnDeathToggle":
                player.onDeath = toggle.isOn;
                break;
        }
    }
    #endregion

    private void Start()
    {
        if (PlayerPrefs.HasKey("onSound"))
        {
            // восстанавливаем настройки из реестра
            Vibration.onVibrate = (PlayerPrefs.GetInt("onVibration") == 1);
            if (!Vibration.onVibrate)
                ReverseButton(vibrBut);


            soundController.GetComponent<AudioSource>().mute = (PlayerPrefs.GetInt("onSound") == 1);
            soundController.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("soundVol");
            if (soundController.GetComponent<AudioSource>().mute)
            {
                ReverseButton(soundBut);
                ReverseSlider(soundSlider);
            }

            mainCamera.GetComponent<AudioSource>().mute = (PlayerPrefs.GetInt("onMusic") == 1);
            mainCamera.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVol");
            if (mainCamera.GetComponent<AudioSource>().mute)
            {
                ReverseButton(musicBut);
                ReverseSlider(musicSlider);
            }

            soundSlider.GetComponent<Slider>().value = soundController.GetComponent<AudioSource>().volume;
            musicSlider.GetComponent<Slider>().value = mainCamera.GetComponent<AudioSource>().volume;
        }


        if (isTest)
        {
            var gameParamsNames = new string[]
            {
            "TimeStopToggle",
            "PlayerSpeedInput",
            "MinSpeedForStartInput",
            "MinVectorForStartInput",
            "MinRotateMemoryDistance",
            "OnNewWheelToggle",
            "OnStartAnimToggle",
            "OnStep1AnimToggle",
            "OnStep1DelayToggle",
            "OnStartDelayToggle",
            "OnDeathToggle"
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
            gameParams[10].GetComponent<Toggle>().isOn = player.onDeath;
        }
    }
}
