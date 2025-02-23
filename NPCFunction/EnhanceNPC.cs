using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhanceNPC : MonoBehaviour, INPCFunction
{
    public NPCFunction FuncType => NPCFunction.Enhance;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute()
    {
        UIManager.Instance.AllClosePanel();
        UIManager.Instance.CheckOpenPopup(UIEnhancement.Instance);
    }

    public void Initialize(NPCData _data)
    {
        
    }
}
