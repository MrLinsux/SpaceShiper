using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ender : MonoBehaviour
{
    private GameController controller;
    private Animator anim;

    private void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
        anim = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if (collision.TryGetComponent(out player))
        {
            player.GetComponent<Animator>().Play("Idle");
            player.GetComponent<SpriteRenderer>().flipX = false;
            player.transform.rotation = new Quaternion(0, 0, 0, 0);
            player.GetComponent<SpriteRenderer>().enabled = false;
            anim.SetBool("IsDespawn", true);
            player.enabled = false;

            Invoke("CompleteLevel", 3f);
        }
    }

    private void CompleteLevel()
    {
        controller.WinLevel();
    }
}
