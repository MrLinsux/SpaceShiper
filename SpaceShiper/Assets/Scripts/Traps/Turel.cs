using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Turel : MonoBehaviour
{
    public Map map;
    public Vector2 startPos;
    public Vector2 endPos;

    public Color shotColor;
    public Color chargeColor;
    public Color offColor;

    public SpriteRenderer point;
    public Animator canon;
    public GameObject bullet;
    public Transform bulletSpawn;

    void Start()
    {
        endPos = startPos = this.transform.position;
        map = GameObject.Find("Map").GetComponent<Map>();
        endPos = ((Vector2Int)map.GetLastTileInCoridor(startPos, (Player.Direction)this.GetComponent<SingleSideTrap>().singleDirection));
        var collider = this.GetComponent<BoxCollider2D>();
        collider.size = new Vector2((endPos - startPos).magnitude - 0.5f, 1);
        collider.offset = new Vector2(((endPos - startPos).magnitude + 0.5f) / 2, 0);
    }

    public void Shot()
    {
        point.color = shotColor;
        Instantiate(bullet, bulletSpawn.transform.position, this.transform.rotation);
    }
    public void Charge()
    {
        point.color = chargeColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if(collision.TryGetComponent(out player))
        {
            this.GetComponent<Animator>().SetBool("hasPlayer", true);
            this.transform.GetChild(0).GetComponent<Animator>().SetBool("isActive", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player;
        if (collision.TryGetComponent(out player))
        {
            this.GetComponent<Animator>().SetBool("hasPlayer", false);
            this.transform.GetChild(0).GetComponent<Animator>().SetBool("isActive", false);
            point.color = offColor;
        }
    }
}
