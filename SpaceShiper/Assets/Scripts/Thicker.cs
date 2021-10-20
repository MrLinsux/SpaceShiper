using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Thicker : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed;
    public float pause;

    IEnumerator SlowDown()
    {
        var temp = speed;
        speed = 0;
        yield return new WaitForSeconds(pause);
        speed = temp;
    }

    private void Start()
    {
        endPos = startPos = this.transform.position;
        
        Vector3Int direction = Vector3Int.zero;
        var tilemap = GameObject.Find("Map").GetComponent<Tilemap>();

        switch((int)this.GetComponent<SingleSideTrap>().singleDirection)
        {
            case 0:
                direction = Vector3Int.right;
                break;
            case 1:
                direction = Vector3Int.up;
                break;
            case 2:
                direction = Vector3Int.left;
                break;
            case 3:
                direction = Vector3Int.down;
                break;
        }

        int wayLong = 0;

        while(direction != Vector3Int.zero)
        {
            if (Array.IndexOf(tilemap.GetComponent<Map>().wallTiles, tilemap.GetTile(tilemap.WorldToCell(startPos + direction * wayLong))) > -1) 
            {
                break;
            }
            else
            {
                wayLong++;
            }
        }

        endPos += direction * (wayLong - 1);
    }


    void FixedUpdate()
    {
        if (Vector3.Distance(this.transform.position, startPos) >= Vector3.Distance(startPos, endPos))
        {
            StartCoroutine(SlowDown());
            startPos = startPos + endPos;
            endPos = startPos - endPos;
            startPos = startPos - endPos;
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, endPos, speed/100);
        }
    }
}
