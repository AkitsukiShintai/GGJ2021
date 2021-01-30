using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class VisLight : MonoBehaviour
{
    public LightData data;

    private Light owningLight;

    private float currentTime = 0.0f;

    private Transform childrenTrans;
    private void Reset()
    {
        owningLight = GetComponent<Light>();
        childrenTrans = transform.GetChild(0);
    }

    private void OnEnable()
    {
        owningLight = GetComponent<Light>();
        currentTime = data.duration;
        childrenTrans = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        float alpha = 1.0f;
        if(data.duration > 0.0f)
        {
            currentTime -= Time.deltaTime;
            alpha = Mathf.Clamp01(currentTime / data.duration);
        }
        owningLight.range = data.range;
        childrenTrans.localScale = Vector3.one * data.range * 2.0f;
        owningLight.intensity = data.intensity;
        if(alpha <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }
}
