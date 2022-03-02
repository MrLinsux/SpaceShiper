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
                    w = 90;
                else if (value > 90)
                    w = 65;
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
                    W--;
                    i = 2;
                }
                else if (value > 2)
                {
                    W++;
                    i = 0;
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
    }

    public Page currentPage = new Page(65, 0);      // текущая страница
    public Page activeLevelPage = new Page(66, 1);  // страница, где находится активный уровень
    public int activeLevel = 8;                     // номер активного уровня

    private Vector2 start;
    public MotherController motherController;

    public float minVector = 10f;
    public float minY = 20f;

    void Start()
    {
        motherController.LoadLevelSelector(currentPage, activeLevelPage, activeLevel);
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                start = touch.position;
            else if(touch.phase == TouchPhase.Ended)
            {
                if((Vector2.Distance(start, touch.position) >= minVector) && (Mathf.Abs(touch.position.y - start.y) > minY))
                {
                    Vector2 end = touch.position;
                    if(end.y > start.y)
                    {
                        currentPage--;
                        motherController.LoadLevelSelector(currentPage, activeLevelPage, activeLevel);
                        Handheld.Vibrate();
                    }
                    else if(end.y < start.y)
                    {
                        currentPage++;
                        motherController.LoadLevelSelector(currentPage, activeLevelPage, activeLevel);
                        Handheld.Vibrate();
                    }
                    else
                        return;
                }
            }    
        }
    }
}
