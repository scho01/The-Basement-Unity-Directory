using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    walk,
    attack,
    interact
}

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController pc;
    private Vector3 change;
    private float cspeed = 4;
    private bool dash = false;
    private float dashSpeed = 12;
    private float lastDash = -1f;
    private Vector3 attack;
    private float lastAttack = -1f;
    public Text itemText;
    private bool showItemDetails;
    private bool useItem;
    public PlayerState currentState;
    public int numItems = 0;
    public List<GameObject> items;
    private Vector3 bcoffset = new Vector3(0, -0.5f, 0);
    private string itemName = "";
    private string itemDescription = "";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        GetInputs();
        if ((attack != Vector3.zero) && (Time.time > lastAttack + PlayerController.attackSpeed))
            StartCoroutine(Attack());
        if (showItemDetails)
            ShowItemDetails(true);
        else
            ShowItemDetails(false);
        if (useItem)
            UseItem();
        UpdateAnimationAndMove();
    }

    IEnumerator Attack()
    {
        lastAttack = Time.time;
        anim.SetFloat("attackX", attack.x);
        anim.SetFloat("attackY", attack.y);
        anim.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        anim.SetBool("attacking", false);
        yield return new WaitForSeconds(0.2f);
        currentState = PlayerState.walk;
    }

    void GetInputs()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        change = Vector3.Normalize(change);
        attack = Vector3.zero;
        attack.x = Input.GetAxisRaw("AttackHorizontal");
        attack.y = Input.GetAxisRaw("AttackVertical");
        attack = Vector3.Normalize(attack);
        dash = Input.GetKey(KeyCode.LeftShift);
        showItemDetails = Input.GetKey(KeyCode.LeftAlt);
        useItem = Input.GetKeyDown(KeyCode.F);
    }

    void UpdateAnimationAndMove()
    {
        if (dash && Time.time > lastDash + PlayerController.dashCoolDown)
            StartCoroutine(Dash());
        if (change != Vector3.zero)
        {
            MovePlayer();
            if (currentState == PlayerState.walk)
            {
                anim.SetBool("moving", true);
                anim.SetFloat("moveX", change.x);
                anim.SetFloat("moveY", change.y);
            }
        }
        else
            if (currentState == PlayerState.walk)
                anim.SetBool("moving", false);
    }

    void MovePlayer()
    {
        rb.MovePosition(transform.position + change * cspeed * Time.deltaTime);
        rb.velocity = Vector3.zero;
    }

    IEnumerator Dash()
    {
        PlayerController.invulnerable = true;
        cspeed = dashSpeed;
        yield return new WaitForSeconds(PlayerController.dashLength);
        cspeed = PlayerController.moveSpeed;
        PlayerController.invulnerable = false;
        lastDash = Time.time;
    }

    private GameObject FindClosestItem()
    {
        if (numItems > 0)
        {
            GameObject closest = null;
            float cDistance = Mathf.Infinity;
            foreach (GameObject obj in items)
            {
                Vector3 diff = obj.transform.position - (transform.position + bcoffset);
                float curDistance = diff.sqrMagnitude;
                if (curDistance < cDistance)
                {
                    closest = obj;
                    cDistance = curDistance;
                }
            }
            return closest;
        }
        return null;
    }
    
    private void ShowItemDetails(bool press)
    {
        if (numItems > 0)
        {
            if (press)
            {
                itemText.enabled = true;
                GameObject closest = FindClosestItem();
                if (closest.CompareTag("Ladder"))
                {
                    itemName = closest.GetComponent<LadderController>().ladderName;
                    itemDescription = closest.GetComponent<LadderController>().ladderDescription;
                }
                else
                {
                    itemName = closest.GetComponent<CollectionController>().item.itemName;
                    itemDescription = closest.GetComponent<CollectionController>().item.itemDescription;
                }
                itemText.text = itemName + ": \n" + itemDescription;
            }
            else
            {
                itemText.enabled = false;
            }
        }
        else
        {
            itemText.enabled = false;
        }
    }

    private void UseItem()
    {
        if (numItems > 0)
        {
            GameObject closest = FindClosestItem();
            if (closest.CompareTag("Ladder"))
            {
                closest.GetComponent<LadderController>().Use();
            }
            else
            {
                closest.GetComponent<CollectionController>().Use();
            }
            pc.UpdateStatsText();
        }
    }
}
