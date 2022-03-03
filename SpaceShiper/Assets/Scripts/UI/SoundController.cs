using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public void ReverseSound(AudioSource source)
    {
        source.mute = !source.mute;
    }
}
