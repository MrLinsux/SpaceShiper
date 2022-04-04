using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public AudioClip dashSound;                             // звук начала движения
    public float moveSpeed = 1f;                            // скорость движения игрока
    public float minVDirection = 1f;                        // минимальная длина свайпа для начала движения
    public int minDistanceForMR = 5;

    public bool directionChosen;                            // выбран ли вектор движения 
    public bool isMove = false;                             // находится ли игрок в движении
    public bool onStartDelay = true;
    public bool onStep1Delay = true;
    public bool onDeath = true;
    private Vector3 end;                                    // точка у которой движется игрок
    public Direction direction = Direction.zero;            // текущее направление
    public Direction mainDirection = Direction.zero;        // основное направление
    public Direction secondDirection = Direction.zero;      // направление в Памяти поврота
    public Coroutine movement;                              // переменная для корутины Move()
    private Vector2 startPos;                               // точка начала вектора свойпа
    private Vector2 vDirection;                             // вектор свайпа
    private int wasTeleported;

    public GameObject restartPanel;
    public AudioSource soundController;
    public LineRenderer vDirectionLine;
    public GameController gameController;                   // игровой контроллер
    public UIController uIController;
    public Camera cameraController;
    public Camera spectrator;
    public Tilemap tilemap;                                 // объект Map
    public Animator animator;                               // аниматор игрока
    public GameObject pGrave;
    public GameObject pGraveCircle;
    public GameObject landing;                              // след от приземления

    public enum Direction { zero, right, up, left, down }   // все возможные направления движения
    public Vector2[] dirToVector2 = new Vector2[5] { Vector2.zero, Vector2.right, Vector2.up, Vector2.left, Vector2.down };

    public void Dead()
    {
        // функция смерти игрока
        if (onDeath)
        {
            if (mainDirection != Direction.zero)
            {
                Destroy(Instantiate(pGrave, this.transform.position, Quaternion.Euler(0, 0, 90 * (int)mainDirection)), 5f);
            }
            else
            {
                var grave = Instantiate(pGrave, this.transform.position, Quaternion.Euler(0, 0, 90 * (int)mainDirection)).GetComponent<ParticleSystem>();
                var _shape = grave.shape;
                _shape.shapeType = ParticleSystemShapeType.Circle;
                Destroy(grave, 5f);
            }
            Vibration.Vibrate(90);
            animator.SetInteger("Dist", 0);
            animator.SetBool("isMove", false);
            this.GetComponent<Animator>().Play("Idle");
            this.GetComponent<SpriteRenderer>().flipX = false;
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
            isMove = false;
            mainDirection = Direction.zero;
            this.transform.eulerAngles = Vector3.zero;
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = null;
            }
            Invoke("OnRestartPanel", 2f);
            this.gameObject.SetActive(false);

        }
    }

    public void OffPlayer()
    {
        if(movement != null) StopCoroutine(movement);
        movement = null;
        this.transform.rotation.eulerAngles.Set(0, 0, 0);
        isMove = false;
    }

    public void OnRestartPanel()
    {
        restartPanel.SetActive(true);
    }

    private IEnumerator MovementChecker()
    {
        // не позволяет игроку застрять, пркращая его движение, если он не движется при isMove
        var firstPos = this.transform.position;
        yield return new WaitForSeconds(1.5f);
        if(this.transform.position == firstPos)
        {
            animator.SetInteger("Dist", 0);
            animator.SetBool("isMove", false);
            isMove = false;
            mainDirection = Direction.zero;
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = null;
            }
        }
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
            if (this.transform.eulerAngles != new Vector3(0, 0, 90 * (int)mainDirection))
            {
                // TODO: след от призимления, несмотря на отсутствие перемещения
                this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)mainDirection);        // поворот в направлении движения
                Destroy(Instantiate(landing, this.transform.position + (Vector3)dirToVector2[(int)direction], this.transform.rotation), 2f);
                animator.SetInteger("Dist", 0);
                yield return new WaitForFixedUpdate();
            }
            else
            {
                animator.SetInteger("Dist", 0);
            }
            animator.SetBool("isMove", false);
            isMove = false;
            mainDirection = Direction.zero;
            movement = null;
            yield break;
        }
        StartCoroutine(MovementChecker());

        Debug.Log("12 - " + end);
        var dist = (int)Vector2.Distance(this.transform.position, end);
        isMove = true;
        animator.SetBool("isMove", true);
        animator.SetInteger("Dist", dist);
        soundController.PlayOneShot(dashSound);

        Debug.Log("13 - " + dist);
        if (dist == 1)
        {
            if (onStep1Delay)
                yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Step 2"));
            this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)mainDirection);        // поворот в направлении движения
        }
        else if (dist > 1)
        {
            this.transform.eulerAngles = new Vector3(0, 0, 90 * (int)mainDirection);        // поворот в направлении движения
            if(onStartDelay)
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Move"));
        }
        else
        {
            Debug.LogError("Distantion is less or equals 0");
            animator.SetBool("isMove", false);
            isMove = false;
            yield break;
        }

        Debug.Log("14");
        while (this.transform.position != end)
        {
            yield return new WaitForFixedUpdate();
            end = GetLastTileInCoridor(
                tilemap.WorldToCell(this.transform.position),
                direction,
                tilemap);
            this.transform.position = Vector3.MoveTowards(this.transform.position, end, moveSpeed*Time.fixedDeltaTime);
        }
        this.transform.position = end;
        Destroy(Instantiate(landing, this.transform.position + (Vector3)dirToVector2[(int)direction], this.transform.rotation), 2f);
        if (secondDirection != Direction.zero)
        {
            movement = StartCoroutine(Move(secondDirection));
            Debug.Log("15 - 1");
            secondDirection = Direction.zero;
        }
        else
        {
            Debug.Log("15 - 2");                        // нормализация положения
            animator.SetInteger("Dist", 0);
            animator.SetBool("isMove", false);
            isMove = false;
            mainDirection = Direction.zero;
            movement = null;
        }
        Debug.Log("16");
        Vibration.Vibrate(30);
    }

    void Start()
    {
        animator = this.GetComponent<Animator>();
        wasTeleported = 0;
    }

    bool moreOneThouch = false;
    void FixedUpdate()
    {
        if (Input.touchCount == 0)
            moreOneThouch = false;

        if ((Input.touchCount > 0) && !moreOneThouch)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                // нажал пальцем
                case TouchPhase.Began:
                    startPos = cameraController.ScreenToWorldPoint(touch.position);
                    directionChosen = false;
                    break;

                // провёл пальцем
                case TouchPhase.Moved:
                    directionChosen = true;
                    vDirection = (Vector2)cameraController.ScreenToWorldPoint(touch.position) - startPos;
                    vDirectionLine.positionCount = 2;
                    vDirectionLine.SetPositions(new Vector3[] { spectrator.transform.position, vDirection + (Vector2)spectrator.transform.position });
                    break;

                // убрал палец
                case TouchPhase.Ended:
                    directionChosen = false;
                    vDirection = Vector2.zero;
                    vDirectionLine.positionCount = 0;
                    if (Input.touchCount > 1)
                        moreOneThouch = true;
                    break;
            }
        }
        // если выбрано направление, скорость больше минимальной и длина свайпа больше минимальной
        if (directionChosen && (vDirection.magnitude > minVDirection))
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

            var step = Mathf.PI / 4;
            if ((dirAngle < step) || (dirAngle >= 7 * step))
                direction = (Direction)1;
            else if ((dirAngle < 3 * step) && (dirAngle >= step))
                direction = (Direction)2;
            else if ((dirAngle < 5 * step) && (dirAngle >= 3 * step))
                direction = (Direction)3;
            else if ((dirAngle < 7 * step) && (dirAngle >= 5 * step))
                direction = (Direction)4;
            #endregion

            // если не запущена Памят Поворота, п направление свайпа отлично от старого (который попал в Move())
            if (
                isMove &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("End") &&
                ((int)mainDirection % 2 != (int)direction % 2) &&
                (Vector2.Distance(this.transform.position, end) <= minDistanceForMR)
                )
            {
                secondDirection = direction;
                startPos = cameraController.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            // если не двигаемся вообще, то начинаем в сторону направления
            else if (!isMove)
            {
                if (movement != null)
                    StopCoroutine(movement);
                movement = StartCoroutine(Move(direction));
                startPos = cameraController.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            // сбрасываем значения
            directionChosen = false;
            vDirection = Vector2.zero;
        }
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
        return 
            (gameController.tilemap.GetComponent<Map>().wayTile == tilemapScheme.GetTile(new Vector3Int(i + di, j + dj, 0))) || 
            (gameController.tilemap.GetComponent<Map>().coinTile == tilemapScheme.GetTile(new Vector3Int(i + di, j + dj, 0)));
    }
}
