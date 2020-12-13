using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBeam : MonoBehaviour
{
    public float duration;
    public float charge;
    public float rotationSpeed;
    private bool charged;
    private Vector3 offset;
    private Vector3 difference;
    private float rotationZ;
    private GameObject player;

    IEnumerator ChargeUp()
    {
        yield return new WaitForSeconds(charge);
        charged = true;
        Vector3 difference = player.transform.position - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
    }

    IEnumerator Projectile()
    {
        yield return new WaitForSeconds(duration);
        Destroy(transform.parent.gameObject);
    }

    private void Start()
    {
        charged = false;
        offset = new Vector3(0.125f, 1.06f, 0);
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ChargeUp());
        StartCoroutine(Projectile());
    }

    private void Update()
    {
        if (charged)
        {
            difference = player.transform.position - transform.position - offset;
            rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0.0f, 0.0f, rotationZ), rotationSpeed * Time.deltaTime);
        }
    }
}
