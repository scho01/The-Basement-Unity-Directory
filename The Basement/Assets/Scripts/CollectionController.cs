using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class Item
{
    public string itemName;
    public string[] positiveEffects;
    public string[] negativeEffects;
    public string itemDescription;
    public float[] stats;
/*  maxHealthMod;
    healthMod;
    attackDamageMod;
    attackSpeedMod;
    moveSpeedMod;
    dashLengthMod;
    dashCoolDownMod;*/
//    public Sprite itemImage;
//}

public class CollectionController : MonoBehaviour
{
//    public Item item;
    public string itemName;
    public string[] positiveEffects;
    public string[] negativeEffects;
    public string itemDescription;
    public float[] stats;
    /*  maxHealthMod;       0
        healthMod;          1
        attackDamageMod;    2
        moveSpeedMod;       3
        dashLengthMod;      4
        attackSpeedMod;     5
        dashCoolDownMod;    6*/
    //    public Sprite itemImage;
    public bool hover;
    private PlayerMovement pm;
    private PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
/*        gameObject.GetComponent<SpriteRenderer>().sprite = item.itemImage;
        gameObject.AddComponent<PolygonCollider2D>();
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;*/
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerMovement>();
        pc = player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hover)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("PlayerHitBox"))
            {
                pm.numItems++;
                pm.items.Add(gameObject);
                hover = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hover)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("PlayerHitBox"))
            {
                pm.numItems--;
                pm.items.Remove(gameObject);
                hover = false;
            }
        }
    }

    public void Use()
    {
        pc.UseItem(stats, positiveEffects, negativeEffects);
/*        PlayerController.maxHealth += maxHealthMod;
        if (PlayerController.maxHealth <= 0)
            PlayerController.maxHealth = 0.1f;
        PlayerController.health += healthMod;
        if (PlayerController.health <= 0)
            PlayerController.health = 0.1f;
        if (PlayerController.health > PlayerController.maxHealth)
            PlayerController.health = PlayerController.maxHealth;
        PlayerController.moveSpeed += moveSpeedMod;
        PlayerController.dashCoolDown *= dashCoolDownMod;
        PlayerController.dashLength += dashLengthMod;
        PlayerController.attackSpeed *= attackSpeedMod;
        PlayerController.attackDamage += attackDamageMod;*/
        Destroy(gameObject);
    }
}
