using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    Idle,
    Wander,
    Follow,
    Attack,
    Hit,
    Die
};

public class EnemyController : MonoBehaviour
{
    GameObject player;
    private Rigidbody2D rb;
    private Animator anim;
    public EnemyState currentState = EnemyState.Wander;
    public float range = 5;
    public float speed = 1;
    private bool chooseDir = false;
    private Vector3 randomDir;
    private Vector3 nextPos;
    public float health = 5;
    public GameObject itemPrefab;
    private float lastAttack;
    public float attackInterval = 5f;
    private float xMin = -7.8f;
    private float xMax = 7.8f;
    private float yMin = -3.7f;
    private float yMax = 4.4f;
    private bool invulnerable = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (IsPlayerInRange(range) && currentState != EnemyState.Die && currentState != EnemyState.Hit && currentState != EnemyState.Attack)
        {
            if (currentState == EnemyState.Idle)
                anim.SetBool("walk", true);
            currentState = EnemyState.Follow;
        }
        else if (!IsPlayerInRange(range) && currentState != EnemyState.Die && currentState != EnemyState.Hit && currentState != EnemyState.Wander)
        {
            anim.SetBool("walk", false);
            anim.SetBool("attack", false);
            currentState = EnemyState.Idle;
        }
        switch(currentState)
        {
            case (EnemyState.Idle):
                rb.velocity = Vector3.zero;
                Idle();
                break;
            case (EnemyState.Wander):
                Wander();
                break;
            case (EnemyState.Follow):
                Attack();
                break;
            case (EnemyState.Attack):
                break;
            case (EnemyState.Hit):
                break;
            case (EnemyState.Die):
                Die();
                break;
        }
    }

    private void ChangeDirection(Vector3 pos)
    {
        if (pos.x - transform.position.x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.identity;
    }

    private IEnumerator ChooseDirection()
    {
        anim.SetBool("walk", false);
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDir = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
        //randomDir = new Vector3(0, 0, Random.Range(0, 360));
        //Quaternion nextRotation = Quaternion.Euler(randomDir);
        //transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Random.Range(0.5f, 3f));
        ChangeDirection(randomDir);
        currentState = EnemyState.Wander;
        chooseDir = false;
        anim.SetBool("walk", true);
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Idle()
    {
        if (!chooseDir)
        {
            StartCoroutine(ChooseDirection());
        }
    }

    void Wander()
    {
        if (IsPlayerInRange(range))
        {
            lastAttack = Time.time;
            currentState = EnemyState.Follow;
        }
        nextPos = randomDir - transform.position;
        if (nextPos.x > 0.3f || nextPos.y > 0.3f)
            rb.MovePosition(transform.position + nextPos.normalized * speed * Time.deltaTime);
        else
        {
            currentState = EnemyState.Idle;
            chooseDir = false;
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
    IEnumerator Attacking()
    {
        currentState = EnemyState.Attack;
        lastAttack = Time.time;
        anim.SetBool("attack", true);
        yield return null;
        anim.SetBool("attack", false);
        yield return new WaitForSeconds(1.35f);
        currentState = EnemyState.Follow;
    }

    void Attack()
    {
        if (currentState != EnemyState.Attack)
        {
            ChangeDirection(player.transform.position);
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
        if (Time.time > lastAttack + attackInterval)
            StartCoroutine(Attacking());
    }

    private IEnumerator Hitstun()
    {
        invulnerable = true;
        currentState = EnemyState.Hit;
        anim.SetBool("damage", true);
        float pspeed = speed;
        speed = 0;
        yield return null;
        anim.SetBool("damage", false);
        yield return new WaitForSeconds(0.617f);
        speed = pspeed;
        invulnerable = false;
        currentState = EnemyState.Follow;
    }

    public void Hit()
    {
        if (!invulnerable)
        {
            health -= PlayerController.attackDamage;
            if (health <= 0)
            {
                currentState = EnemyState.Die;
                Die();
            }
            StartCoroutine(Hitstun());
        }
    }

    void Die()
    {
        Instantiate(itemPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }
}
