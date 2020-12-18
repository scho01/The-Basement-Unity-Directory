using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Text statsText;
    public Text healthText;
    public Text popUpText;
    public static float[] playerStats = new float[] {
    3,      //Max HP
    3,      //HP
    1,      //Atk
    3.5f,   //Spd
    0.2f,   //DD
    1,      //AtkCD
    1       //DCD
    };
//    public static int[] numItemsCollected = new int[19](0);
    public static bool invulnerable = false;
    public Slider healthSlider;
    private SpriteRenderer sr;
    private string itemEffect = "";
    private bool displayText;
    private float itemEffectTimer;

    void Start()
    {
        invulnerable = false;
        SetHealth();
        sr = GetComponent<SpriteRenderer>();
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

    private IEnumerator DisplayItemEffects(string effectDisplayText)
    {
        popUpText.text = effectDisplayText;
        popUpText.enabled = true;
        displayText = true;
        itemEffectTimer = 2 + Time.time;
        while (itemEffectTimer > Time.time)
        {
            yield return null;
        }
        popUpText.enabled = false;
        popUpText.text = "";
        displayText = false;
    }

    public void UseItem(float[] stats, string[] pEffect, string[] nEffect)
    {
        if (displayText)
            StopCoroutine(DisplayItemEffects(itemEffect));
        itemEffect = "";
        for (int i = 0; i < 5; i++)
        {
            playerStats[i] += stats[i];
        }
        for (int i = 5; i < 7; i++)
        {
            playerStats[i] *= stats[i];
        }
        if (playerStats[0] <= 0)
            playerStats[0] = 0.1f;
        if (playerStats[1] <= 0)
            playerStats[1] = 0.1f;
        if (playerStats[0] < playerStats[1])
            playerStats[1] = playerStats[0];
        SetHealth();
        for (int i = 0; i < pEffect.Length; i++)
            itemEffect += $"<color=lime>{pEffect[i]}</color>\n";
        for (int i = 0; i < nEffect.Length; i++)
            itemEffect += $"<color=red>{nEffect[i]}</color>\n";
        StartCoroutine(DisplayItemEffects(itemEffect));
    }

    public void UpdateStatsText()
    {
        statsText.text =
            "Atk = " + Mathf.Round(playerStats[2] * 10) / 10 + "\n"
            + "Atk CD = " + Mathf.Round(playerStats[5] * 1000) / 1000 + "\n"
            + "Spd = " + Mathf.Round(playerStats[3] * 10) / 10 + "\n"
            + "Dash Time = " + Mathf.Round(playerStats[4] * 100) / 100 + "\n"
            + "Dash CD = " + Mathf.Round(playerStats[6] * 1000) / 1000;
    }

    public void SetHealth()
    {
        healthText.text = Mathf.Round(playerStats[1] * 10) / 10 + "/" + Mathf.Round(playerStats[0] * 10) / 10;
        healthSlider.maxValue = playerStats[0];
        healthSlider.value = playerStats[1];
    }

    private IEnumerator Hitstun(bool boss)
    {
        invulnerable = true;
        if (!boss)
            playerStats[1] -= 0.5f;
        else
            playerStats[1] -= 1f;
        UpdateStatsText();
        if (playerStats[1] <= 0)
        {
            Die();
            yield break;
        }
        else
        {
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.1f);
            sr.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.05f);
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.05f);
            sr.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.05f);
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.05f);
            sr.color = new Color(1f, 1f, 1f);
            yield return new WaitForSeconds(0.05f);
            sr.color = new Color(0.7f, 0.7f, 0.7f);
            yield return new WaitForSeconds(0.05f);
            sr.color = new Color(1f, 1f, 1f);
            invulnerable = false;
        }
    }

    public void Hit(bool boss)
    {
        if (!invulnerable)
        {
            AudioController.instance.Play("PDmg");
            StartCoroutine(Hitstun(boss));
        }
    }

    public static void ResetStats()
    {
        playerStats[0] = 3;      //Max HP
        playerStats[1] = 3;      //HP
        playerStats[2] = 1;      //Atk
        playerStats[3] = 3.5f;   //Spd
        playerStats[4] = 0.2f;   //DD
        playerStats[5] = 1;      //AtkCD
        playerStats[6] = 1;      //DCD
    }

    private void Die()
    {
        AudioController.instance.Play("PDie");
        ResetStats();
        DungeonGenerator.reset = true;
        SceneManager.LoadScene(1);
    }
}
