using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;
    public GameObject observer;

    void Start()
    {
        player.transform.position = this.transform.position + Vector3.up;
        observer.transform.position = this.transform.position + Vector3.up;
        player.SetActive(false);
    }

    private void ActivePlayer()
    {
        player.SetActive(true);
    }
}
