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
    void OnApplicationQuit()
    {
        GameSaveData gameSaveData = SaveLoadManager.GameData;

        if (gameSaveData == null)
            gameSaveData = new GameSaveData();

        gameSaveData.Inventory = InventoryManager.Instance.GetInventoryItem();
        gameSaveData.ActiveQuests = QuestManager.Instance.ActiveQuests;
        gameSaveData.Gold = AccountManager.Instance.Gold;
        gameSaveData.EquipItems = EquipmentManager.Instance.EquipmentItems;
        gameSaveData.JobID = PlayerController.Instance.JobID;
        foreach (var skill in PlayerController.Instance.SkillManager.ResisteredSkill)
        {
            gameSaveData.ResisteredSkills[skill.Key] = skill.Value;
        }
        foreach (var item in UIHUD.Instance.HUDItemSlot)
        {
            gameSaveData.ResisteredItems[item.slotHotKey] = item.registedInventoryIndex;
        }
        gameSaveData.VectorData = new List<float> { PlayerController.Instance.transform.position.x, PlayerController.Instance.transform.position.y, PlayerController.Instance.transform.position.z };
        SaveLoadManager.SaveData(gameSaveData, "SaveData");
    }
    public GameSaveData LoadGameData()
    {
        return SaveLoadManager.GameData ?? new GameSaveData();
    }
    void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        GameObject targetObj = GameObject.Find("StartingPoint");
        if (targetObj != null)
        {
            RespawnPoint = targetObj.transform;
        }
        Vector3 loadPostion = LoadGameData().VectorData.Count != 0 ? new Vector3(LoadGameData().VectorData[0], LoadGameData().VectorData[1], LoadGameData().VectorData[2]) : Vector3.zero;
        if (loadPostion != Vector3.zero && loadPostion != RespawnPoint.position)
        {
            RespawnPoint.position = loadPostion;
        }


    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
