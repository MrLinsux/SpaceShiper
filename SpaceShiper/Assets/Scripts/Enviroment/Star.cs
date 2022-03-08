using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public GameController gameController;
    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if (collision.TryGetComponent<Player>(out player))
        {
            gameController.stars++;
            Destroy(this.gameObject);
        }
    }
}
