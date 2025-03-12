using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Interpolate;
public class UIDescription : UIPanel
{

    public static UIDescription Instance;
    [SerializeField] TextMeshProUGUI npcName;
    [SerializeField] TextMeshProUGUI decription;

    [SerializeField] Transform funcBtnRoot;
    [SerializeField] DescriptionFuncBtn funcBtnPrefabs;
    List<DescriptionFuncBtn> funcBtnList = new List<DescriptionFuncBtn>();

    [Header("Accept Quest")]
    [SerializeField] UIQuestAccept uiQuestAccept;

    bool isFinishText;
    public bool isDialogueRunning;

    NPCData data;
    Coroutine currentCoroutine;

    NPCController controller;

    bool isInputNextDialogue;


    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
            Destroy(gameObject);
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
        StopAllCoroutines();
    }
    public void StartDefaultDialogue(NPCController _controller)
    {
        ResetDescription();
        UIManager.Instance.AllClosePanel();
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        CreateFuncBtn(_controller);
        controller = _controller;
        data = controller.NPCData;
        npcName.text = data.Name;
        Open();
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
    public void CreateFuncBtn(NPCController _npc)
    {
        int index = 0;
        foreach (var function in _npc.npcFunction.npcFunction)
        {
            if (index >= funcBtnList.Count)
            {
                DescriptionFuncBtn btn = Instantiate(funcBtnPrefabs, funcBtnRoot);
                funcBtnList.Add(btn);
            }
            funcBtnList[index].SetFuncButton(function.FuncType);
            funcBtnList[index].AddFuncAction(ButtonAction);
            index++;
        }
    }
    public void ButtonAction(NPCFunction _func)
    {
        if (controller.npcFunction.CheckFunction(_func))
        {
            controller.npcFunction.Interact(_func);
        }
    }
    public void OpenQuestAcceptUI(NPCController _npc)
    {
        uiQuestAccept.OnClickAvailQuest(_npc);
    }
    public void OnClickAcceptQuest(QuestData _data)
    {
        SaveQuestData acceptSaveData = QuestManager.Instance.GetActiveQuest(_data.Cartegory, _data.ID);
        if (acceptSaveData == null)
            StartCoroutine(StartDialogue(_data.PreQuestDialogues, () => uiQuestAccept.ShowQuestAcceptUI(_data)));
        else
        {

            if (acceptSaveData.IsCompleted)
            {
                StartCoroutine(StartDialogue(_data.PostQuestDialogues, () => uiQuestAccept.ShowQuestAcceptUI(_data)));
            }
        }
    }

    public override void Open()
    {
        base.Open();
        CameraController.Instance.ChangeDialogueCamera(controller.dialogueCamTrans, true);
        UIHUD.Instance.gameObject.SetActive(false);
    }
    public override void Close()
    {
        base.Close();
        CameraController.Instance.ChangeDialogueCamera(controller.dialogueCamTrans, false);
        ResetDescription();
        isDialogueRunning = false;
        UIHUD.Instance.gameObject.SetActive(true);
        uiQuestAccept.HideAcceptQuestSlot();
    }
}
