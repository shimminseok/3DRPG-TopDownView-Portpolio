using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] List<int> spawnMonsterIDs = new List<int>();
    [SerializeField] float respawnTime = 5f;
    List<MonsterController> activeMonsters = new List<MonsterController>();
    MonsterTable monsterTable;

    int maxSpawnCount = 4;
    bool isFirstInArea;


    Coroutine coPlayerExit;
    Coroutine coSpawnMonster;
    void Start()
    {
        monsterTable = TableLoader.Instance.GetTable<MonsterTable>();
        for (int i = 0; i < spawnMonsterIDs.Count; i++)
        {
            MonsterData monster = monsterTable.GetMonsterDataByID(spawnMonsterIDs[i]);
            ObjectPoolManager.Instance.CreatePool(monster.MonsterPrefabs, 2);
        }
    }
    IEnumerator SpawnMonsters()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);

            if (activeMonsters.Count >= maxSpawnCount)
                continue;


            SpawnMonster();
        }
    }
    void SpawnMonster()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        string spawnMonsterName = monsterTable.GetMonsterDataByID(spawnMonsterIDs[Random.Range(0, spawnMonsterIDs.Count)]).MonsterPrefabs.name;
        GameObject monster = ObjectPoolManager.Instance.GetObject(spawnMonsterName);
        if (monster.TryGetComponent<MonsterController>(out var monsterController))
        {
            monsterController.SetDeathEffect(PlayDeathEffect);
        }
        Vector3 randomPos = Random.insideUnitSphere * 15f;
        randomPos.y = 0;
        spawnPoint.position += randomPos;
        monster.transform.localPosition = spawnPoint.position;
        activeMonsters.Add(monsterController);
    }
    void PlayDeathEffect(MonsterController _monster)
    {
        activeMonsters.Remove(_monster);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (coPlayerExit != null)
                StopCoroutine(coPlayerExit);

            coSpawnMonster = StartCoroutine(SpawnMonsters());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopCoroutine(coSpawnMonster);
            coPlayerExit = StartCoroutine(PlayerExit());
        }
    }

    IEnumerator PlayerExit()
    {
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < activeMonsters.Count; i++)
        {
            if (!activeMonsters[i].IsChasing)
                ObjectPoolManager.Instance.ReturnObject(activeMonsters[i].gameObject);
        }

    }
}
