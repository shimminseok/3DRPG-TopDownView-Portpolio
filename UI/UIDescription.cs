using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
public class UIDescription : UIPanel
{

    public static UIDescription Instance;
    [SerializeField] TextMeshProUGUI npcName;
    [SerializeField] TextMeshProUGUI decription;

    [SerializeField] GameObject questButtonObj;
    [SerializeField] GameObject shopButtonObj;

    [Header("Accept Quest")]
    [SerializeField] UIQuestAccept uiQuestAccept;

    public QuestData SelectedQuestData;

    bool isFinishText;
    bool isSkip;
    public bool isDialogueRunning;

    NPCData data;
    Coroutine currentCoroutine;

    NPCController controller;

    public bool isInputNextDialogue;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
            Destroy(transform.root);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isInputNextDialogue = true;
        }
    }
    public void ResetDescription()
    {
        data = null;
        npcName.text = string.Empty;
        decription.text = string.Empty;
    }
    public void StartDefaultDialogue(NPCController _controller)
    {
        UIManager.Instance.AllClosePanel();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        controller = _controller;
        data = controller.NPCData;
        npcName.text = data.Name;
        OnClickOpenButton();
        currentCoroutine = StartCoroutine(StartDialogue(data.DefaultDialogues));

        //만약 완료된 퀘스트가 있다면 완료된 퀘스트 표시

    }
    IEnumerator StartDialogue(List<string> _desc, Action onDialogueComplete = null)
    {
        isDialogueRunning = true;
        for (int i = 0; i < _desc.Count; i++)
        {
            decription.text = _desc[i];
            yield return new WaitUntil(() => isInputNextDialogue);
            isInputNextDialogue = false;
        }
        isFinishText = true;

        onDialogueComplete?.Invoke();

    }
    public void OnClickShopBtn()
    {
        ShopNPC fuction = controller.GetComponent<ShopNPC>();
        if (fuction != null)
        {
            UIManager.Instance.AllClosePanel();
            fuction.Execute();
        }
    }
    public void OnClickQuestBtn()
    {
        uiQuestAccept.OnClickAvailQuest(controller);
    }
    public void OnClickAcceptQuest(QuestData _data)
    {
        StartCoroutine(StartDialogue(_data.PreQuestDialogues, () => uiQuestAccept.ShowQuestAcceptUI(_data)));
    }

    public override void OnClickOpenButton()
    {
        base.OnClickOpenButton();
        CameraController.Instance.ChangeDialogueCamera(true);
        UIHUD.Instance.gameObject.SetActive(false);
    }
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        CameraController.Instance.ChangeDialogueCamera(false);
        ResetDescription();
        isDialogueRunning = false;
        UIHUD.Instance.gameObject.SetActive(true);

    }
}
