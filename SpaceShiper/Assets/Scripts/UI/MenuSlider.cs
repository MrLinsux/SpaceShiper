using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlider : MonoBehaviour
{
    public GameObject scrollbar;
    float scrollPos = 0f;
    float[] pos;

    void Update()
    {
        pos = new float[this.transform.childCount];
        float dist = 1f / (pos.Length - 1f);

        for(int i = 0; i < pos.Length; i++)
        {
            pos[i] = dist * i;
        }

        if (Input.touchCount > 0)
        {
            scrollPos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for(int i = 0; i < pos.Length; i++)
            {
                if ((scrollPos < pos[i] + dist / 2) && (scrollPos > pos[i] - (dist / 2))) 
                {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }
    }
}
