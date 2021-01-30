using Assets.Script.Monster;
using UnityEngine;

public class MonsterRing : MonoBehaviour
{
    [Header("旋转速度")]
    public float rotateSpeed = 1.0f;
    [Header("颜色")]
    public Color color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    public Monster monster;
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
        if (monster != null)
        {
            alpha = monster.GetRage() / 500.0f;
        }
        mat.SetColor("_Color", color * alpha);
    }
}