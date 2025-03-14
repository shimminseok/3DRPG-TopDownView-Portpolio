using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
    public int JobID { get; private set; }

    bool isInteractable;
    IInteractable interactableTarget;


    bool isDragging;
    public JobData JobData { get; private set; }


    public Transform Transform => transform;
    public SkillManager SkillManager => skillManager;
    public BuffManager BuffManager => buffManager;

    public float FinalDam => characterStat.Attack.FinalValue;

    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnMPChanged;
    public event Action OnPlayerDeath;
    public event Action OnPlayerRevive;
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

        agent.Warp(GameManager.Instance.RespawnPoint);

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
    }
    void SetJobData()
    {
        JobID = GameManager.Instance.LoadGameData().JobID;
        JobData = TableLoader.Instance.GetTable<JobTable>()?.GetJobDataByID(JobID);
        GameObject go = Instantiate(JobData.JobPrefabs, transform);
        playerAnimator.animator.avatar = JobData.JobAvatar;
        playerAnimator.animator.runtimeAnimatorController = JobData.Animator;
        skillManager.RegisterSkill(JobData.JobID);
        characterStat.InitializeFromJob(JobData);
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
            StopMovement();
        }
        currentState = _changeState;
        playerAnimator.ChangeCharacterState(currentState);

    }
    #region Movement

    void HandleMove(Vector3 _des)
    {
        if (currentState == CharacterState.Dead)
            return;
        playerAnimator.ResetTrigger("Attack");
        if (currentState == CharacterState.Attack || currentState == CharacterState.Skill)
            return;


        Move(_des);
    }

    public void Move(Vector3 _moveDir)
    {
        playerAnimator.PlayMoveAnimation(true);
        agent.isStopped = false;
        agent.SetDestination(_moveDir);
    }
    public void StopMovement()
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
        if (currentState == CharacterState.Dead)
            return;

        ChangeCharacterState(CharacterState.Attack);
        Quaternion targetRot = Quaternion.LookRotation(_dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * agent.angularSpeed);
        Debug.Log("Attack");
    }

    public void Attack(IDamageable _target)
    {
        _target.TakeDamage(Mathf.RoundToInt(characterStat.Attack.FinalValue), this);
        CameraController.Instance.ShakeCamera(0.2f);
    }
    #endregion
    #region Skill
    void HandleSkill(Vector3 _mousePos, SaveSkillData _data)
    {
        if (currentState == CharacterState.Skill || currentState == CharacterState.Attack || currentState == CharacterState.Dead)
        {
            return;
        }
        UseSkill(_mousePos, _data);
    }
    public void UseSkill(Vector3 _mousePos, SaveSkillData _data)
    {
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
        Quaternion targetRot = Quaternion.LookRotation(_mousePos);
        transform.rotation = targetRot;
        skillManager.ExecuteSkill(_data, gameObject);
        characterStat.CurrentMP.ModifyAllValue(_data.GetSkillData().RequiredMP, characterStat.MP.FinalValue);
    }
    #endregion
    public void HandleInterect()
    {
        if (interactableTarget != null && !UIDescription.Instance.isDialogueRunning)
        {
            interactableTarget.OnInteract();
            StopMovement();
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
        if (currentState == CharacterState.Dead)
            return;

        _damage = CalculateDamage(_damage);
        characterStat.CurrentHP.ModifyAllValue(_damage);
        DamageTextSpawner.Instance.ShowDamage(_damage, transform.position + Vector3.up * 0.5f);
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
        InputHandler.Instance.OnMove -= HandleMove;
        InputHandler.Instance.OnAttack -= HandleAttack;
        InputHandler.Instance.OnSkill -= HandleSkill;


        OnPlayerDeath?.Invoke();
        agent.ResetPath();
        characterStat.ResetModify();
        YesOrNoPopup.Instance.SetMessage("캐릭터가 사망했습니다. 마을에서 부활 하시겠습니까?");
        YesOrNoPopup.Instance.SetYesButton(() =>
        {
            Revive();
        }, "부활");
        YesOrNoPopup.Instance.Open();

    }
    private void Revive()
    {
        InputHandler.Instance.OnMove += HandleMove;
        InputHandler.Instance.OnAttack += HandleAttack;
        InputHandler.Instance.OnSkill += HandleSkill;
        Init();
        agent.Warp(GameManager.Instance.SaveRespawnPoint);

        OnPlayerRevive?.Invoke();
        playerAnimator.animator.SetTrigger("Revive");
        characterStat.InitializeFromJob(JobData);
    }
    void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactableTarget = interactable;
            InputHandler.Instance.OnInteract += HandleInterect;
            UIHUD.Instance.OnInteractHUDUI();

        }
    }
    void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactableTarget = null;
            InputHandler.Instance.OnInteract -= HandleInterect;
            UIHUD.Instance.StopStartInteract();
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
