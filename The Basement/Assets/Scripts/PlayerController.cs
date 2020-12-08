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
    public static float moveSpeed = 3;      // Walking speed
    public static float dashCoolDown = 1f;  // Cooldown between dashes, modified by multiplying with fraction
    public static float dashLength = 0.2f;  // Dash duration (affects both dash distance and invuln duration)
    public static float attackSpeed = 1;    // Cooldown between attacks, modified by multiplying with fraction
    public static float attackDamage = 1;   // Damage dealt per hit
    public static bool invulnerable = false;

    void Start()
    {
        invulnerable = false;
        UpdateStatsText();
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
        statsText.text = "Health = " + health + "/" + maxHealth + "\n"
            + "Spd = " + moveSpeed + "\n"
            + "Atk = " + attackDamage + "\n"
            + "AtkSpd = " + attackSpeed + "\n"
            + "DashTime = " + dashLength + "\n"
            + "DashCD = " + dashCoolDown + "\n";
    }

    private IEnumerator Hitstun(bool boss)
    {
        invulnerable = true;
        if (!boss)
            health -= 0.5f;
        else
            health -= 1f;
        UpdateStatsText();
        if (health <= 0)
        {
            Die();
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            invulnerable = false;
        }
    }

    public void Hit(bool boss)
    {
        if (!invulnerable)
        {
            StartCoroutine(Hitstun(boss));
        }
    }

    private void ResetStats()
    {
        maxHealth = 3;
        health = 3;
        moveSpeed = 3;
        dashCoolDown = 1f;
        dashLength = 0.2f;
        attackSpeed = 1;
        attackDamage = 1;
        UpdateStatsText();
    }

    private void Die()
    {
        ResetStats();
        DungeonGenerator.reset = true;
        SceneManager.LoadScene(0);
    }
}
