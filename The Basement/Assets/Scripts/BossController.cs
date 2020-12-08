using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BossState
{
    Idle,
    Follow,
    Attack,
    Hit,
    Die
};

public class BossController : MonoBehaviour
{
    GameObject player;
    private Rigidbody2D rb;
    private Animator anim;
    public BossState currentState = BossState.Idle;
    public float range = 5;
    public float speed = 2;
    public float health = 50;
    public GameObject itemPrefab;
    private float lastAttack = 0f;
    public float attackInterval = 3.5f;
    private bool invulnerable = false;
    private bool playerInRange = false;
    private int attackNumber = 0;
    public GameObject ladderPrefab;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        playerInRange = IsPlayerInRange(range);
        switch(currentState)
        {
            case (BossState.Idle):
                rb.velocity = Vector3.zero;
                Idle();
                break;
            case (BossState.Follow):
                Attack();
                break;
            case (BossState.Attack):
                break;
            case (BossState.Hit):
                break;
            case (BossState.Die):
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

    private bool IsPlayerInRange(float range)
    {
        if (player == null)
            return false;
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    void Idle()
    {
        if (playerInRange)
        {
            lastAttack = Time.time;
            anim.SetBool("walk", true);
            currentState = BossState.Follow;
            return;
        }
    }

    IEnumerator Attacking()
    {
        currentState = BossState.Attack;
        lastAttack = Time.time;
        attackNumber = Random.Range(1, 3);
        anim.SetBool("attack" + attackNumber, true);
        yield return null;
        anim.SetBool("attack" + attackNumber, false);
        if (attackNumber == 1)
            yield return new WaitForSeconds(1.85f);
        else
            yield return new WaitForSeconds(2.017f);
        currentState = BossState.Follow;
    }

    void Attack()
    {
        if (currentState != BossState.Attack)
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
        if (currentState != BossState.Attack)
        {
            currentState = BossState.Hit;
            anim.SetBool("damage", true);
            float pspeed = speed;
            speed = 0;
            yield return null;
            anim.SetBool("damage", false);
            yield return new WaitForSeconds(0.267f);
            speed = pspeed;
            invulnerable = false;
            currentState = BossState.Follow;
        }
        else
        {
            yield return new WaitForSeconds(0.267f);
            invulnerable = false;
        }
    }

    public void Hit()
    {
        if (!invulnerable)
        {
            health -= PlayerController.attackDamage;
            if (health <= 0)
                StartCoroutine(Die());
            else
                StartCoroutine(Hitstun());
        }
    }

    private IEnumerator Die()
    {
        invulnerable = true;
        currentState = BossState.Die;
        anim.SetBool("die", true);
        yield return null;
        anim.SetBool("die", false);
        yield return new WaitForSeconds(1.017f);
        invulnerable = false;
        Instantiate(itemPrefab, transform.position, Quaternion.identity);
        Instantiate(ladderPrefab, transform.parent.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
