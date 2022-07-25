using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSlider : MonoBehaviour
{
    private Vector2 start;
    public MotherController motherController;
    public Camera mainCamera;


    public float minVector = 10f;
    public float minY = 20f;

    public int currentPage = 0;

    void Start()
    {
        motherController.LoadSkinSelector(0);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                start = mainCamera.ScreenToWorldPoint(touch.position);
            else if (touch.phase == TouchPhase.Ended)
            {
                if ((Vector2.Distance(start, mainCamera.ScreenToWorldPoint(touch.position)) >= minVector) && (Mathf.Abs(mainCamera.ScreenToWorldPoint(touch.position).y - start.y) > minY))
                {
                    var temp = currentPage;
                    Vector2 end = mainCamera.ScreenToWorldPoint(touch.position);
                    if (end.y > start.y)
                    {
                        motherController.LoadSkinSelector(currentPage--);
                    }
                    else if (end.y < start.y)
                    {
                        motherController.LoadSkinSelector(currentPage++);
                    }
                    else
                        return;

                    if (temp != currentPage)
                        Vibration.Vibrate(70);
                }
            }
        }
    }
}
