using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiFunctionNPC : MonoBehaviour
{
    public List<INPCFunction> npcFunction = new List<INPCFunction>();

    void Awake()
    {
        npcFunction.AddRange(GetComponents<INPCFunction>());    
    }
    public void Interact()
    {
        foreach(var func in npcFunction)
        {
            func.Execute();
        }
    }
    public void AddFunction(INPCFunction _func)
    {
        if(!npcFunction.Contains(_func))
        {
            npcFunction.Add(_func);
        }
    }
    public void RemoveFunction(INPCFunction _func)
    {
        if (npcFunction.Contains(_func))
        {
            npcFunction.Remove(_func);
        }
    }

}
