using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent (typeof(MultiFunctionNPC))]
public class NPCController : CharacterControllerBase, IInteractable, IDisplayable
{
    public int NPC_ID;
    public NPCType npcType;
    public MultiFunctionNPC npcFunction;
    public Transform dialogueCamTrans;

    NPCTable npcTable;
    NPCData npcData;

    List<QuestData> cachedQuests;
    List<QuestData> npcQuestList;

    public UnityEvent OnInteract { get; private set; }

    public NPCData NPCData => npcData;
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        Init();
    }
    protected override void Init()
    {
        npcTable = TableLoader.Instance.GetTable<NPCTable>();
        npcData = npcTable.GetNPCDataByID(NPC_ID);
        name = npcData.Name;
        AddNPCComponents();
    }
    bool HasFunction(NPCFunction npcFunctions, NPCFunction functionToCheck)
    {
        return (npcFunctions & functionToCheck) == functionToCheck;
    }
    void AddNPCComponents()
    {
        var functionType = Enum.GetValues(typeof(NPCFunction));
        foreach(NPCFunction func in functionType)
        {
            if(HasFunction(npcData.NPCFunctions,func))
            {
                AddComponentForFunction(func, npcData);
            }
        }
    }
    void AddComponentForFunction(NPCFunction _func, NPCData _npcData)
    {
        Type componentType = GetComponentTypeForFunction(_func);
        if (componentType != null && !gameObject.TryGetComponent(componentType, out _))
        {
            var component = gameObject.AddComponent(componentType);
            if (component is INPCFunction npcComponent)
            {
                npcComponent.Initialize(npcData);
                npcFunction.AddFunction(npcComponent);
            }
        }
    }
    private Type GetComponentTypeForFunction(NPCFunction function)
    {
        return function switch
        {
            NPCFunction.Shop => typeof(ShopNPC),
            NPCFunction.Quest => typeof(QuestNPC),
            NPCFunction.Enhance => typeof(EnhanceNPC),
            _ => null,
        };
    }
    void IInteractable.OnInteract()
    {
        Debug.Log($"플레이어와 상호작용 헀음");
        UIDescription.Instance.StartDefaultDialogue(this);
        transform.LookAt(PlayerController.Instance.transform);
        //만약 대화 퀘스트가 있다면 완료 처리
    }
    void StopNPC()
    {
        transform.LookAt(PlayerController.Instance.transform);
    }
    void IInteractable.OnExitInteract()
    {
        UIDescription.Instance.ResetDescription();
    }

    public void ShowHUD()
    {
        UITargetInfoHUD.Instance.ShowHUD(name,0,0,0);
    }
    public void HideHUD()
    {
        UITargetInfoHUD.Instance.HideHUD();
    }
}
