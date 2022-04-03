using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSwiper : MonoBehaviour
{
    public GameObject swiper;
    public Camera spectrator;
    public Camera cameraController;

    GameObject vTest;
    void Update()
    {
        if ((Input.touchCount > 0))
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                // םאזאכ ןאכüצול
                case TouchPhase.Began:
                    vTest = Instantiate(swiper, spectrator.ScreenToWorldPoint(touch.position), Quaternion.identity);
                    break;

                // ןנמג¸כ ןאכüצול
                case TouchPhase.Moved:
                    if (vTest != null)
                        vTest.transform.position = spectrator.ScreenToWorldPoint(touch.position) + Vector3.forward * 15;
                    break;

                // ףבנאכ ןאכוצ
                case TouchPhase.Ended:
                    if (vTest != null)
                        Destroy(vTest);
                    break;
            }
        }
    }
}
