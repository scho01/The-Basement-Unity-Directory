using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            collision.gameObject.GetComponent<EnemyController>().Hit();
        else if (collision.CompareTag("Boss"))
            collision.gameObject.GetComponent<BossController>().Hit();
    }
}
