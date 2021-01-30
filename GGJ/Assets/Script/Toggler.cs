using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler : MonoBehaviour
{
    [Header("相应的机关")]
    public BasicToggle basicToggle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            basicToggle?.OnToggle();
        }
    }
}
