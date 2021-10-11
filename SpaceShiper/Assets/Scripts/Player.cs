using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public Vector2 startPos;
    public Vector2 vDirection;
    public bool directionChosen;
    public Tilemap waysTilemap;
    public string[,] tilemapScheme;
    public bool isMove = false;
    public bool isAutoMove = false;
    public float n = 1;
    private Vector3Int end;
    Direction direction = Direction.zero;

    private enum Direction { zero, right, up, left, down }

    public void Dead()
    {
        Debug.Log("Is Dead");
    }

    private IEnumerator Move(Vector3Int end, Direction direction)
    {
        isMove = true;
        this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)direction);
        while (this.transform.position != end)
        {
            // перемещаемся на 1 с определённой скоростью
            this.transform.position = Vector3.MoveTowards(this.transform.position, end, 1/10f);

            yield return new WaitForSeconds(1/moveSpeed);
        }
        isMove = false;
    }

    private IEnumerator PostMove(Vector3Int end, Direction direction)
    {
        if (!isAutoMove)
        {
            isAutoMove = true;
            while (isMove) { yield return new WaitForEndOfFrame(); }
            isMove = true;
            this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)direction);
            while (this.transform.position != end)
            {
                // перемещаемся на 1 с определённой скоростью
                this.transform.position = Vector3.MoveTowards(this.transform.position, end, 1 / 10f);

                yield return new WaitForSeconds(1 / moveSpeed);
            }
            isMove = false;
            isAutoMove = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Trap"))
            {
                Dead();
            }
        }
    }

    void Start()
    {
        Debug.Log(Direction.down.ToString());
        BoundsInt bounds = waysTilemap.cellBounds;
        tilemapScheme = new string[bounds.size.x, bounds.size.y];
        TileBase[] allTiles = waysTilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    tilemapScheme[x, y] = tile.name;
                }
            }
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touch = Input.GetTouch(0);
            // Handle finger movements based on touch phase.
            switch (touch.phase)
            {
                // нажал пальцем
                case TouchPhase.Began:
                    startPos = touch.position;
                    directionChosen = false;
                    break;

                // провёл пальцем
                case TouchPhase.Moved:
                    directionChosen = true;
                    vDirection = touch.position - startPos;
                    break;

                // убрал палец
                case TouchPhase.Ended:
                    directionChosen = false;
                    break;
            }
        }
        if (directionChosen && vDirection.magnitude > n)
        {
            Touch touch = Input.GetTouch(0);
            var pi = Mathf.PI;
            var xAxis = Vector2.right;
            float dirAngle = (
                vDirection.y > 0 ?
                Mathf.Acos(Vector2.Dot(vDirection.normalized, xAxis)) :
                2 * pi - Mathf.Acos(Vector2.Dot(vDirection.normalized, xAxis))
                );

            #region Direction determine
            var secDirection = direction;

            if ((dirAngle < pi / 4) || (dirAngle >= 7 * pi / 4))
            {
                direction = (Direction)1;
            }
            else if ((dirAngle < 3 * pi / 4) && (dirAngle >= pi / 4))
            {
                direction = (Direction)2;
            }
            else if ((dirAngle < 5 * pi / 4) && (dirAngle >= 3 * pi / 4))
            {
                direction = (Direction)3;
            }
            else if ((dirAngle < 7 * pi / 4) && (dirAngle >= 5 * pi / 4))
            {
                direction = (Direction)4;
            }
            #endregion
            if (isMove && (secDirection != direction))
            {
                end = GetLastTileInCoridor(
                    end,
                    direction,
                    waysTilemap
                    );   // тайл с игроком
                StartCoroutine(PostMove(end, direction));
            }
            else
            {
                // устанавливаем целевой тайл
                end = GetLastTileInCoridor(
                    waysTilemap.transform.parent.GetComponent<GridLayout>().WorldToCell(this.transform.position),
                    direction,
                    waysTilemap
                    );   // тайл с игроком
                Debug.Log(end);

                // начинаем движение с помощью корутины
                if (!isMove)
                {
                    StartCoroutine(Move(end, direction));
                }
            }

            directionChosen = false;
            startPos = touch.position;
            vDirection = Vector2.zero;
        }
    }

    private static Vector3Int GetLastTileInCoridor(Vector3Int start, Direction direction, Tilemap tilemapScheme)
    {
        int cellItemX = start.x;
        int cellItemY = start.y;
        switch (direction)
        {
            case Direction.right:
                while (tilemapScheme.GetTile(new Vector3Int(cellItemX + 1, cellItemY, 0)) != null)
                    cellItemX++;
                break;
            case Direction.up:
                while (tilemapScheme.GetTile(new Vector3Int(cellItemX, cellItemY + 1, 0)) != null)
                    cellItemY++;
                break;
            case Direction.left:
                while (tilemapScheme.GetTile(new Vector3Int(cellItemX - 1, cellItemY, 0)) != null)
                    cellItemX--;
                break;
            case Direction.down:
                while (tilemapScheme.GetTile(new Vector3Int(cellItemX, cellItemY - 1, 0)) != null)
                    cellItemY--;
                break;
        }

        return new Vector3Int(cellItemX, cellItemY, 0);

    }
}
