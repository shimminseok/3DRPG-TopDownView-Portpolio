using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [Header("Base")]
    public GameObject Content;
    public virtual void OnClickOpenButton()
    {
        Content.SetActive(true);
        UIManager.Instance.OpenPanel(this);
    }
    public virtual void OnClickCloseButton()
    {
        Content.SetActive(false);
        UIManager.Instance.ClosePanel(this);
    }
}
