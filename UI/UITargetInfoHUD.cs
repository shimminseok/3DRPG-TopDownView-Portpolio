using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITargetInfoHUD : MonoBehaviour
{
    public static UITargetInfoHUD Instance;

    [SerializeField] GameObject HUD;
    [SerializeField] TextMeshProUGUI targetName;
    [SerializeField] TextMeshProUGUI targetHP;
    [SerializeField] TextMeshProUGUI targetLevel;
    [SerializeField] Image targetHPFill;

    [SerializeField] Transform targetBuffIconRoot;
    [SerializeField] HUDBuffIconSlot targetBuffIconSlot;
    Dictionary<Buff, GameObject> activeBuffSlot = new Dictionary<Buff, GameObject>();


    MonsterController currentMonsterCtrl;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        HideHUD();
    }
    public void ShowHUD(string _name, int _curHP, int _maxHP, int _lv, MonsterController _monCtrl = null)
    {
        if (HUD.activeSelf && _monCtrl == currentMonsterCtrl)
            return;



        //���� ���� ��Ʈ�ѷ��� ���� �Ǿ��� ��츸 ó��
        //TO DO
        // HUD���Ͱ� �ٲ������ Ȯ���ϴ� �۾� �ؾ���.
        if (currentMonsterCtrl != null)
            UnSubscribeeMonsterEvents(currentMonsterCtrl);

        currentMonsterCtrl = _monCtrl;
        HUD.SetActive(true);
        targetName.text = _name;

        currentMonsterCtrl = _monCtrl;
        if (currentMonsterCtrl != null)
        {
            SubscribeMonsterEvents(currentMonsterCtrl);
        }

        // HUD ������Ʈ
        if (currentMonsterCtrl != null)
        {
            UpdateHUD(_curHP, _maxHP);
            targetLevel.text = $"Lv.{_lv}";
        }
    }
    public void HideHUD()
    {
        if (!HUD.activeSelf)
            return;
        HUD.SetActive(false);
        ResetHUD();

        if (currentMonsterCtrl != null)
        {
            UnSubscribeeMonsterEvents(currentMonsterCtrl);
            currentMonsterCtrl = null;
        }
    }
    public void UpdateHUD(int _curHP, int _maxHP)
    {
        if (!HUD.activeSelf || currentMonsterCtrl == null) return; // �ʿ� ������ ���� �� ��

        targetHP.text = $"{_curHP} / {_maxHP}";
        targetHPFill.fillAmount = (float)_curHP / _maxHP;
    }

    void ResetHUD()
    {
        targetName.text = string.Empty;
        targetHP.text = string.Empty;
        targetLevel.text = string.Empty;
        targetHPFill.fillAmount = 1f;

    }

    void AddBuffIcon(Buff _buff)
    {
        if (activeBuffSlot.ContainsKey(_buff))
        {
            return;
        }

        HUDBuffIconSlot slot = Instantiate(targetBuffIconSlot, targetBuffIconRoot);
        slot.SetBuffIcon(_buff);
        activeBuffSlot[_buff] = slot.gameObject;
    }
    void RemoveBuffIcon(Buff _buff)
    {
        if (activeBuffSlot.TryGetValue(_buff, out var slot))
        {
            Destroy(slot);
            activeBuffSlot.Remove(_buff);
        }
    }
    void RemoveAllBuffIcon()
    {
        foreach (var buff in activeBuffSlot.Values)
        {
            Destroy(buff);
        }
        activeBuffSlot.Clear();
    }

    void SubscribeMonsterEvents(MonsterController _mon)
    {
        _mon.OnHealthChanged += UpdateHUD;
        _mon.BuffManager.OnBuffAdded += AddBuffIcon;
        _mon.BuffManager.OnBuffRemoved += RemoveBuffIcon;
    }
    void UnSubscribeeMonsterEvents(MonsterController _mon)
    {
        if (_mon != null)
        {
            _mon.OnHealthChanged -= UpdateHUD;
            _mon.BuffManager.OnBuffAdded -= AddBuffIcon;
            _mon.BuffManager.OnBuffRemoved -= RemoveBuffIcon;
        }
    }
}
