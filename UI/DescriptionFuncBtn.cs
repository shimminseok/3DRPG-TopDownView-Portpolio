using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Interpolate;

public class DescriptionFuncBtn : MonoBehaviour
{
    [SerializeField] Image funcIcon;
    [SerializeField] TextMeshProUGUI funcText;
    [SerializeField] EventTrigger funcAction;

    EventTrigger.Entry pointerClickEntry;
    NPCFunction funcType;
    public void SetFuncButton(NPCFunction _funcType)
    {
        RemoveAction();
        switch (_funcType)
        {
            case NPCFunction.Quest:
                funcIcon.sprite = SpriteAtlasManager.Instance.GetSprite("UI", "menu_icon_024");
                funcText.text = "퀘스트";
                break;
            case NPCFunction.Shop:
                funcIcon.sprite = SpriteAtlasManager.Instance.GetSprite("UI", "menu_icon_032");
                funcText.text = "상점";
                break;
            case NPCFunction.Enhance:
                funcIcon.sprite = SpriteAtlasManager.Instance.GetSprite("UI", "menu_icon_005");
                funcText.text = "강화";
                break;
        }

        funcType = _funcType;
    }
    public void AddFuncAction(Action<NPCFunction> _action)
    {
        pointerClickEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        pointerClickEntry.callback.AddListener((BaseEventData _baseData) => _action(funcType));
        funcAction.triggers.Add(pointerClickEntry);
    }
    public void RemoveAction()
    {
        funcAction.triggers.Remove(pointerClickEntry);
    }
}
