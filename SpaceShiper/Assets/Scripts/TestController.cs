using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestController : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile wall;
    public Tile way;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector3Int p = new Vector3Int(x, y, 0);
                bool odd = (x + y) % 2 == 1;
                Tile tile = odd ? way : wall;
                tilemap.SetTile(p, tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
