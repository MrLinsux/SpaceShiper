using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public GameObject trailSegment;

    private float dTimeSpawn;
    public float startTimeSpawn;
    public float segmentLifeTime;

    void FixedUpdate()
    {
        if (this.GetComponent<Player>().isMove)
        {
            var trail = Instantiate(trailSegment, this.transform.position, this.transform.rotation);
            Destroy(trail, segmentLifeTime);
            dTimeSpawn = startTimeSpawn;
        }
    }
}
