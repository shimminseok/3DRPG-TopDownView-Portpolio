using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;


public class SaveLoadManager
{
    public static GameSaveData GameData { get; private set; }
    public static void SaveData(GameSaveData _data, string _fileName)
    {
        JsonSerializerSettings setting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        string json = JsonConvert.SerializeObject(_data, setting);
        string filePath = Path.Combine(Application.persistentDataPath, _fileName);

        File.WriteAllText(filePath, json);
        Debug.LogWarning("데이터 저장 완료");
    }
    public static GameSaveData LoadData(string _fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath,_fileName);
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameSaveData data = JsonConvert.DeserializeObject<GameSaveData>(json);
            Debug.Log("데이터 로드 성공");
            GameData = data;
            return data;
        }
        else
        {
            Debug.Log("데이터 로드 실패");
            return null;
        }
    }
    public static void ResetData(string _fileName)
    {
        GameData = new GameSaveData();
        SaveData(GameData, _fileName);
    }

}


