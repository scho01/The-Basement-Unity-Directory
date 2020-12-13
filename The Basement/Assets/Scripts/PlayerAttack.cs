using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            collision.gameObject.GetComponent<EnemyController>().Hit();
        else if (collision.CompareTag("Minotaur"))
            collision.gameObject.GetComponent<MinotaurController>().Hit();
        else if (collision.CompareTag("Golem"))
            collision.gameObject.GetComponent<GolemController>().Hit();
    }
}
