using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Turel : MonoBehaviour
{
    public float activeTime = 1f;
    public float passiveTime = 1f;
    public Tilemap map;
    private Laser[] lasers;
    public Vector3 startPos;
    public Vector3 endPos;

    private void SetActiveLasers(bool val)
    {
        for (int i = 0; i < lasers.Length; i++)
            lasers[i].gameObject.SetActive(val);
    }

    void Start()
    {
        List<Laser> _lasers = new List<Laser>() { this.transform.GetChild(0).GetComponent<Laser>() };
        endPos = startPos = this.transform.position;

        Vector3Int direction = Vector3Int.zero;
        var tilemap = GameObject.Find("Map").GetComponent<Tilemap>();

        switch ((int)this.GetComponent<SingleSideTrap>().singleDirection)
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

        int wayLong = 2;

        while (direction != Vector3Int.zero)
        {
            if (Array.IndexOf(tilemap.GetComponent<Map>().wallTiles, tilemap.GetTile(tilemap.WorldToCell(startPos + direction * wayLong))) > -1)
            {
                break;
            }
            else
            {
                _lasers.Add(Instantiate(_lasers[0].gameObject, startPos + direction * wayLong, _lasers[0].transform.rotation, this.transform).GetComponent<Laser>());
                wayLong++;
            }
        }
        lasers = _lasers.ToArray();
        StartCoroutine(Strike());
    }

    IEnumerator Strike()
    {
        while(true)
        {
            SetActiveLasers(false);
            yield return new WaitForSeconds(passiveTime);
            SetActiveLasers(true);
            yield return new WaitForSeconds(activeTime);
        }
    }
}
