using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public string itemDescription;
    public string itemEffect;
    public Sprite itemImage;
}

public class CollectionController : MonoBehaviour
{
    public Item item;
    public float maxHealthMod;
    public float healthMod;
    public float moveSpeedMod;
    public float dashCoolDownMod;
    public float dashLengthMod;
    public float attackSpeedMod;
    public float attackDamageMod;
    public bool hover;
    private PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = item.itemImage;
        gameObject.AddComponent<PolygonCollider2D>();
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hover)
        {
            if (collision.CompareTag("Player") || collision.CompareTag("PlayerHitBox"))
            {
                player.numItems++;
                player.items.Add(gameObject);
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
                player.numItems--;
                player.items.Remove(gameObject);
                hover = false;
            }
        }
    }

    public void Use()
    {
        PlayerController.maxHealth += maxHealthMod;
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
        PlayerController.attackDamage += attackDamageMod;
        Destroy(gameObject);
    }
}
