using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        player.transform.position = this.transform.position;
        StartCoroutine(WaitStart());
    }

    private IEnumerator WaitStart()
    {
        player.GetComponent<Player>().enabled = false;

        while(this.transform.GetChild(0).localPosition.y != -1)
            yield return new WaitForEndOfFrame();

        player.GetComponent<Player>().enabled = true;
    }
}
