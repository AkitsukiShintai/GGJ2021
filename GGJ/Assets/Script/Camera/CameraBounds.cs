using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    Camera cam;
    Bounds cameraBounds;
    Transform testPoint;

    public Player player0;
    public Player player1;
    public Transform LeftWall;
    public Transform RightWall;
    public static CameraBounds Inst { get; private set; }

    private void OnEnable()
    {
        
        Inst = this;
        cam = GetComponent<Camera>();
    }

    private void OnDisable()
    {
        if(Inst == this)
        {
            Inst = null;
        }
    }
    void Start()
    {
        if(Player.players != null)
        {
            player0 = Player.players[0];
            player1 = Player.players[1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        Player p = player0;
        if(p == null || !p.gameObject.activeSelf)
        {
            p = player1;
        }
        if (p == null || !p.gameObject.activeSelf) return;
        Vector3 Targeting = p.transform.position;
        float interval = 0.0f;
        var otherP = Player.FindAnotherPlayer(p);
        if (otherP != null || !otherP.gameObject.activeSelf)
        {
            Targeting = (p.transform.position + otherP.transform.position) * 0.5f;
            interval = Mathf.Abs(p.transform.position.x - otherP.transform.position.x);
        }

        Targeting.y = cam.transform.position.y;
        float dist = cam.transform.InverseTransformPoint(p.transform.position).z;
        float widthFov = cam.fieldOfView * cam.aspect;
        cam.transform.position = new Vector3(Targeting.x, Targeting.y, cam.transform.position.z);// Targeting - cam.transform.forward * dist;
        float width = Mathf.Max(0.0f, Mathf.Tan(widthFov * 0.5f * Mathf.Deg2Rad) * dist * 2.0f - 2.0f);

        LeftWall.position = Targeting - Vector3.right * width * 0.5f;
        RightWall.position = Targeting + Vector3.right * width * 0.5f;

        if (interval > width)
        {
            Debug.Log("Out of Range");
        }
    }
}
