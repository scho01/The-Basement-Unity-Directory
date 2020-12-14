using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void Awake()
    {
        AudioController.instance.Play("PMiss");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            AudioController.instance.Play("PHit");
            collision.gameObject.GetComponent<EnemyController>().Hit();
        }
        else if (collision.CompareTag("Minotaur"))
        {
            AudioController.instance.Play("PHit");
            collision.gameObject.GetComponent<MinotaurController>().Hit();
        }
        else if (collision.CompareTag("Golem"))
        {
            AudioController.instance.Play("PHit");
            collision.gameObject.GetComponent<GolemController>().Hit();
        }
    }
}
