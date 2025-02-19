using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;


    public Dictionary<string, Queue<GameObject>> poolObjects = new Dictionary<string, Queue<GameObject>>();

    Dictionary<string, GameObject> registeredObj = new Dictionary<string, GameObject>();

    Dictionary<string,Transform> parentCache = new Dictionary<string, Transform>();

    Transform poolGroup;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

    }
    /// <summary>
    /// ������Ʈ Ǯ �ʱ�ȭ
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_prefab"></param>
    /// <param name="_poolSize"></param>
    public void CreatePool(GameObject _prefab, int _poolSize)
    {
        if (poolObjects.ContainsKey(_prefab.name))
        {
            Debug.LogWarning($"�̹� �����ϴ� Ǯ : {_prefab.name}");
            return;
        }

        Queue<GameObject> newPool = new Queue<GameObject>();
        GameObject parentObj = new GameObject(_prefab.name) { transform = { parent = transform } };
        parentCache[_prefab.name] = parentObj.transform;

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_prefab, parentObj.transform);
            obj.name = _prefab.name;
            obj.SetActive(false);
            newPool.Enqueue(obj);
        }
        poolObjects[_prefab.name] = newPool;
        registeredObj[_prefab.name] = _prefab;
    }
    /// <summary>
    /// Ǯ���� ������Ʈ�� �������� �Լ�
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public GameObject GetObject(string _name)
    {
        if (!poolObjects.ContainsKey(_name))
        {
            Debug.LogWarning($"��ϵ� Ǯ�� �����ϴ� : {name}");
            return null;
        }

        Queue<GameObject> pool = poolObjects[_name];
        if (pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject prefab = registeredObj[_name];
            GameObject newObj = Instantiate(prefab);
            newObj.name = _name;
            newObj.transform.SetParent(parentCache[_name]);
            newObj.SetActive(true);
            return newObj;
        }
    }
    /// <summary>
    /// ����� ������Ʈ�� Ǯ�� �ݳ��ϴ� �Լ�
    /// </summary>
    /// <param name="_obj"></param>
    IEnumerator DelayedReturnObject(GameObject _obj, float _returnTime)
    {
        if (!poolObjects.ContainsKey(_obj.name))
        {
            Debug.LogWarning($"��ϵ� Ǯ�� �����ϴ� : {_obj.name}");
            yield return null;
        }
        yield return new WaitForSeconds(_returnTime);
        _obj.SetActive(false);
        poolObjects[_obj.name].Enqueue(_obj);
    }
    public void ReturnObject(GameObject _obj, float _returnTime = 0)
    {
        StartCoroutine(DelayedReturnObject(_obj, _returnTime));
    }
    public void RemovePool(string _name)
    {
        parentCache.Remove(_name);
        poolObjects.Remove(_name);
        registeredObj.Remove(_name);
    }

}
