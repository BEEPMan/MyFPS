using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    public List<Scroll> scrolls;

    public override void Init()
    {
        base.Init();
        scrolls = new List<Scroll>();
    }

    public override int TakeDamage(float damage, ElementalType elementalType = ElementalType.None, Stat attacker = null)
    {
        int damageAmount;
        if (healthType != SecondaryType.None && SA > 0f)
        {
            damageAmount = (int)damageCalculator.CalculateTakenDamage(damage, elementalType, healthType);
            SA -= damageAmount;
            SA = Mathf.Clamp(SA, 0, MaxSA);
            Player.Instance.UI.SetSABar(SA, MaxSA);
        }
        else
        {
            damageAmount = (int)damageCalculator.CalculateTakenDamage(damage, elementalType, SecondaryType.None);
            HP -= damageAmount;
            HP = Mathf.Clamp(HP, 0, MaxHP);
            Player.Instance.UI.SetHPBar(HP, MaxHP);
        }
        return damageAmount;
    }

    public void TakeDamage(float damage)
    {
        if (healthType != SecondaryType.None && SA > 0f)
        {
            SA -= (int)(damage * _takenDamage);
            SA = Mathf.Clamp(SA, 0, MaxSA);
            Player.Instance.UI.SetSABar(SA, MaxSA);
        }
        else
        {
            HP -= (int)(damage * _takenDamage);
            HP = Mathf.Clamp(HP, 0, MaxHP);
            Player.Instance.UI.SetHPBar(HP, MaxHP);
        }
    }

    public override void RestoreHealth(float healAmount)
    {
        base.RestoreHealth(healAmount);
        Player.Instance.UI.SetHPBar(HP, MaxHP);
    }

    public void GetScroll(Scroll scroll)
    {
        scrolls.Add(scroll);
        damageCalculator.MulGivenDamage(scroll.givenDamage / 100f);
        damageCalculator.MulTakenDamage(scroll.takenDamage / 100f);
        damageCalculator.WeaponDamage += scroll.weaponDamage / 100f;
        damageCalculator.CritDamage += scroll.critDamage / 100f;
        damageCalculator.SkillDamage += scroll.skillDamage / 100f;
        for (int i = 0; i < 4; i++)
        {
            damageCalculator.AddElementalDamage((ElementalType)i, scroll.elementalDamage[i] / 100f);
        }
    }

    public void DelScroll(Scroll scroll)
    {
        scrolls.Remove(scroll);
        damageCalculator.DivGivenDamage(scroll.givenDamage / 100f);
        damageCalculator.DivTakenDamage(scroll.takenDamage / 100f);
        damageCalculator.WeaponDamage -= scroll.weaponDamage / 100f;
        damageCalculator.CritDamage -= scroll.critDamage / 100f;
        damageCalculator.SkillDamage -= scroll.skillDamage / 100f;
        for (int i = 0; i < 4; i++)
        {
            damageCalculator.AddElementalDamage((ElementalType)i, -scroll.elementalDamage[i] / 100f);
        }
    }

    public override void Die()
    {
        
    }
}
