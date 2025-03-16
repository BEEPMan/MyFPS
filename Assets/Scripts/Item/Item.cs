using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item", order = 1)]
public class Item : ScriptableObject
{
    [SerializeField] string id;
    public string ID { get { return id; } }
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public int Price;

    public List<ItemEffect> GainEffects;

    #if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
    #endif

    public virtual void Gain(PlayerController player)
    {
        foreach(ItemEffect effect in GainEffects)
        {
            effect.ExecuteEffect(this, player);
        }
    }
}
