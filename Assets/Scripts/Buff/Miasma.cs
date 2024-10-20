using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Miasma : Damagable
{
    protected float damageTimer;

    public Miasma(string buffName, float duration, float damage, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, damage, buffer, buffTarget, OnEndBuffAction)
    {
        _isStackable = true;
        _maxStack = 9;
    }

    protected override IEnumerator StartBuff()
    {
        for (int i = 0; i < _duration / _damageInterval; i++)
        {
            _buffTarget.TakeTrueDamage(_damage);
            yield return _delay;
        }
        _buffTarget.TakeTrueDamage(_damage);
        OnDisableBuff();
    }
}
