using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHUD : MonoBehaviour
{
    public static UIHUD Instance;

    [Header("Bottom")]
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBar;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;

    [SerializeField] List<HUDSkillSlot> hudSkillSlots = new List<HUDSkillSlot>();
    [SerializeField] List<HUDItemSlot> hudItemSlots = new List<HUDItemSlot>();

    [SerializeField] Transform buffEffectIconRoot;
    [SerializeField] HUDBuffIconSlot buffSlotIconPrefabs;


    Dictionary<Buff,GameObject> activeBuffSlot = new Dictionary<Buff,GameObject>();
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        PlayerController.Instance.SkillManager.OnSkillUsed += HandleSkillUsed;
        PlayerController.Instance.OnHealthChanged += UpdateHPUI;
        PlayerController.Instance.OnMPChanged += UpdateMPUI;

        PlayerController.Instance.BuffManager.OnBuffAdded += AddBuffIcon;
        PlayerController.Instance.BuffManager.OnBuffRemoved += RemoveBuffIcon;
    }
    public void UpdateHPUI(int _current, int _max)
    {
        hpText.text = $"{_current} / {_max}";
        hpBar.fillAmount = (float)_current / _max;
    }
    public void UpdateMPUI(int _current, int _max)
    {
        mpText.text = $"{_current} / {_max}";
        mpBar.fillAmount = (float)_current / _max;
    }

    public void HandleSkillUsed(KeyCode _hotkey, float _coolTime)
    {
        hudSkillSlots.Find(x => x.slotHotKey == _hotkey).StartCoolTime(_coolTime);
    }
    void AddBuffIcon(Buff _buff)
    {
        HUDBuffIconSlot slot = Instantiate(buffSlotIconPrefabs, buffEffectIconRoot);
        slot.SetBuffIcon(_buff);
        activeBuffSlot[_buff] = slot.gameObject;
    }
    void RemoveBuffIcon(Buff _buff)
    {
        if(activeBuffSlot.TryGetValue(_buff, out var slot))
        {
            Destroy(slot);
            activeBuffSlot.Remove(_buff);
        }
    }

    public HUDItemSlot GetHUDItemSlot(int _index)
    {
        return hudItemSlots[_index];
    }
    public HUDSkillSlot GetHUDSkillSlot(int _index)
    {
        return hudSkillSlots[_index];
    }

}
