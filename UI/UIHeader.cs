using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] Transform targetTrans;

    Vector2 beginPoint;
    Vector2 moveBegin;


    UIPanel targetPanel;
    void Awake()
    {
        targetTrans = transform.parent;
    }
    public void OnDrag(PointerEventData eventData)
    {
        targetTrans.position = beginPoint + (eventData.position - moveBegin);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        beginPoint = targetTrans.position;
        moveBegin = eventData.position;
    }
    public void OnClickCloseBtn()
    {
        if(targetPanel == null)
            targetPanel = GetComponentInParent<UIPanel>();

        targetPanel?.Close();
    }
}
