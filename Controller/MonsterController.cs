using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MonsterStat))]
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
    [SerializeField] float activationDistance = 30f;


    
    IDamageable target;

    public Transform Transform => transform;
    public BuffManager BuffManager => buffManager;


    Transform spawnTrans;

    float checkTimer = 0.2f;

    public Action<int, int> OnHealthChanged; //현재 HP와 최대 HP를 전달
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

        monsterStat.CurrentHP.OnStatChanged += UpdateHealth;
    }
    void GetComponents()
    {
        animator = GetComponent<MonsterAnimator>();
        characterStatus = GetComponent<CharacterStatus>();
        buffManager = GetComponent<BuffManager>();
        agent = GetComponent<NavMeshAgent>();
        objectRenderer = GetComponentInChildren<Renderer>();
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
            if (!IsInView())
            {
                objectRenderer.enabled = false;
                return;
            }
            else
            {
                objectRenderer.enabled = true;
            }
        }



        if (currentState == CharacterState.Stun || currentState == CharacterState.Dead || currentState == CharacterState.Hit)
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
        Debug.Log("Set Monster Data");
    }
    void HandleState()
    {
        switch (currentState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Move:
                Move(target.Transform.position);
                break;
            case CharacterState.Attack:
                HandleAttack();
                break;
            case CharacterState.Return:
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.Hit:
                break;
        }
    }
    public void ChangeCharacterState(CharacterState _changeState)
    {
        if (currentState == CharacterState.Move && _changeState != CharacterState.Move)
        {
            StopMovement(transform.position);
        }
        currentState = _changeState;
        animator.ChangeCharacterState(currentState);
    }

    public void TakeDamage(int _damage, IAttacker _attacker = null)
    {
        if (currentState == CharacterState.Dead)
        {
            return;
        }
        ChangeCharacterState(CharacterState.Hit);
        ApplyDamage(_damage);
        StartCoroutine(HandleTargeting(_attacker, 1f));
        Debug.Log($"{gameObject.name}가 {_attacker}에게 공격당했습니다 : -{_damage}");

    }
    IEnumerator HandleTargeting(IAttacker _attacker, float _stunDuration)
    {
        yield return new WaitForSeconds(_stunDuration);
        if (currentState == CharacterState.Dead)
            yield break;
        if (_attacker is IDamageable damable && !monsterData.IsAggressive && target == null)
        {
            target = damable;
        }
        ChangeCharacterState(CharacterState.Move);
    }
    void ApplyDamage(int _dam)
    {
        _dam = CalculateDamage(_dam);
        monsterStat.CurrentHP.ModifyAllValue(_dam,monsterStat.Health.FinalValue);
        if (monsterStat.CurrentHP.FinalValue <= 0)
        {
            Die();
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
        bodyCollider.enabled = false;
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Attack");
        ChangeCharacterState(CharacterState.Dead);
        StopMovement(transform.position);
        QuestManager.Instance.OnTargetAchieved(QuestTargetType.Monster, ID);
        ObjectPoolManager.Instance.ReturnObject(gameObject, 3);
        Invoke("Init", 3.2f);
        monsterStat.CurrentHP.OnStatChanged -= UpdateHealth;
    }
    public void Move(Vector3 _moveDir)
    {
        animator.ResetTrigger("Attack");
        if (currentState == CharacterState.Attack || currentState == CharacterState.Dead)
            return;

        if (agent.remainingDistance <= monsterData.AttackRangeData.Range)
        {
            ChangeCharacterState(CharacterState.Attack);
        }
        animator.PlayMoveAnimation(true);
        agent.SetDestination(_moveDir);
    }
    public void StopMovement(Vector3 _position)
    {
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
        if (Vector3.Distance(transform.position, target.Transform.position) > monsterData.AttackRangeData.Range)
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
        Debug.Log($"Monster AttackDam {monsterStat.Attack.FinalValue}");
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
        UITargetInfoHUD.Instance.ShowHUD(name, Mathf.RoundToInt(monsterStat.CurrentHP.FinalValue), monsterData.Health,10, this);
    }
    public void HideHUD()
    {
        UITargetInfoHUD.Instance.HideHUD();
    }


}
