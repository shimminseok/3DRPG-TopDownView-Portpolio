using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform RespawnPoint;

    public GameSaveData GameSaveData { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject);
    }
    void Start()
    {

    }
    void OnApplicationQuit()
    {
        if(GameSaveData == null)
            GameSaveData = new  GameSaveData();

        GameSaveData.Inventory = InventoryManager.Instance.GetInventoryItem();
        GameSaveData.ActiveQuests = QuestManager.Instance.ActiveQuests;
        GameSaveData.Gold = AccountManager.Instance.Gold;
        GameSaveData.EquipItems = EquipmentManager.Instance.EquipmentItems;
        GameSaveData.JobID = PlayerController.Instance.JobID;
        foreach (var skill in PlayerController.Instance.SkillManager.ResisteredSkill)
        {
            GameSaveData.ResisteredSkills[skill.Key] = skill.Value;
        }
        foreach (var item in UIHUD.Instance.HUDItemSlot)
        {
            GameSaveData.ResisteredItems[item.slotHotKey] = item.registedInventoryIndex;
        }

        SaveLoadManager.SaveData(GameSaveData, "SaveData");
    }
    public GameSaveData LoadGameData()
    {
        return SaveLoadManager.GameData ?? new GameSaveData();
    }
    void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        GameObject targetObj = GameObject.Find("StartingPoint");
        if(targetObj != null)
        {
            RespawnPoint = targetObj.transform;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
