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
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<Player>().enabled = false;
        player.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void ActivePlayer()
    {
        player.GetComponent<SpriteRenderer>().enabled = true;
    }
    private void StartPlayer()
    {
        player.GetComponent<Animator>().enabled = true;
        player.GetComponent<Player>().enabled = true;
        player.GetComponent<SpriteRenderer>().enabled = true;
    }
}
