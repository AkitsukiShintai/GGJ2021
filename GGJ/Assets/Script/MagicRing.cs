using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRing : MonoBehaviour
{
    [Header("旋转速度")]
    public float rotateSpeed = 1.0f;
    [Header("颜色")]
    public Color color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    public Player player;
    private MeshRenderer renderer;
    private Material mat;


    private void OnEnable()
    {
        renderer = GetComponent<MeshRenderer>();
        mat = renderer.material;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), rotateSpeed * Time.deltaTime, Space.Self);
        float alpha = 1.0f;
        if (player != null)
        {
            alpha = player.rageValue / player.playerData.rageMax;
        }
        mat.SetColor("_Color", color * alpha);
    }
}
