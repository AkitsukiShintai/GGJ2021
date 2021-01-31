using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityRange : MonoBehaviour
{
    /// <summary>
    /// 角色可视范围
    /// </summary>
    // Start is called before the first frame update
    Player owningPlayer;
    public GameObject visionPrefab;
    [HideInInspector]
    public Transform VisibililtyObj;

    private float m_VisibilityRadius;
    public float visibilityRadius { get
        {
            return m_VisibilityRadius;
        }
        set
        {
            m_VisibilityRadius = value;
            VisibililtyObj.localScale = Vector3.one * m_VisibilityRadius * 2.0f;
        }
    }

    private void Start()
    {
        owningPlayer = GetComponent<Player>();
        VisibililtyObj = GameObject.Instantiate(visionPrefab).transform;
    }
    private void Update()
    {
        if(owningPlayer == null || !owningPlayer.gameObject.activeSelf) 
        {
            VisibililtyObj.gameObject.SetActive(false);
            return; 
        }
        VisibililtyObj.gameObject.SetActive(true);
        VisibililtyObj.position = transform.position;
        float distance = 1000.0f;
        var other = Player.FindAnotherPlayer(owningPlayer);
        if(other != null && other.gameObject.activeSelf)
        {
            distance = Vector3.Distance(other.transform.position, transform.position);
        }
        if (distance > owningPlayer.playerData.visionOverlap)
        {
            visibilityRadius = Mathf.Lerp(visibilityRadius, owningPlayer.playerData.vision, 0.3f);
        }
        else
        {
            visibilityRadius = Mathf.Lerp(visibilityRadius, owningPlayer.playerData.vision + owningPlayer.playerData.visionAmplification, 0.3f);
        }
    }
}
