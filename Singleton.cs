using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }
}
