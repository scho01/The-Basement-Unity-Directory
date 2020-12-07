using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Text statsText;
    public static float maxHealth = 3;      // Maximum health
    public static float health = 3;         // Current health
    public static float moveSpeed = 4;      // Walking speed
    public static float dashCoolDown = 1f;  // Cooldown between dashes, modified by multiplying with fraction
    public static float dashLength = 0.2f;  // Dash duration (affects both dash distance and invuln duration)
    public static float attackSpeed = 1;    // Cooldown between attacks, modified by multiplying with fraction
    public static float attackDamage = 1;   // Damage dealt per hit
    public static bool invulnerable = false;
    public GameObject phb;

    void Start()
    {
        UpdateStatsText();
    }

    void Update()
    {

    }
    /*    void Attack(float x, float y)
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
    */

    public void UpdateStatsText()
    {
        statsText.text = "HP = " + health + "/" + maxHealth + "\n"
            + "MS = " + moveSpeed + "\n"
            + "AD = " + attackDamage + "\n"
            + "AS = " + attackSpeed + "\n"
            + "DL = " + dashLength + "\n"
            + "DCD = " + dashCoolDown + "\n"
            + "I = " + invulnerable;
    }

    public void Invulnerable(bool status)
    {
        if (status)
        {
            phb.SetActive(false);
            invulnerable = true;
        }
        else
        {
            invulnerable = false;
            phb.SetActive(true);
        }
        UpdateStatsText();
    }

    private IEnumerator Hitstun()
    {
        Invulnerable(true);
        health--;
        UpdateStatsText();
        if (health < 1)
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            Invulnerable(false);
        }
    }

    public void Hit()
    {
        if (!invulnerable)
        {
            StartCoroutine(Hitstun());
            if (health < 1)
                Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
