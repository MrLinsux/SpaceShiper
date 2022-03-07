using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ender : MonoBehaviour
{
    private GameController controller;
    public GameObject effect;

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if (collision.TryGetComponent<Player>(out player))
        {
            player.gameObject.SetActive(false);
            Instantiate(effect, this.transform.position, Quaternion.identity);
            Invoke("CompleteLevel", 3f);
        }
    }

    private void CompleteLevel()
    {
        
    }
}
