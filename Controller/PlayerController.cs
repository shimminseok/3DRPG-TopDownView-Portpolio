using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public enum CharacterState
{
    Idle,
    Move,
    Attack,
    Dodge,
    Defend,
    Skill,
    Dead,
    Stun,
    Return,
    Hit
}
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerAniEventListener))]
[RequireComponent(typeof(BuffManager))]
[RequireComponent(typeof(CharacterStatus))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerStat))]
public class PlayerController : CharacterControllerBase, IMoveable, IAttacker, IDamageable, ISkillCaseter
{
    public static PlayerController Instance;



    [Header("Componrnts")]
    NavMeshAgent agent;
    PlayerAnimator playerAnimator;
    BuffManager buffManager;
    CharacterStatus characterStatus;
    SkillManager skillManager;
    [Header("MovementStat")]
    public float dodgeSpeed = 10f;

    [Header("State")]
    float dodgeDuration = 1f;
    float dodgeTimer;

    [Header("Stat")]
    public PlayerStat characterStat = new PlayerStat();

    //임시
    public GameObject target;
    public int jobID;
    bool isInteractable;
    IInteractable interactableTarget;


    bool isDragging;
    public JobData jobData;

    public Action<int, int> OnHealthChanged;
    public Action<int, int> OnMPChanged;

    public Transform Transform => transform;
    public SkillManager SkillManager => skillManager;
    public BuffManager BuffManager => buffManager;
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        GetComponents();


        InputHandler.Instance.OnMove += HandleMove;
        InputHandler.Instance.OnAttack += HandleAttack;
        InputHandler.Instance.OnSkill += HandleSkill;
        characterStat.CurrentHP.OnStatChanged += UpdateHealth;
        characterStat.CurrentMP.OnStatChanged += UpdateMP;
        characterStat.MoveSpd.OnStatChanged += UpdateMovementSpeed;
        characterStat.AttackSpd.OnStatChanged += UpdateAttackSpeed;

    }
    void Start()
    {
        SetJobData();
        Init();
    }

    void Update()
    {
        if (currentState == CharacterState.Dead || currentState == CharacterState.Stun)
            return;

        UpdatePlayerState();

    }
    void GetComponents()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<PlayerAnimator>();
        buffManager = GetComponent<BuffManager>();
        characterStatus = GetComponent<CharacterStatus>();
        agent = GetComponent<NavMeshAgent>();
        if (this is ISkillCaseter && !TryGetComponent(out skillManager))
        {
            skillManager = gameObject.AddComponent<SkillManager>();
        }
    }
    protected override void Init()
    {
        base.Init();
        transform.localPosition = GameManager.Instance.RespawnPoint.localPosition;
    }
    void SetJobData()
    {
        jobData = TableLoader.Instance.GetTable<JobTable>()?.GetJobDataByID(jobID);
        GameObject go = Instantiate(jobData.JobPrefabs, transform);
        playerAnimator.animator.avatar = jobData.JobAvatar;
        playerAnimator.animator.runtimeAnimatorController = jobData.Animator;
        skillManager.RegisterSkill(jobData.JobID);
        characterStat.InitializeFromJob(jobData);
    }
    void UpdatePlayerState()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            ChangeCharacterState(CharacterState.Move);
        }
        else if (currentState == CharacterState.Move)
        {
            ChangeCharacterState(CharacterState.Idle);
        }
        else if (currentState == CharacterState.Attack)
        {
            AnimatorStateInfo animatorStateInfo = playerAnimator.animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsTag("Attack") && animatorStateInfo.normalizedTime >= 0.9f)
            {
                playerAnimator.ResetTrigger("Attack");
                ChangeCharacterState(CharacterState.Idle);
            }
        }
    }

    public void ChangeCharacterState(CharacterState _changeState)
    {
        if (currentState == CharacterState.Move && _changeState != CharacterState.Move)
        {
            StopMovement(transform.position);
        }
        currentState = _changeState;
        playerAnimator.ChangeCharacterState(currentState);

    }
    #region Movement

    void HandleMove(Vector3 _des)
    {
        playerAnimator.ResetTrigger("Attack");
        if (currentState == CharacterState.Attack || currentState == CharacterState.Skill)
            return;

        if ((_des == Vector3.zero))
        {
            StopMovement(transform.position);
        }
        else
        {
            Move(_des);
        }
    }

    public void Move(Vector3 _moveDir)
    {
        playerAnimator.PlayMoveAnimation(true);
        agent.isStopped = false;
        agent.SetDestination(_moveDir);
    }
    public void StopMovement(Vector3 _position)
    {
        agent.isStopped = true;
        agent.ResetPath();
        playerAnimator.PlayMoveAnimation(false);
    }
    #endregion

    #region Attack
    /// <summary>
    /// Attack Handler
    /// </summary>
    /// <param name="_dir">클릭한 방향</param>
    void HandleAttack(Vector3 _dir)
    {
        ChangeCharacterState(CharacterState.Attack);
        Quaternion targetRot = Quaternion.LookRotation(_dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * agent.angularSpeed);
        Debug.Log("Attack");
    }

    public void Attack(IDamageable _target)
    {
        _target.TakeDamage(Mathf.RoundToInt(characterStat.Attack.FinalValue), this);
    }
    #endregion
    #region Skill
    void HandleSkill(SaveSkillData _data)
    {
        if (currentState == CharacterState.Skill || currentState == CharacterState.Attack)
        {
            return;
        }
        UseSkill(_data);
    }
    public void UseSkill(SaveSkillData _data)
    {
        //범위 내의 타겟을 검사
        //List<GameObject> targets = new List<GameObject>();

        //스킬 범위 가져오기
        if (_data == null)
        {
            Debug.Log($"존재 하지 않는 스킬 {_data}");
            return;
        }
        if (characterStat.CurrentMP.FinalValue < _data.GetSkillData().RequiredMP)
        {
            Debug.Log("MP가 부족합니다.");
            return;
        }
        if (skillManager.CanUseSkill(_data.SkillID))
        {
            Debug.Log("아직은 사용 할 수 없습니다.");
            return;
        }
        ChangeCharacterState(CharacterState.Skill);
        skillManager.ExecuteSkill(_data, gameObject);
        characterStat.CurrentMP.ModifyAllValue(_data.GetSkillData().RequiredMP, characterStat.MP.FinalValue);
    }
    public bool CanUseSkill(int _skillID)
    {
        bool result = false;
        //마나 및 쿨타임

        return result;
    }
    #endregion
    public void HandleInterect()
    {
        if (interactableTarget != null && !UIDescription.Instance.isDialogueRunning)
        {
            interactableTarget.OnInteract();
        }
    }
    #region Dodge
    void StartDodge()
    {
        if (currentState == CharacterState.Skill)
            return;

        ChangeCharacterState(CharacterState.Dodge);
        dodgeTimer = dodgeDuration;
    }

    void HandleDodge()
    {
        dodgeTimer -= Time.deltaTime;
        characterController.Move(transform.forward * dodgeSpeed * Time.deltaTime);

        if (dodgeTimer <= 0)
        {
            ChangeCharacterState(CharacterState.Idle);
        }
    }
    #endregion

    #region Block
    //캐릭터 변경으로인한 막기 삭제
    //void StartBlock()
    //{
    //    if (currentState == PlayerState.Skill || currentState == PlayerState.Dodge)
    //        return;

    //    ChangeCharacterState(PlayerState.Defend);
    //}
    //void Defending()
    //{
    //    Debug.Log("공격을 막았습니다.");
    //    playerAnimator.PlayDefandingAttack();
    //}
    //void HandleBlock()
    //{
    //    if (!inputHandler.BlockInput)
    //    {
    //        ChangeCharacterState(PlayerState.Idle);
    //    }
    //}
    #endregion
    public void TakeDamage(int _damage, IAttacker _attacker = null)
    {
        _damage = CalculateDamage(_damage);
        Debug.Log($"플레이어가 {_attacker}에게 공격당했습니다 : -{_damage}");
        characterStat.CurrentHP.ModifyAllValue(_damage, characterStat.Health.FinalValue);

        if (characterStat.CurrentHP.FinalValue <= 0)
        {
            Die();
        }
    }
    public int CalculateDamage(int _dam)
    {
        float damageReduction = characterStat.Defense.FinalValue / (characterStat.Defense.FinalValue + 100);
        int finalDam = Mathf.RoundToInt(_dam * (1 - damageReduction));
        return Mathf.Max(1, finalDam);
    }
    #region[EventHandle]
    void UpdateHealth(float _newValue)
    {
        OnHealthChanged?.Invoke(Mathf.RoundToInt(_newValue), Mathf.RoundToInt(characterStat.Health.FinalValue));
    }
    void UpdateMP(float _newValue)
    {
        OnMPChanged?.Invoke(Mathf.RoundToInt(_newValue), Mathf.RoundToInt(characterStat.MP.FinalValue));
    }
    void UpdateMovementSpeed(float _newValue)
    {
        agent.speed = _newValue * 10;
        playerAnimator.animator.SetFloat("MoveSpeed", _newValue);
    }
    void UpdateAttackSpeed(float _newValue)
    {
        playerAnimator.animator.SetFloat("AttackSpeed", _newValue);
    }
    #endregion
    public void Die()
    {
        ChangeCharacterState(CharacterState.Dead);
        characterController.enabled = false;
        characterStat.ResetModify();
    }
    void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactableTarget = interactable;
            InputHandler.Instance.OnInteract += HandleInterect;
        }
    }
    void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactableTarget = null;
            InputHandler.Instance.OnInteract -= HandleInterect;
        }
    }

    private void OnDestroy()
    {
        InputHandler.Instance.OnMove -= HandleMove;
        InputHandler.Instance.OnAttack -= HandleAttack;
        InputHandler.Instance.OnSkill -= HandleSkill;
        characterStat.CurrentHP.OnStatChanged -= UpdateHealth;
        characterStat.CurrentMP.OnStatChanged -= UpdateMP;
        characterStat.MoveSpd.OnStatChanged -= UpdateMovementSpeed;
        characterStat.AttackSpd.OnStatChanged -= UpdateAttackSpeed;
    }
}
