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
        var pi = Mathf.PI;
        var n = 1024;
        for (int x = 0; x < 2 * n; x++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.localScale = Vector3.one * 0.1f;
            obj.name = x.ToString();
            obj.transform.position = (2 * Mathf.Sqrt(2) * new Vector2(Mathf.Pow(Mathf.Cos(x * pi / n + pi / 4), 4), Mathf.Pow(Mathf.Sin(x * pi / n + pi / 4), 4)));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
