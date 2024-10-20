using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damagable : Buff
{
    protected float _damage;
    protected float _damageInterval = 1f;

    public Damagable(string buffName, float duration, float damage, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, buffer, buffTarget, OnEndBuffAction)
    {
        _damage = damage;
        _delay = new WaitForSeconds(_damageInterval);
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    protected override IEnumerator StartBuff()
    {
        yield return _delay;
        OnDisableBuff();
    }
}
