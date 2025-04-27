using UnityEngine;

public enum EffectTriggerType
{
    OnEquip,
    OnAttack,
    OnHit,
    OnKill,
    OnTakeDamage,
}

public enum EffectActionType
{
    ModifyHP,
    ModifySpeed,
    ModifyWeaponDamage,
    ModifySkillDamage,
    ModifyTakenDamage,
    ModifyElementalDamage,
    ModifyFinalDamage,
    ApplyBuff,
}

[System.Serializable]
public class TriggeredEffect
{
    // Naming convention: itemName + _ + actionType (ex. "Healthkit_ModifyHP")
    public string name;
    public EffectTriggerType trigger;
    public EffectActionType actionType;

    // ModifyElementalDamage
    [HideInInspector] public EnumTypes.ElementType elementType;

    public int value;
    
    public float duration;
    public float cooldown;
    public int maxStacks = 0;
    public string condition;

    [HideInInspector] public float lastTriggeredTime;
    [HideInInspector] public int currentStacks;
}