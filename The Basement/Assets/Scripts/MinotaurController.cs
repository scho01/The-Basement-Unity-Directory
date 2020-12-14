using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MinotaurController : MonoBehaviour
{
    GameObject player;
    private Rigidbody2D rb;
    public Animator anim;
    public BossState currentState = BossState.Idle;
    public float range = 5;
    public float speed = 2;
    public float health = 30;
    private float maxHealth;
    public GameObject[] itemPrefab;
    public float[] dropRates;
    private float lastAttack = 0f;
    public float attackInterval = 3.5f;
    public bool invulnerable = false;
    private bool playerInRange;
    private int attackNumber = 0;
    public float[] windUpTime;
    public float[] attackTime;
    public float hitTime;
    public float dieSoundTime;
    public float dieTime;
    public GameObject projectile;
    public GameObject ladderPrefab;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        maxHealth = health;
        RoomController.instance.UpdateBossHealth("Minotaur", health, maxHealth);
    }

    void FixedUpdate()
    {
        playerInRange = IsPlayerInRange(range);
        switch (currentState)
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
        yield return new WaitForSeconds(windUpTime[attackNumber - 1]);
        AudioController.instance.Play("MAtk" + attackNumber);
        yield return new WaitForSeconds(attackTime[attackNumber - 1] - windUpTime[attackNumber - 1]);
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
        RoomController.instance.UpdateBossHealth("Minotaur", health, maxHealth);
        if (currentState != BossState.Attack)
        {
            currentState = BossState.Hit;
            anim.SetBool("damage", true);
            float pspeed = speed;
            speed = 0;
            yield return null;
            anim.SetBool("damage", false);
            yield return new WaitForSeconds(hitTime);
            speed = pspeed;
            invulnerable = false;
            currentState = BossState.Follow;
        }
        else
        {
            yield return new WaitForSeconds(hitTime);
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
        RoomController.instance.UpdateBossHealth("Minotaur", 0, maxHealth);
        anim.SetBool("die", true);
        yield return null;
        anim.SetBool("die", false);
        yield return new WaitForSeconds(dieSoundTime);
        AudioController.instance.Play("MDie");
        yield return new WaitForSeconds(dieTime - dieSoundTime);
        RoomController.instance.bossUI.SetActive(false);
        RoomController.instance.bossDead = true;
        float drop = Random.Range(0f, 1f);
        for (int i = 0; i < dropRates.Length; i++)
        {
            if (drop < dropRates[i])
            {
                Instantiate(itemPrefab[i], (transform.position + transform.position + player.transform.position) / 3, Quaternion.identity);
                break;
            }
        }
        Instantiate(ladderPrefab, transform.parent.position, Quaternion.identity);
        invulnerable = false;
        Destroy(gameObject);
    }
}
