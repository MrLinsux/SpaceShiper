using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlider : MonoBehaviour
{
    public GameObject scrollbar;
    float scrollPos = 0f;               // текущее положение скролла
    float[] pos;

    void Update()
    {
        pos = new float[this.transform.childCount];     // позиции каждого варианта
        float dist = 1f / (pos.Length - 1f);            // расстояние между вариантами

        for(int i = 0; i < pos.Length; i++)
        {
            pos[i] = dist * i;
        }

        if (Input.touchCount > 0)
        {
            // если палец на экране, то это обычный скролл
            scrollPos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            // если пальца на экране нет, то движемся к ближайшему варианту
            for(int i = 0; i < pos.Length; i++)
            {
                if ((scrollPos < pos[i] + (dist / 2)) && (scrollPos > pos[i] - (dist / 2))) 
                {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }
    }
}
