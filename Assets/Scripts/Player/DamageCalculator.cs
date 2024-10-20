using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    public float WeaponDamage { get { return _weaponDamage; } set { _weaponDamage = value; } }
    public float CritDamage { get { return _critDamage; } set { _critDamage = value; } }
    public float SkillDamage { get { return _skillDamage; } set { _skillDamage = value; } }

    private float _givenDamage;
    private float _takenDamage;

    private float _weaponDamage;
    private float _critDamage;
    private float _skillDamage;
    private float[] _elementalDamage;

    public void Init()
    {
        _givenDamage = 1f;
        _takenDamage = 1f;
        _weaponDamage = 1f;
        _critDamage = 1f;
        _skillDamage = 1f;
        _elementalDamage = new float[4];
        for (int i = 0; i < 4; i++)
            _elementalDamage[i] = 1f;
    }

    public float CalculateGivenDamage(float damage, ElementalType damageType, bool isWeapon, bool isCrit)
    {
        if (isWeapon)
        {
            if (isCrit)
                return damage * _givenDamage * _weaponDamage * _critDamage * _elementalDamage[(int)damageType];
            else
                return damage * _givenDamage * _weaponDamage * _elementalDamage[(int)damageType];
        }
        else
            return damage * _givenDamage * _skillDamage * _elementalDamage[(int)damageType];
    }

    public float CalculateTakenDamage(float damage, ElementalType damageType, SecondaryType secondaryType)
    {
        if(secondaryType == SecondaryType.Armor)
        {
            if (damageType == ElementalType.Corrosion)
                return damage * _takenDamage * 1.5f;
            else
                return damage * _takenDamage * 0.75f;
        }
        else if(secondaryType == SecondaryType.Shield)
        {
            if (damageType == ElementalType.Lightning)
                return damage * _takenDamage * 1.5f;
            else if (damageType == ElementalType.Corrosion || damageType == ElementalType.Fire)
                return damage * _takenDamage * 0.75f;
        }
        return damage * _takenDamage;
    }

    public void MulGivenDamage(float amount)
    {
        _givenDamage *= (1f + amount);
    }

    public void DivGivenDamage(float amount)
    {
        _givenDamage /= (1f + amount);
    }

    public void MulTakenDamage(float amount)
    {
        _takenDamage *= (1f + amount);
    }

    public void DivTakenDamage(float amount)
    {
        _takenDamage /= (1f + amount);
    }

    public void AddElementalDamage(ElementalType type, float amount)
    {
        _elementalDamage[(int)type] += amount;
    }
}
