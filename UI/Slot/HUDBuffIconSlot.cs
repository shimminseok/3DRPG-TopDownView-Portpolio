using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDBuffIconSlot : MonoBehaviour
{
    [SerializeField] Image buffIcon;
    [SerializeField] TextMeshProUGUI buffDuration;

    Buff currentBuff;

    public void SetBuffIcon(Buff _buff)
    {
        buffIcon.sprite = _buff.BuffTableData.BuffImage;
        _buff.OnBuffUpdated += UpdateBuffTime;
    }

    public void UpdateBuffTime(float _duration)
    {
        buffDuration.text = $"{_duration.ToString("F1")}√ ";
    }
}
