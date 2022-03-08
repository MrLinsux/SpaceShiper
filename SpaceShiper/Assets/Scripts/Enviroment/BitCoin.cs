using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitCoin : MonoBehaviour
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
            gameController.coins++;
            Destroy(this.gameObject);
        }
    }
}
