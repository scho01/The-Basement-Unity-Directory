using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    Wander,
    Attack,
    Hit,
    Die
};

public class EnemyController : MonoBehaviour
{
    GameObject player;
    CircleCollider2D cc;
    public EnemyState currentState = EnemyState.Wander;
    public float range = 5;
    public float speed = 1;
    private bool chooseDir = false;
    private bool dead = false;
    private Vector3 randomDir;
    public int health = 5;
    public GameObject itemPrefab;
    public GameObject attackPrefab;
    private float lastAttack;
    public float attackInterval = 5f;
    GameObject attack;
    public bool attackExists = false;
    private float mapSizeX = 8f;
    private float mapSizeY = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cc = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInRange(range) && currentState != EnemyState.Die && currentState != EnemyState.Hit)
        {
            currentState = EnemyState.Attack;
        }
        else if (!IsPlayerInRange(range) && currentState != EnemyState.Die && currentState != EnemyState.Hit)
        {
            currentState = EnemyState.Wander;
        }
        switch(currentState)
        {
            case (EnemyState.Wander):
                Wander();
                break;
            case (EnemyState.Attack):
                Attack();
                break;
            case (EnemyState.Hit):
                break;
            case (EnemyState.Die):
                Die();
                break;
        }
    }

    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDir = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 3f));
        chooseDir = false;
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Wander()
    {

        if (!chooseDir)
        {
            StartCoroutine(ChooseDirection());
        }
        transform.position += -transform.right * speed * Time.deltaTime;
        if(IsPlayerInRange(range))
        {
            currentState = EnemyState.Attack;
            lastAttack = Time.time;
        }
    }

    /*
    Quaternion AttackDirection()
    {
        Vector2 direction = player.transform.position - this.transform.position;
        this.transform.right = direction;
        Vector3 upward = new Vector3(0, 0, 1);
        Quaternion rotation = Quaternion.LookRotation(this.transform.forward, upward);
        return rotation;
    }
*/
    void Attack()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        Vector2 direction = player.transform.position - transform.position;
        transform.right = direction;
        if (Time.time > lastAttack + attackInterval)
        {
            attack = Instantiate(attackPrefab, transform.position, transform.rotation) as GameObject;
            attack.GetComponent<StickController>().parent = gameObject;
            attackExists = true;
            lastAttack = Time.time;
        }
    }

    private IEnumerator Hitstun()
    {
        cc.enabled = false;
        currentState = EnemyState.Hit;
        float pspeed = speed;
        speed = 0;
        yield return new WaitForSeconds(0.2f);
        speed = pspeed;
        currentState = EnemyState.Attack;
        cc.enabled = true;
    }

    public void Hit()
    {
        health -= PlayerController.atkStat;
        if (health <= 0)
        {
            currentState = EnemyState.Die;
            Die();
        }
        StartCoroutine(Hitstun());
    }

    void Die()
    {
        if (attackExists)
            Destroy(attack);
        Instantiate(itemPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
}
