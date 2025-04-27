using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : NetworkBehaviour
{
    public StatData Data;

    #region Stat
    public EnumTypes.HPType HPType { get; set; }
    public NetworkVariable<int> HP = new();
    public NetworkVariable<int> MaxHP = new();
    public NetworkVariable<int> Shield = new();
    public NetworkVariable<int> MaxShield = new();
    public NetworkVariable<int> Armor = new();
    public NetworkVariable<int> MaxArmor = new();

    public float Speed { get; set; }
    public int SpeedFactor { get; set; }

    public int WeaponDamage { get; set; }
    public int SkillDamage { get; set; }
    public int TakenDamage { get; set; }
    public int[] ElementalDamage { get; set; }
    public float FinalDamage { get; set; }
    #endregion

    public BuffManager BuffManager;

    protected virtual void InitStat()
    {
        HPType = Data.HPType;
        if (NetworkManager.Singleton.IsServer)
        {
            HP.Value = Data.HP;
            MaxHP.Value = Data.HP;
            switch (HPType)
            {
                case EnumTypes.HPType.Shield:
                    Shield.Value = Data.Shield;
                    MaxShield.Value = Data.Shield;
                    break;
                case EnumTypes.HPType.Armor:
                    Armor.Value = Data.Armor;
                    MaxArmor.Value = Data.Armor;
                    break;
            }
        }
        Speed = Data.Speed;
        SpeedFactor = 0;
        WeaponDamage = 0;
        SkillDamage = 0;
        TakenDamage = 0;
        for (int i=0;i<ElementalDamage.Length; i++)
        {
            ElementalDamage[i] = 0;
        }
        FinalDamage = 1f;
        BuffManager.Init();
    }

    public void Attack()
    {
        AttackServerRPC();
    }

    [Rpc(SendTo.Server)]
    protected virtual void AttackServerRPC()
    {
        AttackClientRPC();
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected virtual void AttackClientRPC()
    {
        if (!IsHost)
        {
            // Attack Logic
        }
    }

    public abstract void TakeDamage(int damage, EnumTypes.ElementType elementType = EnumTypes.ElementType.None, bool isTrueDamage = false);

    public abstract void Die();

    public virtual void RestoreHealth(int healAmount)
    {
        HP.Value += healAmount;
    }

    public virtual void AddBuff(BaseBuff newBuff)
    {
        BuffManager.AddBuff(newBuff);
    }

    public int CalcDamageByHPType(EnumTypes.HPType hPType, EnumTypes.ElementType elementType)
    {
        switch (hPType)
        {
            case EnumTypes.HPType.HPOnly:
                if (elementType == EnumTypes.ElementType.Fire) return 50;
                else if (elementType == EnumTypes.ElementType.Corrosion) return -25;
                else if (elementType == EnumTypes.ElementType.Lightning) return -25;
                else return 0;
            case EnumTypes.HPType.Shield:
                if (elementType == EnumTypes.ElementType.Fire) return -25;
                else if (elementType == EnumTypes.ElementType.Corrosion) return -25;
                else if (elementType == EnumTypes.ElementType.Lightning) return 50;
                else return 0;
            case EnumTypes.HPType.Armor:
                if (elementType == EnumTypes.ElementType.Fire) return -25;
                else if (elementType == EnumTypes.ElementType.Corrosion) return 50;
                else if (elementType == EnumTypes.ElementType.Lightning) return -25;
                else return 0;
            default:
                return 0;
        }
    }

    public void TriggerElementalEffect(EnumTypes.ElementType elementType, int damage)
    {
        if (elementType == EnumTypes.ElementType.None) return;
        switch (elementType)
        {
            case EnumTypes.ElementType.Fire:
                BuffManager.AddBuff(new Burning(this, 5.0f, (int)(damage * 0.2f)));
                break;
            case EnumTypes.ElementType.Lightning:
                BuffManager.AddBuff(new Shock(this, 5.0f, 10));
                break;
            case EnumTypes.ElementType.Corrosion:
                BuffManager.AddBuff(new Decay(this, 5.0f, 50));
                break;
        }
    }
}
