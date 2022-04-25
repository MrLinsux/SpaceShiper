using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    Player player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out player))
            player.Dead();
    }
}
