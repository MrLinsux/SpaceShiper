using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    public AudioClip ui_ButtonClick;

    public void ClickSound()
    {
        this.GetComponent<AudioSource>().PlayOneShot(ui_ButtonClick);
    }
}
