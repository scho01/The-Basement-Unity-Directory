using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    CircleCollider2D cc;
    SpriteRenderer sr;
    public Text collectedText;
    public Text healthText;
    public static int atkStat = 1;
    public GameObject stickPrefab;
    private float lastAttack;
    public float attackInterval = 1f;
    public static int health = 3;
    public bool attackExists = false;
    Vector3 vel;
    GameObject stick;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(255, 255, 255);
        healthText.text = "HP = " + health;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float atkHorizontal = Input.GetAxis("AttackHorizontal");
        float atkVertical = Input.GetAxis("AttackVertical");
        bool dash = Input.GetKeyDown(KeyCode.LeftShift);
        if (dash)
            StartCoroutine(Dash());
        if((atkHorizontal != 0 || atkVertical != 0 ) && Time.time > lastAttack + attackInterval)
        {
            Attack(atkHorizontal, atkVertical);
            lastAttack = Time.time;
        }
        vel = new Vector3(horizontal, vertical, 0);
        rb.velocity = vel.normalized * speed;
        collectedText.text = "Atk = " + atkStat;
    }
    IEnumerator Dash()
    {
        cc.enabled = false;
        speed *= 10;
        yield return new WaitForSeconds(0.05f);
        speed /= 10;
        cc.enabled = true;
    }
    void Attack(float x, float y)
    {
        int angle = 0;
        if (x != 0)
        {
            if (y > 0)
            {
                if (x < 0)
                    angle = 0;
                else
                    angle = -90;
            }
            else if (y < 0)
            {
                if (x < 0)
                    angle = 90;
                else
                    angle = 180;
            }
            else
            {
                if (x < 0)
                    angle = 45;
                else
                    angle = -135;
            }
        }
        else
        {
            if (y < 0)
                angle = 135;
            else
                angle = -45;
        }
        stick = Instantiate(stickPrefab, transform.position, Quaternion.Euler(0, 0, angle)) as GameObject;
        stick.GetComponent<StickController>().parent = gameObject;
        attackExists = true;
    }

    private IEnumerator Hitstun()
    {
        cc.enabled = false;
        sr.color = new Color(150, 150, 150);
        yield return new WaitForSeconds(1f);
        sr.color = new Color(255, 255, 255);
        cc.enabled = true;
    }

    public void Hit()
    {
        health--;
        healthText.text = "HP = " + health;
        if (health > 0)
            StartCoroutine(Hitstun());
        else
            Die();
    }

    private void Die()
    {
        if (attackExists)
            Destroy(stick);
        Destroy(gameObject);
    }
}
