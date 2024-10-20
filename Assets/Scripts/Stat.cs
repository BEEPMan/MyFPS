using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SecondaryType
{
    None,
    Shield,
    Armor,
}

public abstract class Stat : MonoBehaviour
{
    public float HP { get { return _HP; } set { _HP = value; } }
    public float MaxHP { get { return _maxHP; } set { _maxHP = value; } }
    public float SA { get { return _SA;  } set { _SA = value; } }
    public float MaxSA { get { return _maxSA; } set { _maxSA = value; } }
    public float Speed { get { return _speed; } set { _speed = value; } }
    public float JumpHeight { get { return _jumpHeight; } set { _jumpHeight = value; } }

    protected BuffSystem _buffSystem;

    public SecondaryType healthType = SecondaryType.Shield;

    protected float _HP;
    protected float _SA;
    [SerializeField]
    protected float _maxHP = 100f;
    [SerializeField]
    protected float _maxSA = 100f;
    [SerializeField]
    protected float _speed = 5f;
    [SerializeField]
    protected float _jumpHeight = 10f;

    protected float _takenDamage;
    protected float _givenDamage;

    public DamageCalculator damageCalculator = new();

    public bool isStaggerImmune = false;
    public bool isInCombat = false;

    protected float immuneTimer;
    protected float combatTimer;

    void Start()
    {
        Init();
    }

    void Update()
    {
        OnUpdate();
    }

    public virtual void Init()
    {
        HP = MaxHP;
        SA = MaxSA;
        _takenDamage = 1f;
        _givenDamage = 1f;
        _buffSystem = GetComponent<BuffSystem>();
        damageCalculator.Init();
    }

    protected virtual void OnUpdate()
    {
        if(isStaggerImmune)
        {
            immuneTimer += Time.deltaTime;
            if(immuneTimer > 5f)
            {
                isStaggerImmune = false;
                immuneTimer = 0f;
            }
        }
        if(isInCombat)
        {
            combatTimer += Time.deltaTime;
            if(combatTimer > 3f)
            {
                isInCombat = false;
                combatTimer = 0f;
            }
        }
    }

    public virtual int TakeDamage(float damage, ElementalType elementalType = ElementalType.None, Stat attacker = null)
    {
        int damageAmount;
        if (healthType != SecondaryType.None && SA > 0f)
        {
            damageAmount = (int)damageCalculator.CalculateTakenDamage(damage, elementalType, healthType);
            SA -= damageAmount;
            SA = Mathf.Clamp(SA, 0, _maxSA);
        }
        else
        {
            damageAmount = (int)damageCalculator.CalculateTakenDamage(damage, elementalType, SecondaryType.None);
            HP -= damageAmount;
            HP = Mathf.Clamp(_HP, 0, _maxHP);
        }
        isInCombat = true;
        combatTimer = 0f;
        return damageAmount;
    }

    public virtual void TakeTrueDamage(float damage, Stat attacker = null)
    {
        if (healthType != SecondaryType.None && SA > 0f)
        {
            SA -= (int)damage;
            SA = Mathf.Clamp(SA, 0, _maxSA);
        }
        else
        {
            HP -= (int)damage;
            HP = Mathf.Clamp(HP, 0, _maxHP);
        }
        isInCombat = true;
        combatTimer = 0f;
    }

    public virtual void RestoreHealth(float healAmount)
    {
        HP += (int)healAmount;
        HP = Mathf.Clamp(HP, 0, _maxHP);
    }

    public abstract void Die();
}
