using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(MonsterAnimator))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(CharacterStatus))]
[RequireComponent(typeof(MonsterAniEventListener))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
public class MonsterController : CharacterControllerBase, IDamageable, IMoveable, IAttacker, IDisplayable
{
    [Header("Component")]
    [SerializeField] MonsterAnimator animator;
    [SerializeField] BuffManager buffManager;
    [SerializeField] CharacterStatus characterStatus;
    [SerializeField] Renderer objectRenderer;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] CapsuleCollider bodyCollider;
    [SerializeField] MonsterStat monsterStat;


    public int ID;
    public MonsterData monsterData;

    Camera mainCam;
    Vector3 spawnPoint;
    float checkTimer = 0.2f;
    bool isChasing = false;


    IDamageable target;

    public Transform Transform => transform;
    public BuffManager BuffManager => buffManager;

    public bool IsChasing => isChasing;
    public float FinalDam => monsterStat.Attack.FinalValue;


    public Action<int, int> OnHealthChanged; //���� HP�� �ִ� HP�� ����
    public Action<MonsterController> OnDeath;
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        mainCam = Camera.main;
        monsterData = TableLoader.Instance.GetTable<MonsterTable>()?.GetMonsterDataByID(ID);
        GetComponents();
        Init();
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        if (monsterData.IsAggressive)
        {
            GameObject go = new GameObject("DetectionRange");
            go.transform.SetParent(transform, false);
            go.AddComponent<DetectionRange>();
        }
    }
    void GetComponents()
    {
        animator = GetComponent<MonsterAnimator>();
        characterStatus = GetComponent<CharacterStatus>();
        buffManager = GetComponent<BuffManager>();
        agent = GetComponent<NavMeshAgent>();
        monsterStat = GetComponent<MonsterStat>();


    }
    void UpdateHealth(float _newValue)
    {
        int maxHP = Mathf.RoundToInt(monsterStat.Health.FinalValue);
        int currentHP = Mathf.RoundToInt(_newValue); ;

        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
    void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= 0.2f)
        {
            checkTimer = 0;
            objectRenderer.enabled = IsInView() || isChasing;
        }
        if (currentState == CharacterState.Stun || currentState == CharacterState.Dead || currentState == CharacterState.Hit || !IsInView())
        {
            return;
        }
        HandleState();
    }
    protected override void Init()
    {
        base.Init();
        monsterStat.InitializeFromMonsterData(monsterData);
        agent.stoppingDistance = monsterData.AttackRangeData.Range;
        bodyCollider.enabled = true;
        ChangeCharacterState(CharacterState.Idle);
        agent.Warp(transform.position);
        spawnPoint = transform.position;
        monsterStat.CurrentHP.OnStatChanged += UpdateHealth;
        PlayerController.Instance.OnPlayerDeath += HandlePlayerDeath;
        agent.enabled = true;
    }
    void HandleState()
    {
        switch (currentState)
        {
            case CharacterState.Idle:
                if(target != null)
                {
                    HandleAttack();
                }
                else if(currentState != CharacterState.Idle)
                {
                    ChangeCharacterState(CharacterState.Idle);
                }
                break;
            case CharacterState.Move:
                Move(target.Transform.position);
                break;
            case CharacterState.Attack:
                HandleAttack();
                break;
            case CharacterState.Return:
                Return(spawnPoint);
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.Hit:
                animator.ResetTrigger("Attack");
                break;
        }
    }
    public void ChangeCharacterState(CharacterState _changeState)
    {
        if (currentState == CharacterState.Move && _changeState != CharacterState.Move)
        {
            StopMovement();
        }
        currentState = _changeState;
        animator.ChangeCharacterState(currentState);
    }

    public void TakeDamage(int _damage, IAttacker _attacker = null)
    {
        if (currentState == CharacterState.Dead && _attacker == null)
        {
            return;
        }
        ChangeCharacterState(CharacterState.Hit);
        ApplyDamage(_damage);

        StartCoroutine(HandleTargeting(_attacker));
        Debug.Log($"{gameObject.name}�� {_attacker}���� ���ݴ��߽��ϴ� : -{_damage}");
    }
    public IEnumerator HandleTargeting(IAttacker _attacker, float _stunDuration = 0)
    {
        yield return new WaitForSeconds(_stunDuration);
        if (currentState == CharacterState.Dead)
            yield break;
        if (_attacker is IDamageable damable  && target == null)
        {
            target = damable;
        }
        isChasing = true;
        ChangeCharacterState(CharacterState.Move);
    }
    void ApplyDamage(int _dam)
    {
        _dam = CalculateDamage(_dam);
        monsterStat.CurrentHP.ModifyAllValue(_dam);
        DamageTextSpawner.Instance.ShowDamage(_dam, transform.position);
        if (monsterStat.CurrentHP.FinalValue <= 0)
        {
            Die();
        }
    }
    void HandlePlayerDeath()
    {
        if(target != null)
        {
            target = null;
            ChangeCharacterState(CharacterState.Return);
        }

    }
    public int CalculateDamage(int _dam)
    {
        float damageReduction = monsterStat.Defense.FinalValue / (monsterStat.Defense.FinalValue + 50);
        int finalDam = Mathf.RoundToInt(_dam * (1 - damageReduction));
        return Mathf.Max(1, finalDam);
    }
    public void Die()
    {
        if (currentState == CharacterState.Dead)
        {
            return;
        }

        bodyCollider.enabled = false;
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Attack");
        ChangeCharacterState(CharacterState.Dead);
        OnDeath?.Invoke(this);

        QuestManager.Instance.OnTargetAchieved(QuestTargetType.Monster, ID);
        ObjectPoolManager.Instance.ReturnObject(gameObject, 3,Init);
        agent.enabled = false;
        monsterStat.CurrentHP.OnStatChanged -= UpdateHealth;
        PlayerController.Instance.OnPlayerDeath -= HandlePlayerDeath;
        PlayerController.Instance.characterStat.GainExp(50);
        DropGold();
        DropItem();

        target = null;

    }
    public void Move(Vector3 _moveDir)
    {
        animator.ResetTrigger("Attack");
        if (currentState == CharacterState.Attack || currentState == CharacterState.Dead)
            return;

        agent.SetDestination(_moveDir);

        if (!agent.pathPending && agent.remainingDistance <= monsterData.AttackRangeData.Range)
        {
            if (target != null && Vector3.Distance(transform.position, target.Transform.position) <= monsterData.AttackRangeData.Range)
            {
                ChangeCharacterState(CharacterState.Attack);
                return;
            }
            else if (currentState == CharacterState.Return)
            {
                isChasing = false;
                StopMovement();
                return;
            }
        }
        else if (agent.remainingDistance >= 20f)
        {
            ChangeCharacterState(CharacterState.Return);
        }
        animator.PlayMoveAnimation(true);
    }
    public void Return(Vector3 _point)
    {
        if (currentState != CharacterState.Return)
        {
            target = null;
        }

        Move(spawnPoint);
        monsterStat.RecoverHP(Mathf.RoundToInt(monsterStat.Health.FinalValue * 0.03f));
    }
    public void StopMovement()
    {
        if(!agent.isOnNavMesh)
            Debug.LogWarning($"�׺�޽� ���� ����!");
        agent.isStopped = true;
        agent.ResetPath();
        animator.PlayMoveAnimation(false);
    }
    public void Hit()
    {
        ChangeCharacterState(CharacterState.Hit);
    }

    #region Attack
    public void HandleAttack()
    {
        if (Vector3.Distance(transform.position, target.Transform.position) > monsterData.AttackRangeData.Range && currentState != CharacterState.Attack)
        {
            ChangeCharacterState(CharacterState.Move);
        }
        else
        {
            transform.LookAt(target.Transform.position);
            ChangeCharacterState(CharacterState.Attack);
        }
    }
    public void Attack(IDamageable _target)
    {
        target = _target;
        _target.TakeDamage(Mathf.RoundToInt(monsterStat.Attack.FinalValue));
    }
    #endregion
    bool IsInView()
    {
        Vector3 viewPort = mainCam.WorldToViewportPoint(transform.position);

        return viewPort.x >= -0.1f && viewPort.x <= 1.1f &&
            viewPort.y >= -0.1f && viewPort.y <= 1.1f &&
            viewPort.z > 0;
    }

    public void ShowHUD()
    {
        UITargetInfoHUD.Instance.ShowHUD(monsterData.MonsterName, Mathf.RoundToInt(monsterStat.CurrentHP.FinalValue), Mathf.RoundToInt(monsterStat.Health.FinalValue), monsterData.Level, this);
    }
    public void HideHUD()
    {
        UITargetInfoHUD.Instance.HideHUD();
    }
    public void SetDeathEffect(Action<MonsterController> _addEffect)
    {
        if (OnDeath == null || !OnDeath.GetInvocationList().Contains(_addEffect))
            OnDeath += _addEffect;
    }
    void DropGold()
    {
        if(ShouldDrop(monsterData.dropGoldChance))
        {
            int goldAmount = UnityEngine.Random.Range(monsterData.minDropGold, monsterData.maxDropGold + 1);
            AccountManager.Instance.AddGold(goldAmount);
        }
    }
    public void DropItem()
    {
        foreach (var dropItem in monsterData.dropItems)
        {
            if(ShouldDrop(dropItem.itemChance))
            {
                SaveItemData item = new SaveItemData()
                {
                    ItemID = dropItem.ItemID,
                    Quantity = UnityEngine.Random.Range(dropItem.minItemCount, dropItem.maxItemCount + 1)
                };
                InventoryManager.Instance.AddItem(item);
            }
        }
    }
    bool ShouldDrop(float _chance)
    {
        return UnityEngine.Random.Range(0f, 1f) <= _chance;
    }

}
