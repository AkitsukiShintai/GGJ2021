using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableStair : BasicToggle
{
    [Header("速度")]
    public float Speed = 1.0f;
    [Header("返回标点")]
    public Transform endTrans;
    public bool moving = false;
    private Vector3 startPos;
    private float distance;
    private bool reversed = false;
    private float currentDistance = 0.0f;

    public override void OnToggle()
    {
        moving = true;
    }

    private void OnEnable()
    {
        startPos = transform.position;
        distance = Vector3.Distance(startPos, endTrans.position);
    }
    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            currentDistance += Speed * Time.deltaTime;
            float alpha = currentDistance / distance;
            if(alpha > 1.0f)
            {
                currentDistance = 0.0f;
                alpha = 0.0f;
                reversed = !reversed;
            }
            if(reversed)
            {
                transform.position = Vector3.Lerp(endTrans.position, startPos, alpha);
            }
            else
            {
                transform.position = Vector3.Lerp(startPos, endTrans.position, alpha);
            }
        }
    }
}
