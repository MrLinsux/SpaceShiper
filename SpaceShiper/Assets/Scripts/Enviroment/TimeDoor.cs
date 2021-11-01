using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TimeDoor : MonoBehaviour
{
    public Transform switcher;
    public Tilemap tilemap;
    public float timeToClose = 1f;

    void Start()
    {
        switcher = this.transform.GetChild(0);
        tilemap = this.transform.parent.GetComponent<Tilemap>();
        tilemap.SetTile(tilemap.WorldToCell(this.transform.position), null);
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Open()
    {
        StartCoroutine(RevertState());
    }
    private IEnumerator RevertState()
    {
        tilemap.SetTile(tilemap.WorldToCell(this.transform.position), tilemap.GetComponent<Map>().wayTile);
        this.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(timeToClose);

        tilemap.SetTile(tilemap.WorldToCell(this.transform.position), null);
        this.GetComponent<SpriteRenderer>().enabled = true;
    }
}
