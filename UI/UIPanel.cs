using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] GameObject Content;
    [SerializeField] CanvasGroup CanvasGroup;

    protected virtual void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        Content.SetActive(false);
    }
    public virtual void Open()
    {
        Content.SetActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(CanvasGroup.DOFade(1, 0.1f).From(0.7f));
        UIManager.Instance.OpenPanel(this);
    }
    public virtual void Close()
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(CanvasGroup.DOFade(0, 0.1f).From(1f));
        sq.OnComplete(() => Content.SetActive(false));
        UIManager.Instance.ClosePanel(this);
    }
}
