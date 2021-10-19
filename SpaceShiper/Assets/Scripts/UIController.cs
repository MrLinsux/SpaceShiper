using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text timer;

    public void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
        }
        else
            StartCoroutine(ResumeGame());
    }

    private IEnumerator ResumeGame()
    {
        int time = 3;
        timer.transform.parent.gameObject.SetActive(true);
        while (time >= 0)
        {
            timer.text = time.ToString();
            yield return new WaitForSecondsRealtime(1f);
            time--;
        }
        Time.timeScale = 1;
        timer.transform.parent.gameObject.SetActive(false);
    }
}
