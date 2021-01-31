using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    private PlayerMove move;
    public UnityEngine.UI.Image rageImg;
    public UnityEngine.UI.Image normalImg;
    public UnityEngine.UI.Image starImg;
    public Color NormalColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    public Color RageColor = new Color(1.0f, 0.0f, 0.0f, 0.8f);

    private void OnEnable()
    {
        move = player.GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        starImg.gameObject.SetActive(player.haveStar);
        normalImg.gameObject.SetActive(player.gameObject.activeSelf);
        rageImg.fillAmount = player.rageValue / player.playerData.rageMax;
        rageImg.color = Color.Lerp(NormalColor, RageColor, rageImg.fillAmount);
    }
}
