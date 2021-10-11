using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thicker : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float speed;
    public float pause;

    IEnumerator SlowDown()
    {
        var temp = speed;
        speed = 0;
        yield return new WaitForSeconds(pause);
        speed = temp;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, startPos) >= Vector3.Distance(startPos, endPos))
        {
            StartCoroutine(SlowDown());
            startPos = startPos + endPos;
            endPos = startPos - endPos;
            startPos = startPos - endPos;
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, endPos, speed/100);
        }
    }
}
