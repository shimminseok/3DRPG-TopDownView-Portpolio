using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(100)]

public class AttackAreaCollider : MonoBehaviour
{
    [SerializeField] bool IsPlayer;
    [SerializeField] LayerMask targetLayer;

    IAttacker attacker;
    AttackRangeData rangeData;
    List<Collider> hitEnemies = new List<Collider>();
    void Awake()
    {
        attacker = GetComponentInParent<IAttacker>();
    }
    void Start()
    {
        if (IsPlayer)
        {
            rangeData = PlayerController.Instance.jobData.AttackRangeData;

        }
        else
        {
            rangeData = GetComponentInParent<MonsterController>().monsterData.AttackRangeData;
            Debug.Log("Set RangeData");
        }
    }

    public void AttackStart()
    {
        hitEnemies = Physics.OverlapSphere(transform.position, rangeData.Range, targetLayer).ToList();
    }
    public void PerformAttack()
    {
        foreach (var target in hitEnemies)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) <= rangeData.Angle / 2)
            {
                IDamageable dam = target.GetComponentInParent<IDamageable>();
                if(dam != null)
                {
                    attacker?.Attack(dam);
                    Debug.Log($"Hit target : {target.name}");
                }
            }
        }
    }
    public void AttackEnd()
    {
        hitEnemies.Clear();
    }
    private void OnDrawGizmosSelected()
    {
        if (rangeData == null)
            return;
        Gizmos.color = Color.red;
        Vector3 left = Quaternion.Euler(0, -rangeData.Angle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, rangeData.Angle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position + rangeData.Offset , transform.position + (left * rangeData.Range)  + rangeData.Offset);
        Gizmos.DrawLine(transform.position + rangeData.Offset , transform.position + (right * rangeData.Range) + rangeData.Offset);
    }

}
