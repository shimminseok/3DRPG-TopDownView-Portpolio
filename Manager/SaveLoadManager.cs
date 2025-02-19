using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


[Serializable]
public class Wrapper<T>
{
    public List<T> Items;

    public Wrapper(List<T> _items)
    {
        Items = _items;
    }
    public Wrapper()
    {
        Items = new List<T>();
    }
}

public class SaveLoadManager
{

    public static string GetSavePath<T>()
    {
        return $"{Application.persistentDataPath}/{typeof(T).Name}.dat";
    }
    //public static void Save(GameSaveData _data)
    //{
    //    string json = JsonUtility.ToJson(_data, true);
    //    byte[] binaryData = System.Text.Encoding.UTF8.GetBytes(json);
    //    File.WriteAllBytes(savePath, binaryData);
    //    Debug.LogWarning("데이터 저장 완료");
    //}

    //public static GameSaveData Load()
    //{
    //    if(File.Exists(savePath))
    //    {
    //        byte[] binaryData = File.ReadAllBytes(savePath);
    //        string json = System.Text.Encoding.UTF8.GetString(binaryData);
    //        return JsonUtility.FromJson<GameSaveData>(json);
    //    }

    //    return null;
    //}
    public static void Save<T>(T _data)
    {
        string json = JsonUtility.ToJson(_data, true);
        byte[] binaryData = System.Text.Encoding.UTF8.GetBytes(json);
        File.WriteAllBytes(GetSavePath<T>(), binaryData);
        Debug.LogWarning("데이터 저장 완료");
    }
    public static void SaveList<T>(List<T> _data)
    {
        Wrapper<T> wrapper = new Wrapper<T>(_data);
        Save(wrapper);
    }
    public static T Load<T>() where T : new()
    {
        string path = GetSavePath<T>();
        if(File.Exists(path))
        {
            byte[] binaryData = File.ReadAllBytes(path);
            string json = Encoding.UTF8.GetString(binaryData);
            return JsonUtility.FromJson<T>(json);
        }

        return new T();
    }
    public static List<T> LoadList<T>()
    {
        Wrapper<T> wrapper = Load<Wrapper<T>>();
        return wrapper.Items ?? new List<T>();
    }

    public static void ResetAllSaves()
    {
        ResetSave<Wrapper<SaveItemData>>();
    }
    public static void ResetSave<T>()
    {
        string path = GetSavePath<T>();
        if(File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
