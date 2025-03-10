using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageTxt;
    [SerializeField] RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform;

    public void ShowDamage(int _damage, Vector3 _worldPos)
    {
        damageTxt.text = _damage.ToString("N0");

        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 50, 2f).SetEase(Ease.OutExpo);
        damageTxt.DOFade(0, 2).SetEase(Ease.OutExpo).OnComplete(() => RetrunToPool());
    }
    void RetrunToPool()
    {
        damageTxt.alpha = 1;
        ObjectPoolManager.Instance.ReturnObject(gameObject);
    }
}
