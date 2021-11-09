using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;        // �������� �������� ������
    public float minSwipeSpeed = 1f;    // ����������� �������� ������ ������� ��� ������ ��������
    public float minVDirection = 1f;    // ����������� ����� ������ ��� ������ ��������
    public int minPostMoveDistance = 5;

    public bool directionChosen;        // ������ �� ������ �������� 
    public bool isMove = false;         // ��������� �� ����� � ��������
    public bool isAutoMove = false;     // ������� �� �������� ������ ��������
    private Vector3 end;                // ����� � ������� �������� �����
    public Direction direction = Direction.zero;            // ������� �����������
    public Direction mainDirection = Direction.zero;        // ����������� � �������� Move()
    public Direction secondDirection = Direction.zero;      // ����������� � �������� PostMove()
    private Coroutine movement;                             // ���������� ��� �������� Move()
    private Coroutine secMovement;                          // ���������� ��� �������� PostMove()
    private Vector2 startPos;                               // ����� ������ ������� ������
    private Vector2 vDirection;                             // ������ ������
    public bool rotateMemoryOn = true;                      // ������� ������ ��������
    private int wasTeleported;

    public GameObject tail;                                 // ���� ������
    public GameController controller;                // ������� ����������
    public Tilemap tilemap;                                 // ������ Map
    private Animator animator;                              // �������� ������

    public enum Direction { zero, right, up, left, down }   // ��� ��������� ����������� ��������

    public void Dead()
    {
        // ������� ������ ������
        //Debug.Log("Is Dead");
    }

    private IEnumerator Move(Direction direction)
    {
        // ������� ��������� ��������
        mainDirection = direction;
        isMove = true;
        animator.SetBool("isMove", true);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Move"));
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Start"));

        end = GetLastTileInCoridor(
            tilemap.WorldToCell(this.transform.position),
            direction,
            tilemap);           // �����, � ������� ����� �����
        GameObject _tail = Instantiate(tail, this.transform.position, Quaternion.identity);
        this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)direction);        // ������� � ����������� ��������

        yield return new WaitForFixedUpdate();                                      // ���������� ����� ����� ��� ���� ������� ���������
        while (this.transform.position != end)
        {
            yield return new WaitForFixedUpdate();
            end = GetLastTileInCoridor(
                tilemap.WorldToCell(this.transform.position),
                direction,
                tilemap);
            _tail.transform.position = this.transform.position = Vector3.MoveTowards(this.transform.position, end, moveSpeed);
        }
        Destroy(_tail, 1f);
        animator.SetBool("isMove", false);
        animator.speed = 1;
        isMove = false;
    }

    private IEnumerator PostMove(Direction direction)
    {
        isAutoMove = true;
        secondDirection = direction;
        // ��� ��������� ��������
        yield return new WaitWhile(() => isMove);
        // ��������� �������� �������� �������� � ��� �� ���������
        yield return StartCoroutine(Move(direction));
        secondDirection = Direction.zero;
        isAutoMove = false;
    }

    void Start()
    {
        animator = this.GetComponent<Animator>();
        wasTeleported = 0;
    }

    void FixedUpdate()
    {
        // �������� ������
        float deltaThouch = 0;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                // ����� �������
                case TouchPhase.Began:
                    startPos = touch.position;
                    directionChosen = false;
                    break;

                // ����� �������
                case TouchPhase.Moved:
                    directionChosen = true;
                    vDirection = touch.position - startPos;
                    deltaThouch = touch.deltaPosition.magnitude;
                    break;

                // ����� �����
                case TouchPhase.Ended:
                    directionChosen = false;
                    break;
            }
        }
        // ���� ������� �����������, �������� ������ ����������� � ����� ������ ������ �����������
        if (directionChosen && (deltaThouch > minSwipeSpeed) && (vDirection.magnitude > minVDirection))
        {
            var pi = Mathf.PI;
            var xAxis = Vector2.right;
            float dirAngle = (
                vDirection.y > 0 ?
                Mathf.Acos(Vector2.Dot(vDirection.normalized, xAxis)) :
                2 * pi - Mathf.Acos(Vector2.Dot(vDirection.normalized, xAxis))
                );          // ���� � ������ ���������� �� �������-���������
            // ��� � ����� �������, ����������� �������

            #region Direction determine
            var firstDirection = direction;     // ��������� ����������� ����������� ������

            // ���������� ����������� ������ �� ������ ����������
            if ((dirAngle < pi / 4) || (dirAngle >= 7 * pi / 4))
                direction = (Direction)1;
            else if ((dirAngle < 3 * pi / 4) && (dirAngle >= pi / 4))
                direction = (Direction)2;
            else if ((dirAngle < 5 * pi / 4) && (dirAngle >= 3 * pi / 4))
                direction = (Direction)3;
            else if ((dirAngle < 7 * pi / 4) && (dirAngle >= 5 * pi / 4))
                direction = (Direction)4;
            #endregion

            // ���� �� �������� ����� ��������, � ����������� ������ ������� �� ������� (������� ����� � Move())
            if (
                isMove && 
                (firstDirection != direction) && 
                (Vector2.Distance(this.transform.position, end) < minPostMoveDistance) && 
                ((int)direction % 2 != (int)firstDirection % 2)) 
            {
                if (secMovement != null)
                    StopCoroutine(secMovement);
                secMovement = StartCoroutine(PostMove(direction));
            }
            // ���� �� ��������� ������, �� �������� � ������� �����������
            else if (!isMove)
                movement = StartCoroutine(Move(direction));

            // ���������� ��������
            directionChosen = false;
            vDirection = Vector2.zero;
        }
        // ������ ���� ���������� ����� �������
        if(Input.touchCount > 0)
            startPos = Input.GetTouch(0).position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Trap"))
            {
                // ��� ������������ � ��������� - �����
                Dead();
            }
            if (collision.GetComponent<Portal>())
            {
                if (wasTeleported == 0)
                {
                    // ��� ������������ � ��������
                    // ������������� ��� �������� ��������
                    // ���������� � ����� ����� �������-��������
                    this.transform.position = collision.GetComponent<Portal>().twinkPortal.transform.position - Vector3.forward * 40;
                    this.wasTeleported = 2 - (int)Math.Truncate(moveSpeed);
                }
            }
            if (collision.GetComponent<Pusher>())
            {
                // ��� ������������ � ���������� ��������� ������ ��������
                if (secMovement != null)
                    StopCoroutine(secMovement);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.GetComponent<Pusher>())
            {
                if (!isMove && (Vector2.Distance(this.transform.position, collision.transform.position) < 1f))
                {
                    // ��� ��������� � ���� � ���������� �����
                    // ���� ������ � ������������, �� ����� � ��������������
                    if (mainDirection == collision.GetComponent<Pusher>().verticalEnter)
                    {
                        movement = StartCoroutine(Move((collision.GetComponent<Pusher>().horizontalEnter == Direction.right) ? Direction.left : Direction.right));
                    }
                    else if (mainDirection == collision.GetComponent<Pusher>().horizontalEnter)
                    {
                        movement = StartCoroutine(Move((collision.GetComponent<Pusher>().verticalEnter == Direction.up) ? Direction.down : Direction.up));
                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.GetComponent<Portal>())
            {
                if (wasTeleported == 1)
                {
                    wasTeleported = 0;
                }
                else if (wasTeleported == 2 - (int)Math.Truncate(moveSpeed))
                {
                    wasTeleported = 1;
                }
            }
        }
    }

    private Vector3Int GetLastTileInCoridor(Vector3 start, Direction direction, Tilemap tilemapScheme)
    {
        // ��������� ������� ������� ����� �������� �� ������
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
    private bool IsThisTileFromWay(Tilemap tilemapScheme, int i, int di, int j, int dj)
    {
        // ������� ��� ������� ��������� ����
        return controller.tilemap.GetComponent<Map>().wayTile == tilemapScheme.GetTile(new Vector3Int(i + di, j + dj, 0));
    }

    public static Vector3Int DirectionToVector(Direction direction)
    {
        switch((int)direction)
        {
            case 1:
                return Vector3Int.right;
            case 2:
                return Vector3Int.up;
            case 3:
                return Vector3Int.left;
            case 4:
                return Vector3Int.down;
            default:
                return Vector3Int.zero;
        }
    }

    public static Vector3 ToCellNormalazing(Tilemap tilemap, Vector3 pos)
    {
        return tilemap.CellToWorld(tilemap.WorldToCell(pos));
    }
}
