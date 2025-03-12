using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector3 RespawnPoint;
    public Vector3 SaveRespawnPoint;

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
        GameObject targetObj = GameObject.FindWithTag("StartingPoint");
        var loadData = LoadGameData();
        if (targetObj != null)
        {
            RespawnPoint = targetObj.transform.position;
            SaveRespawnPoint = targetObj.transform.position;
        }
        if (loadData.VectorData.Count != 0)
        {
            Vector3 loadPosition = new Vector3(loadData.VectorData[0], loadData.VectorData[1], loadData.VectorData[2]);
            RespawnPoint = loadPosition;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
