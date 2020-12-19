using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Vector3 change;
    private float cspeed = 3.5f;
    private bool dash = false;
    public float dashSpeed = 12;
    private float lastDash = -1f;
    private Vector3 attack;
    private float lastAttack = -1f;
    private GameObject closestItem;
    public Text itemText;
    public Text toggleText;
    private bool showItemDetails;
    private bool showStats;
    private bool useItem;
    public PlayerState currentState;
    public int numItems = 0;
    public List<GameObject> items;
    private Vector3 bcoffset = new Vector3(0, -0.5f, 0);
    private static bool autoUseToggle = false;
    private PlayerController pc;
    public static bool vScreen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
        cspeed = PlayerController.playerStats[3];
    }

    void Update()
    {
        GetInputs();
        if (Input.GetKey(KeyCode.Backspace))
        {
            PlayerController.playerStats[2] = 99;
            PlayerController.playerStats[3] = 15;
            PlayerController.playerStats[5] = 0.1f;
            PlayerController.invulnerable = true;
            cspeed = 15;
            //pc.UpdateStatsText();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            autoUseToggle = !autoUseToggle;
            if (autoUseToggle)
                toggleText.enabled = true;
            else
                toggleText.enabled = false;
        }
        //        if (quit)
        //            Application.Quit();
        if ((attack != Vector3.zero) && (Time.time > lastAttack + PlayerController.playerStats[5]))
            StartCoroutine(Attack());
        if (showStats)
            ShowStats(true);
        else
            ShowStats(false);
        pc.SetHealth();
        if (numItems > 0)
        {
            closestItem = FindClosestItem();
            if (showItemDetails)
                ShowItemDetails(true);
            else
                ShowItemDetails(false);
            if (useItem || autoUseToggle)
                UseItem();
        }
        else
            ShowItemDetails(false);
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
        showItemDetails = Input.GetKey(KeyCode.F);
        showStats = Input.GetKey(KeyCode.Tab);
        useItem = Input.GetKeyDown(KeyCode.Space);
//        quit = Input.GetKey(KeyCode.Escape);
    }

    void UpdateAnimationAndMove()
    {
        if (dash && Time.time > lastDash + PlayerController.playerStats[6])
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
        yield return new WaitForSeconds(PlayerController.playerStats[4]);
        cspeed = PlayerController.playerStats[3];
        if (!vScreen)
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

    private void ShowStats(bool press)
    {
        if (press)
        {
            pc.UpdateStatsText();
            pc.statsText.enabled = true;
        }
        else
        {
            pc.statsText.enabled = false;
        }
    }


    private void ShowItemDetails(bool press)
    {
        if (press)
        {
            itemText.enabled = true;
            if (closestItem.CompareTag("Ladder"))
            {
                LadderController lc = closestItem.GetComponent<LadderController>();
                if (RoomController.instance.currentFloorNum < 3)
                    itemText.text = $"<size=48>{lc.ladderName}:</size>\n\n{lc.ladderDescription}";
                else
                    itemText.text = $"<size=48>{lc.ladderName}:</size>\n\n{lc.reverseDescription}";
            }
            else
            {
                CollectionController cc = closestItem.GetComponent<CollectionController>();
                itemText.text = $"<size=48>{cc.itemName}:</size>\n\n";
                for (int i = 0; i < cc.positiveEffects.Length; i++)
                    itemText.text += $"<color=lime>{cc.positiveEffects[i]}</color>\n";
                for (int i = 0; i < cc.negativeEffects.Length; i++)
                    itemText.text += $"<color=red>{cc.negativeEffects[i]}</color>\n";
                itemText.text += "\n" + cc.itemDescription;
            }
        }
        else
        {
            itemText.enabled = false;
        }
    }

    private void UseItem()
    {
        if (closestItem.CompareTag("Ladder"))
        {
            if (useItem)
                closestItem.GetComponent<LadderController>().Use();
        }
        else
        {
            closestItem.GetComponent<CollectionController>().Use();
        }
    }

    public void ReadyForLoad()
    {
        numItems = 0;
        items.Clear();
        transform.position = Vector3.zero;
    }
}
