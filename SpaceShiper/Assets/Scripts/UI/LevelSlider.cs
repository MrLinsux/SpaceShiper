using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSlider : MonoBehaviour
{
    public class Page
    {
        public int W
        {
            get
            {
                return w;
            }
            set
            {
                if (value < 65)
                    i = 0;
                else if (value > 90)
                    i = 2;
                else
                    w = value;
            }
        }   // мир
        public int I 
        {
            get
            {
                return i;
            }
            set
            {
                if (value < 0)
                {
                    i = 2;
                    W--;
                }
                else if (value > 2)
                {
                    i = 0;
                    W++;
                }
                else
                {
                    i = value;
                }
            }
        }           // страница мира
        private int i; private int w;
        public Page(int w, int i){ W = w; I = i; }

        public static Page operator++(Page a)
        {
            a.I++;
            return a;
        }
        public static Page operator --(Page a)
        {
            a.I--;
            return a;
        }
        public static bool operator ==(Page a, Page b)
        {
            return ((a.W == b.W) && (a.I == b.I));
        }
        public static bool operator !=(Page a, Page b)
        {
            return ((a.W != b.W) || (a.I != b.I));
        }
    }

    public Page currentPage = new Page(65, 0);      // текущая страница

    private Vector2 start;
    public MotherController motherController;
    public Camera mainCamera;

    public float minVector = 10f;
    public float minY = 20f;

    void Start()
    {
        if (PlayerPrefs.HasKey("currentWorld"))
            currentPage = new Page(PlayerPrefs.GetInt("currentWorld"), PlayerPrefs.GetInt("currentIndex"));
        else
            currentPage = new Page(65, 0);
        motherController.LoadLevelSelector(currentPage);
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                start = mainCamera.ScreenToWorldPoint(touch.position);
            else if(touch.phase == TouchPhase.Ended)
            {
                if((Vector2.Distance(start, mainCamera.ScreenToWorldPoint(touch.position)) >= minVector) && (Mathf.Abs(mainCamera.ScreenToWorldPoint(touch.position).y - start.y) > minY))
                {
                    Page temp = new Page(currentPage.W, currentPage.I);
                    Vector2 end = mainCamera.ScreenToWorldPoint(touch.position);
                    if(end.y > start.y)
                    {
                        motherController.LoadLevelSelector(currentPage--);
                    }
                    else if(end.y < start.y)
                    {
                        motherController.LoadLevelSelector(currentPage++);
                    }
                    else
                        return;

                    if(temp != currentPage)
                        Vibration.Vibrate(70);
                }
            }    
        }
    }
}
