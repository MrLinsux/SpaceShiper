using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour
{
    public EditMap tilemap;
    public int world = 65;
    public int level = 0;

    void Start()
    {
        tilemap.BuildLevel(world, level);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving...");
            tilemap.SaveLevel();
        }
    }
}
