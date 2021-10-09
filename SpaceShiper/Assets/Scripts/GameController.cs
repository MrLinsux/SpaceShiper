using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public List<Tilemap> tilemaps = new List<Tilemap>();     // основной холст для всех тайлов

    // тайлы
    public Tile[] tiles;

    public Texture2D levelScheme;

    public int World = 0; public int Level = 0;

    public GameObject player;
    public Vector3 playerSpawner;

    private void BuildLevel(int world, int lvl)
    {

        for (int map = 0; map < tilemaps.Count; map++)
        {
            levelScheme = Resources.Load<Texture2D>("Levels\\" + world.ToString() + "\\" + lvl.ToString() + "_" + tiles[map].name);
            for (int i = 0; i < levelScheme.width; i++)
            {
                for (int j = 0; j < levelScheme.height; j++)
                {
                    if(levelScheme.GetPixel(i, j).a != 0)
                        tilemaps[map].SetTile(new Vector3Int(i, j, 0), tiles[map]);

                    if (levelScheme.GetPixel(i, j).gamma == Color.green)
                        playerSpawner = tilemaps[map].CellToWorld(new Vector3Int(i, j, 0));
                }
            }
        }
    }

    private void Start()
    {
        BuildLevel(World, Level);
        player.transform.position = playerSpawner + new Vector3(0, 0, player.transform.position.z);
    }
}
