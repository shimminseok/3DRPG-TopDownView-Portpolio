using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    [SerializeField] MonsterController monsterController;

    MonsterData monsterData;


    // Update is called once per frame
    private void Start()
    {
        monsterController = GetComponentInParent<MonsterController>();

        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = 100;
        col.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && monsterController.monsterData.IsAggressive)
        {
            if(other.TryGetComponent<IAttacker>(out var attacker))
            {
                StartCoroutine(monsterController.HandleTargeting(attacker));
            }
            else
                Debug.Log("대상에 Attacker Interface가 없습니다.");
        }
    }
}
