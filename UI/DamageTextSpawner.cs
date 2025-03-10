using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }
    // Start is called before the first frame update
    [SerializeField] RectTransform Canvas;
    [SerializeField] DamageText damageText;
    [SerializeField] CinemachineBrain mainCam;
    [SerializeField] Vector3 offset;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ObjectPoolManager.Instance.CreatePool(damageText.gameObject, 30);
    }
    public void ShowDamage(int _damage, Vector3 _worldPos)
    {
        if(ObjectPoolManager.Instance.GetObject("DamageText").TryGetComponent<DamageText>(out var damageTextObj))
        {
            damageTextObj.transform.SetParent(Canvas.transform, false);

            Vector3 screenPos = mainCam.OutputCamera.WorldToScreenPoint(_worldPos + offset); 
            RectTransform damageRect = damageTextObj.RectTransform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas, screenPos, mainCam.OutputCamera, out localPoint);
            damageRect.anchoredPosition = localPoint;
            damageTextObj.ShowDamage(_damage, _worldPos);
        }

    }
}
