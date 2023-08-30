using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("sadvhasjdfhah");
        if (other.CompareTag("Ground"))
        {
            //Debug.Log("sadv");
            Destroy(gameObject);
        }
        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            //Debug.Log("sadv");
            Destroy(gameObject);
        }
    }
}
