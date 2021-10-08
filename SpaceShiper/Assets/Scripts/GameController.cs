using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap tilemap;     // основной холст для всех тайлов

    // тайлы
    public Tile way;
    public Tile wall;

    public Texture2D[][] allLevelSchemes;

    public Texture2D levelScheme;

    private void BuildLevel(int world, int lvl)
    {

    }

    private void Start()
    {
        
    }
}
