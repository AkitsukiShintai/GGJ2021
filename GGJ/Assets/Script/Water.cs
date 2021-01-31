using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    other.gameObject.GetComponent<PlayerMove>().Die();
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float myTop = GetComponent<BoxCollider>().bounds.max.y;
            Bounds playerBounds = other.gameObject.GetComponent<CapsuleCollider>().bounds;
            float playerTop = playerBounds.max.y;
            if (playerTop - 0.1 < myTop)
            {
                other.gameObject.GetComponent<PlayerMove>().Die();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    collision.gameObject.GetComponent<PlayerMove>().Die();
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float myTop = GetComponent<BoxCollider>().bounds.max.y;
            Bounds playerBounds = collision.gameObject.GetComponent<CapsuleCollider>().bounds;
            float playerTop = playerBounds.max.y;
            if (playerTop - 0.5 < myTop)
            {
                collision.gameObject.GetComponent<PlayerMove>().Die();
            }
        }
    }
}