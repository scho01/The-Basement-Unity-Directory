using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool boss = false;
    public bool projectile;
    public float duration;

    IEnumerator Projectile()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void Start()
    {
        if (projectile)
            StartCoroutine(Projectile());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitBox"))
            collision.gameObject.GetComponentInParent<PlayerController>().Hit(boss);
    }
}
