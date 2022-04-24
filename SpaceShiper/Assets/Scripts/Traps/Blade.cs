using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    Player player;
    Animator anim;
    public float delay;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out player))
        {
            anim.SetBool("Triggering", true);
            StartCoroutine(ChooseFate());
        }
    }

    private IEnumerator ChooseFate()
    {
        yield return new WaitForSeconds(delay);
        if(this.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<PolygonCollider2D>()))
        {
            anim.SetInteger("Fate", 2);
            player.enabled = false;
            player.GetComponent<Animator>().enabled = false;
            player.SetStandartSprite();
            yield return new WaitWhile(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Kill"));
            player.Dead();
        }    
        else
        {
            anim.SetInteger("Fate", 1);
        }
        anim.SetBool("Triggering", false);
    }
}
