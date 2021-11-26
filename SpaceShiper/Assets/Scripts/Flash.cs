using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
