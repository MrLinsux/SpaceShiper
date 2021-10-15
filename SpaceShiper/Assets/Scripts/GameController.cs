using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap tilemap;     // основной холст для всех тайлов

    // тайлы

    public int World = 0; public int Level = 0;



    private void Start()
    {
        Application.targetFrameRate = 30;
        //player.transform.position = playerSpawner;
    }
}
