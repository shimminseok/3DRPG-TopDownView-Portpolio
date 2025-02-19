using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }


    Dictionary<KeyCode, HUDItemSlot> hudItemSlotMapping = new Dictionary<KeyCode, HUDItemSlot>();
    Dictionary<KeyCode,HUDSkillSlot> hudSkillSlotMapping = new Dictionary<KeyCode, HUDSkillSlot>();


    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnAttack;
    public event Action<SaveSkillData> OnSkill;
    public event Action <KeyCode> OnUseItem;

    public event Action OnInteract;
    public Vector3 MovementInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool SkillInput { get; private set; }
    public bool DodgeInput { get; private set; }
    public bool BlockInput { get; private set; }

    public bool IsDragging { get; private set; }

    Camera mainCamera;


    float hoverCheckTime = 0;
    IDisplayable currentTarget;
    Vector3 lastMousePosion;
    SkillManager skillManager;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        mainCamera = Camera.main;
    }
    void Start()
    {
        InitKeyMapping();
        skillManager = PlayerController.Instance.SkillManager;
    }
    void InitKeyMapping()
    {
        KeyCode[] itemHotKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        KeyCode[] skillHotKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };

        for (int i = 0;  i < itemHotKeys.Length; i++)
        {
            hudItemSlotMapping[itemHotKeys[i]] = UIHUD.Instance.GetHUDItemSlot(i);
        }
        for (int i = 0; i < skillHotKeys.Length; i++)
        {
            hudSkillSlotMapping[skillHotKeys[i]] = UIHUD.Instance.GetHUDSkillSlot(i);
        }
    }
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();

        hoverCheckTime += Time.deltaTime;
        if(hoverCheckTime >= 0.1f && Input.mousePosition != lastMousePosion)
        {
            lastMousePosion = Input.mousePosition;
            hoverCheckTime = 0;
            HandleMouseHover();
        }
    }
    void HandleMouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButton(1))
        {

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                OnMove?.Invoke(hit.point); // 이동 이벤트 호출
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,LayerMask.GetMask("Ground")))
            {
                Vector3 targetPos = hit.point;
                Vector3 dir = targetPos - PlayerController.Instance.transform.position;
                dir.y = 0;
                OnAttack?.Invoke(dir);

            }
        }
    }
    private void HandleKeyboardInput()
    {

        HandleHotKeyInput();
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnInteract?.Invoke();
        }


        if(Input.GetKeyDown(KeyCode.K))
        {
            UIManager.Instance.CheckOpenPopup(UISkill.Instance);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.CheckOpenPopup(UIInventory.Instance);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIManager.Instance.CheckOpenPopup(UIQuest.Instance);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            UIManager.Instance.CheckOpenPopup(UICharacterInfo.Instance);
        }
    }
    void HandleHotKeyInput()
    {
        foreach(var key in hudItemSlotMapping.Keys)
        {
            if(Input.GetKeyDown(key))
            {
                OnUseItem?.Invoke(key);
            }
        }
        foreach(var key in hudSkillSlotMapping.Keys)
        {
            if(Input.GetKeyDown(key))
            {
                OnSkill?.Invoke(hudSkillSlotMapping[key].assigendSkill);
            }
        }


    }
    void HandleMouseHover()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,1 << LayerMask.NameToLayer("NPC") | 1 << LayerMask.NameToLayer("Enemy")))
        {
            if (hit.collider.TryGetComponent<IDisplayable>(out IDisplayable displayable))
            {
                if (currentTarget == null || currentTarget != displayable)
                {
                    currentTarget = displayable;
                    displayable.ShowHUD();
                }
                return;

            }
        }
        if(currentTarget != null)
        {
            currentTarget.HideHUD();
            currentTarget = null;
        }
    }

    public void ClearInputs()
    {
        MovementInput = Vector2.zero;
        AttackInput = false;
        SkillInput = false;
        DodgeInput = false;
        BlockInput = false;
    }

    public HUDSkillSlot GetHUDSkillSlot(KeyCode _key)
    {
        return hudSkillSlotMapping[_key];
    }
}
