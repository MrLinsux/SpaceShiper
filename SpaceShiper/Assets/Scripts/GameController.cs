using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap tilemap;     // основной холст для всех тайлов

    private void Start()
    {
        Application.targetFrameRate = 30;
    }
}
