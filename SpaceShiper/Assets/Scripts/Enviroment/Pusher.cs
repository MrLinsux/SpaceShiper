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
        var map = GameObject.Find("Map").GetComponent<Tilemap>();
        var pos = map.WorldToCell(this.transform.position);
        this.transform.Rotate(new Vector3(0, 0, 0));
        verticalEnter = Player.Direction.down;
        horizontalEnter = Player.Direction.left;

        if (Array.IndexOf(map.GetComponent<Map>().wayTiles, map.GetTile(pos + Vector3Int.right)) > -1)
        {
            if (Array.IndexOf(map.GetComponent<Map>().wayTiles, map.GetTile(pos + Vector3Int.down)) > -1)
            {
                this.transform.Rotate(new Vector3(0, 0, 270));
                verticalEnter = Player.Direction.up;
                horizontalEnter = Player.Direction.left;
            }
        }
        else if(Array.IndexOf(map.GetComponent<Map>().wayTiles, map.GetTile(pos + Vector3Int.left)) > -1)
        {
            if (Array.IndexOf(map.GetComponent<Map>().wayTiles, map.GetTile(pos + Vector3Int.up)) > -1)
            {
                this.transform.Rotate(new Vector3(0, 0, 90));
                verticalEnter = Player.Direction.down;
                horizontalEnter = Player.Direction.right;
            }
            else if (Array.IndexOf(map.GetComponent<Map>().wayTiles, map.GetTile(pos + Vector3Int.down)) > -1)
            {
                this.transform.Rotate(new Vector3(0, 0, 180));
                verticalEnter = Player.Direction.up;
                horizontalEnter = Player.Direction.right;
            }
        }
    }
}
