using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIPanel : MonoBehaviour
{
    [Header("Base")]
    public GameObject Content;
    public CanvasGroup CanvasGroup;

    protected virtual void Awake()
    {
    }
    public virtual void OnClickOpenButton()
    {
        Content.SetActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(CanvasGroup.DOFade(1, 0.1f).From(0.7f));
        UIManager.Instance.OpenPanel(this);
    }
    public virtual void OnClickCloseButton()
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(CanvasGroup.DOFade(0, 0.1f).From(1f));
        sq.OnComplete(() => Content.SetActive(false));
        UIManager.Instance.ClosePanel(this);
    }
}
