using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class YesOrNoPopup : UIPanel
{
    public static YesOrNoPopup Instance { get; private set; }


    [SerializeField] TextMeshProUGUI descript;
    [SerializeField] Button yesButton;
    [SerializeField] TextMeshProUGUI yesBtnText;
    [SerializeField] Button noButton;
    [SerializeField] TextMeshProUGUI noBtnText;

    UnityAction yesAction;
    UnityAction noAction;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }
    public override void Open()
    {
        base.Open();
        yesButton.onClick.RemoveAllListeners();

        if(yesAction != null)
            yesButton.onClick.AddListener(yesAction);

        noButton.onClick.RemoveAllListeners();

        if(noAction != null)
            noButton.onClick.AddListener(noAction);

        yesButton.onClick.AddListener(Close);
        noButton.onClick.AddListener(Close);


    }
    public override void Close()
    {
        base.Close();
    }
    public void SetMessage(string _text)
    {
        descript.text = _text;
    }
    public void SetYesButton(UnityAction _action, string _desc = "»Æ¿Œ")
    {
        yesAction = _action;
        yesBtnText.text = _desc;
    }
    public void SetNoButton(UnityAction _action, string _desc ="¥›±‚")
    {
        noAction = _action;
        noBtnText.text = _desc;
    }
}
