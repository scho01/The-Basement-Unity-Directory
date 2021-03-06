﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GolemController : MonoBehaviour
{
    GameObject player;
    private Rigidbody2D rb;
    private Animator anim;
    public BossState currentState = BossState.Idle;
    public float range = 5;
    public float speed = 1;
    public float health = 50;
    private float maxHealth;
    public GameObject[] itemPrefab;
    public float[] dropRates;
    private float lastAttack = 0f;
    public float attackInterval = 4f;
    public bool invulnerable = false;
    private bool playerInRange = false;
    private int attackNumber = 0;
    public float[] windUpTime;
    public float[] attackTime;
    public float hitTime;
    public float dieSoundTime;
    public float dieTime;
    public GameObject projectile;
    public GameObject ladderPrefab;
    private Vector3 startPosition;
    private SpriteRenderer sr;
    public bool vScreen = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        maxHealth = health;
        if (!vScreen)
            RoomController.instance.UpdateBossHealth("Golem", health, maxHealth);
        StartCoroutine(SetStartingPosition());
    }
    private IEnumerator SetStartingPosition()
    {
        yield return new WaitForSeconds(1f);
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        switch(currentState)
        {
            case (BossState.Idle):
                playerInRange = IsPlayerInRange(range);
                rb.velocity = Vector3.zero;
                Idle();
                break;
            case (BossState.Follow):
                if (!vScreen)
                {
                    if (!RoomController.instance.currRoom.bossRoom)
                        ResetPosition();
                    else
                        Attack();
                    break;
                }
                else
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
        attackNumber = Random.Range(1, 5);
        anim.SetBool("attack" + attackNumber, true);
        yield return null;
        anim.SetBool("attack" + attackNumber, false);
        if (attackNumber == 4)
        {
            yield return new WaitForSeconds(0.5f);
            invulnerable = true;
            yield return new WaitForSeconds(1.5f);
            if (!vScreen)
                invulnerable = false;
            yield return new WaitForSeconds(attackTime[3] - 2f);
        }
        else
        {
            if (attackNumber == 3)
            {
                GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
                proj.transform.parent = gameObject.transform;
            }
            yield return new WaitForSeconds(windUpTime[attackNumber - 1]);
            AudioController.instance.Play("GAtk" + attackNumber);
            yield return new WaitForSeconds(attackTime[attackNumber - 1] - windUpTime[attackNumber - 1]);
        }
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
        sr.color = new Color(0.5f, 0.5f, 0.5f);
        RoomController.instance.UpdateBossHealth("Golem", health, maxHealth);
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
            currentState = BossState.Follow;
        }
        else
            yield return new WaitForSeconds(hitTime);
        sr.color = new Color(1f, 1f, 1f);
        if (!vScreen)
            invulnerable = false;
    }

    public void Hit()
    {
        if (!invulnerable)
        {
            health -= PlayerController.playerStats[2];
            if (health <= 0)
                StartCoroutine(Die());
            else
                StartCoroutine(Hitstun());
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        if (health > 0)
        {
            anim.SetBool("attack1", false);
            anim.SetBool("attack2", false);
            anim.SetBool("attack3", false);
            anim.SetBool("attack4", false);
            anim.SetBool("damage", false);
            anim.SetBool("walk", false);
            if (transform.childCount > 1)
                transform.GetChild(0).gameObject.SetActive(false);
            if (!vScreen)
                invulnerable = false;
            currentState = BossState.Idle;
            lastAttack = Time.time;
        }
        else
            StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        invulnerable = true;
        currentState = BossState.Die;
        RoomController.instance.UpdateBossHealth("Golem", 0, maxHealth);
        anim.SetBool("die", true);
        yield return null;
        anim.SetBool("die", false);
        yield return new WaitForSeconds(dieTime);
        RoomController.instance.bossUI.SetActive(false);
        RoomController.instance.bossDead = true;
        float drop = Random.Range(0f, 1f);
        for (int i = 0; i < dropRates.Length; i++)
        {
            if (drop <= dropRates[i])
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
