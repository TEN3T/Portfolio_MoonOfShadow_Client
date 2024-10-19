using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : InGameUI
{
    private Image hpBar;

    private void Awake()
    {
        hpBar = GetComponent<Image>();
    }

    public void HpBarSetting(Vector3 pos, float currentHp, float maxHp)
    {
        transform.position = CameraManager.Instance.cam.WorldToScreenPoint(pos);
        hpBar.fillAmount = currentHp / maxHp;
    }
}
