using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pusher : MonoBehaviour
{
    public Player.Direction verticalEnter;
    public Player.Direction horizontalEnter;

    void Start()
    {
        // определям направление толкателя
        var anim = this.transform.GetChild(0).GetComponent<Animator>();
        anim.speed = 1 / Time.fixedDeltaTime;
        var map = GameObject.Find("Map").GetComponent<Tilemap>();
        var pos = map.WorldToCell(this.transform.position);
        this.transform.Rotate(new Vector3(0, 0, 0));
        verticalEnter = Player.Direction.down;
        horizontalEnter = Player.Direction.left;

        if (map.GetComponent<Map>().wayTile == map.GetTile(pos + Vector3Int.right))
        {
            if (map.GetComponent<Map>().wayTile == map.GetTile(pos + Vector3Int.down))
            {
                this.transform.Rotate(new Vector3(0, 0, 270));
                verticalEnter = Player.Direction.up;
                horizontalEnter = Player.Direction.left;
            }
        }
        else if(map.GetComponent<Map>().wayTile == map.GetTile(pos + Vector3Int.left))
        {
            if (map.GetComponent<Map>().wayTile == map.GetTile(pos + Vector3Int.up))
            {
                this.transform.Rotate(new Vector3(0, 0, 90));
                verticalEnter = Player.Direction.down;
                horizontalEnter = Player.Direction.right;
            }
            else if (map.GetComponent<Map>().wayTile == map.GetTile(pos + Vector3Int.down))
            {
                this.transform.Rotate(new Vector3(0, 0, 180));
                verticalEnter = Player.Direction.up;
                horizontalEnter = Player.Direction.right;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.CompareTag("Player"))
            {
                this.transform.GetChild(0).GetComponent<Animator>().SetBool("isPushing", true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Player"))
            {
                this.transform.GetChild(0).GetComponent<Animator>().SetBool("isPushing", false);
            }
        }
    }
}
