using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float delay = 1f;
    public float minVDirection = 1f;

    public bool directionChosen;
    public bool isMove = false;
    public bool isAutoMove = false;
    private Vector3 end;
    public Direction direction = Direction.zero;
    public Direction mainDirection = Direction.zero;
    public Direction secondDirection = Direction.zero;
    private Vector2 startPos;
    private Vector2 vDirection;

    public static GameController controller;
    public Tilemap waysTilemap;

    public enum Direction { zero, right, up, left, down }

    public void Dead()
    {
        Debug.Log("Is Dead");
    }

    private IEnumerator Move(Vector3 end, Direction direction)
    {
        mainDirection = direction;
        isMove = true;
        this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)direction);
        yield return new WaitForFixedUpdate();
        while (this.transform.position != end)
        {
            yield return new WaitForFixedUpdate();
            this.transform.position = Vector3.MoveTowards(this.transform.position, end, moveSpeed);
        }
        isMove = false;
    }

    private IEnumerator PostMove(Vector3 end, Direction direction)
    {
        if (!isAutoMove && (direction == Direction.zero))
        {
            Debug.Log(end);
            isAutoMove = true;
            secondDirection = direction;
            while (isMove) { yield return new WaitForEndOfFrame(); }
            mainDirection = direction;
            isMove = true;
            this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)direction);
            yield return new WaitForFixedUpdate();
            while (this.transform.position != end)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, end, moveSpeed);

                yield return new WaitForFixedUpdate();
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
            if(collision.GetComponent<Portal>())
            {
                StopAllCoroutines();
                this.transform.position = collision.GetComponent<Portal>().GetTeleportPoint(mainDirection);
                StartCoroutine(Move(GetLastTileInCoridor(this.transform.position, mainDirection, waysTilemap), mainDirection));
                StartCoroutine(PostMove(GetLastTileInCoridor(GetLastTileInCoridor(this.transform.position, mainDirection, waysTilemap), secondDirection, waysTilemap), secondDirection));
            }
        }
    }

    void Start()
    {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        float deltaThouch = 0;
        Vector2 oldStartPos = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
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
                    deltaThouch = touch.deltaPosition.magnitude;
                    break;

                // убрал палец
                case TouchPhase.Ended:
                    directionChosen = false;
                    break;
            }
        }
        if (directionChosen && (deltaThouch > delay) && (vDirection.magnitude > minVDirection))
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
            else if(!isMove)
            {
                // устанавливаем целевой тайл
                end = GetLastTileInCoridor(
                    waysTilemap.transform.parent.GetComponent<GridLayout>().WorldToCell(this.transform.position),
                    direction,
                    waysTilemap
                    );   // тайл с игроком

                // начинаем движение с помощью корутины
                StartCoroutine(Move(end, direction));
            }

            directionChosen = false;
            vDirection = Vector2.zero;
        }
        if(Input.touchCount > 0)
        {
            startPos = Input.GetTouch(0).position;
        }
    }

    private static Vector3Int GetLastTileInCoridor(Vector3 start, Direction direction, Tilemap tilemapScheme)
    {
        int cellItemX = (int)start.x;
        int cellItemY = (int)start.y;
        switch (direction)
        {
            case Direction.right:
                while (IsThisTileFromWay(tilemapScheme, cellItemX, 1, cellItemY, 0))
                    cellItemX++;
                break;
            case Direction.up:
                while (IsThisTileFromWay(tilemapScheme, cellItemX, 0, cellItemY, 1))
                    cellItemY++;
                break;
            case Direction.left:
                while (IsThisTileFromWay(tilemapScheme, cellItemX, -1, cellItemY, 0))
                    cellItemX--;
                break;
            case Direction.down:
                while (IsThisTileFromWay(tilemapScheme, cellItemX, 0, cellItemY, -1))
                    cellItemY--;
                break;
        }

        return new Vector3Int(cellItemX, cellItemY, 0);

    }
    private static bool IsThisTileFromWay(Tilemap tilemapScheme, int i, int di, int j, int dj)
    {
        return Array.IndexOf(controller.tilemap.GetComponent<Map>().wayTiles, tilemapScheme.GetTile(new Vector3Int(i + di, j + dj, 0))) > -1;
    }
}
