using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasManager : MonoBehaviour
{
    public static SpriteAtlasManager Instance;

    [SerializeField] SpriteAtlas inventory;
    [SerializeField] SpriteAtlas item;
    [SerializeField] SpriteAtlas skill;
    [SerializeField] SpriteAtlas ui;

    Dictionary<string,SpriteAtlas> atlasDic = new Dictionary<string,SpriteAtlas>();
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        RegisterAtlas("Inventory", inventory);
        RegisterAtlas("Item", item);
        RegisterAtlas("Skill", skill);
        RegisterAtlas("UI", ui);

    }
    public void RegisterAtlas(string _key, SpriteAtlas _atlas)
    {
        if(!atlasDic.ContainsKey(_key))
        {
            atlasDic.Add(_key, _atlas);
        }
    }
    public Sprite GetSprite(string _key, Sprite _name)
    {
        if(atlasDic.TryGetValue(_key ,out var atlas))
        {
            Sprite sprite = atlas.GetSprite(_name.name);
            if(sprite != null)
                return sprite;
        }

        Debug.LogWarning($"Sprite를 찾지 못했습니다. {_name}");
        return null;
    }
    public Sprite GetSprite(string _key, string _name)
    {
        if (atlasDic.TryGetValue(_key, out var atlas))
        {
            Sprite sprite = atlas.GetSprite(_name);
            if (sprite != null)
                return sprite;
        }

        Debug.LogWarning($"Sprite를 찾지 못했습니다. {_name}");
        return null;
    }
}
