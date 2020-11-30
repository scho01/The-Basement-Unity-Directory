using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{
    public bool isPlayer;
    public int hitSpeed;
    float attackLength;
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        if (isPlayer)
            attackLength = 0.2f;
        else
            attackLength = 1.5f;
        StartCoroutine(DeathDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (!parent)
            Destroy(gameObject);
        transform.position = parent.transform.position;
        transform.Rotate(0, 0, hitSpeed * Time.deltaTime);
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(attackLength);
        if (!isPlayer)
            parent.GetComponent<EnemyController>().attackExists = false;
        else
            parent.GetComponent<PlayerController>().attackExists = true;
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isPlayer)
        {
            if (col.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<EnemyController>().Hit();
            }
        }
        else
        {
            if (col.CompareTag("Player"))
            {
                col.gameObject.GetComponent<PlayerController>().Hit();
            }
        }
    }
}
