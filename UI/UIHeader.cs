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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
