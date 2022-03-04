using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrigger : MonoBehaviour
{
    public float waitForCharging = 1.5f;
    public float waitForActing = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.CompareTag("Player"))
            {
                StartCoroutine(Charge());
            }
        }
    }

    private IEnumerator Charge()
    {
        yield return new WaitForSeconds(waitForCharging);
        this.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(waitForActing);
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Start()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
