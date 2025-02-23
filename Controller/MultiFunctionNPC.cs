using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiFunctionNPC : MonoBehaviour
{
    public List<INPCFunction> npcFunction = new List<INPCFunction>();

    public void Interact(NPCFunction _func)
    {
        npcFunction.Find(x => x.FuncType == _func)?.Execute();
    }
    public bool CheckFunction(NPCFunction _func)
    {
        return npcFunction.Exists(x => x.FuncType == _func);
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
    public INPCFunction GetFunction(NPCFunction _func)
    {
        return npcFunction.Find(x => x.FuncType == _func);
    }

}
