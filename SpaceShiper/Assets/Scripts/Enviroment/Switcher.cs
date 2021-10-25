using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcher : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (this.transform.parent.GetComponent<Door>())
                this.transform.parent.GetComponent<Door>().IsOpen = !this.transform.parent.GetComponent<Door>().IsOpen;
            else if (this.transform.parent.GetComponent<TimeDoor>())
                this.transform.parent.GetComponent<TimeDoor>().Open();
        }
    }
}
