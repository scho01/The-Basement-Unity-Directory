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
    public string enemyInitial;
    GameObject player;
    private Rigidbody2D rb;
    public Animator anim;
    public EnemyState currentState = EnemyState.Wander;
    public float range = 5;
    public float speed = 1;
    private bool chooseDir = false;
    public Vector3 randomDir;
    public Vector3 nextPos;
    public float health = 5;
    public GameObject[] itemPrefab;
    public float[] dropRates;
    private float lastAttack = 0f;
    public float attackInterval = 5f;
    private readonly float xMin = -7.8f;
    private readonly float xMax = 7.8f;
    private readonly float yMin = -3.7f;
    private readonly float yMax = 4.4f;
    public bool invulnerable = false;
    private float idleStartTime;
    private float idleLength;
    private bool playerInRange;
    private readonly float dropChance = 5.0f;
    public float windUpTime;
    public float attackTime;
    public float hitTime;
    public float dieSoundTime;
    public float dieTime;
    public bool ranged;
    public GameObject projectile;
    private SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        playerInRange = IsPlayerInRange(range);
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
            //lastAttack = Time.time;
            anim.SetBool("walk", true);
            currentState = EnemyState.Follow;
            return;
        }
        if (!chooseDir)
        {
            anim.SetBool("walk", false);
            chooseDir = true;
            idleLength = Random.Range(2f, 8f);
            randomDir = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
            idleStartTime = Time.time;
        }
        else
        {
            if (Time.time > idleStartTime + idleLength)
            {
                ChangeDirection(randomDir + transform.parent.position);
                currentState = EnemyState.Wander;
                anim.SetBool("walk", true);
                chooseDir = false;
            }
        }
    }

    void Wander()
    {
        if (playerInRange)
        {
            //lastAttack = Time.time;
            currentState = EnemyState.Follow;
            anim.SetBool("walk", true);
            return;
        }
        nextPos = randomDir - transform.localPosition;
        if (Vector3.Distance(randomDir, transform.localPosition) > 0.2f)
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
        if (ranged)
        {
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
            proj.transform.parent = gameObject.transform;
            proj.transform.localPosition = new Vector3(0.5f, 0, 0);
        }
        yield return null;
        anim.SetBool("attack", false);
        yield return new WaitForSeconds(windUpTime);
        AudioController.instance.Play(enemyInitial + "Atk");
        yield return new WaitForSeconds(attackTime - windUpTime);
        currentState = EnemyState.Follow;
    }

    void Attack()
    {
        if (!playerInRange)
        {
            anim.SetBool("walk", false);
            anim.SetBool("attack", false);
            chooseDir = false;
            currentState = EnemyState.Idle;
            return;
        }
        if (currentState != EnemyState.Attack)
        {
            ChangeDirection(player.transform.position);
            if (ranged)
                transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x + (1f/(transform.position.x - player.transform.position.x + 0.0001f)), player.transform.position.y, 0), speed * Time.deltaTime);
            else
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
        if (Time.time > lastAttack + attackInterval)
            StartCoroutine(Attacking());
    }

    private IEnumerator Hitstun()
    {
        invulnerable = true;
        sr.color = new Color(0.5f, 0.5f, 0.5f);
        if (currentState != EnemyState.Attack)
        {
            currentState = EnemyState.Hit;
            anim.SetBool("damage", true);
            float pspeed = speed;
            speed = 0;
            yield return null;
            anim.SetBool("damage", false);
            yield return new WaitForSeconds(hitTime);
            speed = pspeed;
            currentState = EnemyState.Follow;
        }
        else
            yield return new WaitForSeconds(hitTime);
        sr.color = new Color(1f, 1f, 1f);
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

    public void ForceIdle()
    {
        if (health > 0)
        {
            anim.SetBool("walk", false);
            anim.SetBool("attack", false);
            anim.SetBool("damage", false);
            if (transform.childCount > 0)
                transform.GetChild(0).gameObject.SetActive(false);
            invulnerable = false;
            chooseDir = false;
            currentState = EnemyState.Idle;
            lastAttack = Time.time;
        }
        else
        {
            if (invulnerable)
            {
                if (Random.Range(0f, 10f) > dropChance)
                    Instantiate(itemPrefab[Random.Range(0, itemPrefab.Length)], transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator Die()
    {
        invulnerable = true;
        currentState = EnemyState.Die;
        anim.SetBool("die", true);
        yield return null;
        anim.SetBool("die", false);
        yield return new WaitForSeconds(dieSoundTime);
        AudioController.instance.Play(enemyInitial + "Die");
        yield return new WaitForSeconds(dieTime - dieSoundTime);
        float drop = Random.Range(0f, 1f);
        for (int i = 0; i < dropRates.Length; i++)
        {
            if (drop <= dropRates[i])
            {
                Instantiate(itemPrefab[i], (transform.position + transform.position + player.transform.position)/3, Quaternion.identity);
                break;
            }
        }
        invulnerable = false;
        Destroy(gameObject);
    }
}
