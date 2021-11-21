using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;        // скорость движения игрока
    public float minSwipeSpeed = 1f;    // минимальная скорость свайпа пальцеп для надала движения
    public float minVDirection = 1f;    // минимальная длина свайпа для начала движения
    public int minDistanceForMR = 5;

    public bool directionChosen;        // выбран ли вектор движения 
    public bool isMove = false;         // находится ли игрок в движении
    private Vector3 end;                // точка у которой движется игрок
    public Direction direction = Direction.zero;            // текущее направление
    public Direction mainDirection = Direction.zero;        // основное направление
    public Direction secondDirection = Direction.zero;      // направление в Памяти поврота
    private Coroutine movement;                             // переменная для корутины Move()
    private Vector2 startPos;                               // точка начала вектора свойпа
    private Vector2 vDirection;                             // вектор свайпа
    private int wasTeleported;

    public GameObject flash;                                // вспышка в начале движения
    public GameController controller;                // игровой контроллер
    public Tilemap tilemap;                                 // объект Map
    private Animator animator;                              // аниматор игрока

    public enum Direction { zero, right, up, left, down }   // все возможные направления движения

    public void Dead()
    {
        // функция смерти игрока
        //Debug.Log("Is Dead");
    }

    private IEnumerator Move(Direction direction)
    {
        // функция основного движения
        Debug.Log("11");
        mainDirection = direction;
        end = GetLastTileInCoridor(
            tilemap.WorldToCell(this.transform.position),
            direction,
            tilemap);           // точка, к которой летит игрок
        // на тот случай, если уже в нужной клетке стоим
        if (this.transform.position == end)
        {
            this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)direction);        // поворот в направлении движения
            yield break;
        }

        Debug.Log("12");
        var dist = (int)Vector2.Distance(this.transform.position, end);
        isMove = true;
        animator.SetBool("isMove", true);
        animator.SetInteger("Dist", dist);

        Debug.Log("13 - " + dist);
        if (dist == 1)
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Step"));
        else if (dist == 2)
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"));
        else if (dist >= 3)
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Move"));
        else
        {
            Debug.LogError("Distantion is less or equals 0");
            animator.SetBool("isMove", false);
            isMove = false;
            yield break;
        }
        this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)mainDirection);        // поворот в направлении движения
        if (dist > 1)
            Instantiate(flash, this.transform.position, Quaternion.identity);

        Debug.Log("14");
        while (this.transform.position != end)
        {
            yield return new WaitForFixedUpdate();
            end = GetLastTileInCoridor(
                tilemap.WorldToCell(this.transform.position),
                direction,
                tilemap);
            this.transform.position = Vector3.MoveTowards(this.transform.position, end, moveSpeed);
        }
        Debug.Log("15");
        this.transform.position = end;                                              // нормализация положения
        animator.SetInteger("Dist", 0);
        animator.SetBool("isMove", false);
        isMove = false;
        if (secondDirection != Direction.zero)
        {
            movement = StartCoroutine(Move(secondDirection));
            Debug.Log("16");
            secondDirection = Direction.zero;
        }
        Debug.Log("17");
    }

    void Start()
    {
        animator = this.GetComponent<Animator>();
        wasTeleported = 0;
    }

    void FixedUpdate()
    {
        // скорость свайпа
        float deltaThouch = 0;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
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
        // если выбрано направление, скорость больше минимальной и длина свайпа больше минимальной
        if (directionChosen && (deltaThouch > minSwipeSpeed) && (vDirection.magnitude > minVDirection))
        {
            var xAxis = Vector2.right;
            float dirAngle = (
                vDirection.y > 0 ?
                Mathf.Acos(Vector2.Dot(vDirection.normalized, xAxis)) :
                2 * Mathf.PI - Mathf.Acos(Vector2.Dot(vDirection.normalized, xAxis))
                );          // угол в колесе управления из концепт-документа
            // как и любой натурал, использовал радианы

            #region Direction determine
            var firstDirection = direction;     // сохранили направление предыдущего свайпа

            // определяем направление свайпа по колесу управления

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                var step = Mathf.PI / 4;
                if ((dirAngle < step) || (dirAngle >= 7 * step))
                    direction = (Direction)1;
                else if ((dirAngle < 3 * step) && (dirAngle >= step))
                    direction = (Direction)2;
                else if ((dirAngle < 5 * step) && (dirAngle >= 3 * step))
                    direction = (Direction)3;
                else if ((dirAngle < 7 * step) && (dirAngle >= 5 * step))
                    direction = (Direction)4;
            }
            else
            {
                var step = Mathf.PI / 8;
                if ((int)mainDirection % 2 == 1)
                {
                    if ((dirAngle < step) || (dirAngle >= 15 * step))
                        direction = (Direction)1;
                    else if ((dirAngle < 7 * step) && (dirAngle >= step))
                        direction = (Direction)2;
                    else if ((dirAngle < 9 * step) && (dirAngle >= 7 * step))
                        direction = (Direction)3;
                    else if ((dirAngle < 15 * step) && (dirAngle >= 9 * step))
                        direction = (Direction)4;
                }
                else
                {
                    if ((dirAngle < 3 * step) || (dirAngle >= 13 * step))
                        direction = (Direction)1;
                    else if ((dirAngle < 5 * step) && (dirAngle >= 3*step))
                        direction = (Direction)2;
                    else if ((dirAngle < 11 * step) && (dirAngle >= 5 * step))
                        direction = (Direction)3;
                    else if ((dirAngle < 13 * step) && (dirAngle >= 11 * step))
                        direction = (Direction)4;
                }
            }
            #endregion

            // если не запущена Памят Поворота, п направление свайпа отлично от старого (который попал в Move())
            if (
                isMove &&
                (Vector2.Distance(this.transform.position, end) <= minDistanceForMR)
                )
            {
                secondDirection = direction;
            }
            // если не двигаемся вообще, то начинаем в сторону направления
            else if (
                (
                animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
                )
                && !isMove
                && secondDirection == Direction.zero
                )
                movement = StartCoroutine(Move(direction));

            // сбрасываем значения
            directionChosen = false;
            vDirection = Vector2.zero;
        }
        // всегда надо сбрасывать точку нажатия
        if(Input.touchCount > 0)
            startPos = Input.GetTouch(0).position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Trap"))
            {
                // при столкновении с ловушками - смерт
                Dead();
            }
            if (collision.GetComponent<Portal>())
            {
                if (wasTeleported == 0)
                {
                    // при столкновении с порталом
                    // останавливаем все корутины движения
                    // перемещаем в точку около портала-близнеца
                    this.transform.position = collision.GetComponent<Portal>().twinkPortal.transform.position - Vector3.forward * 40;
                    this.wasTeleported = 2 - (int)Math.Truncate(moveSpeed);
                }
            }
            if (collision.GetComponent<Pusher>())
            {
                // при столкновении с толкателем отключаем Память Поворота
                secondDirection = Direction.zero;
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
                    // ждём попадания в одну с толкателем точку
                    // если попали в вертикальную, то летим в горизонтальную
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
        // вычисляем находим крайнюю точку движения по дороге
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
        // функция для лучшего понимания кода
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
