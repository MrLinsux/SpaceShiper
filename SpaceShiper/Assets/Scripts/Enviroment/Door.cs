using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    public Transform switcher;
    public Tilemap tilemap;
    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
        set
        {
            isOpen = value;
            SwitchState();
        }
    }
    private bool isOpen = false;

    void Start()
    {
        switcher = this.transform.GetChild(0);
        tilemap = this.transform.parent.GetComponent<Tilemap>();
        IsOpen = false;
    }

    private void SwitchState()
    {
        tilemap.SetTile(tilemap.WorldToCell(this.transform.position), isOpen ? tilemap.GetComponent<Map>().wayTile[0] : null);
        this.GetComponent<SpriteRenderer>().enabled = !isOpen;
    }
}
