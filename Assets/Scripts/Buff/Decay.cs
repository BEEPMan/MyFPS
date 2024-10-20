using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Decay : Buff
{
    private float slowAmount;

    public Decay(string buffName, float duration, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, buffer, buffTarget, OnEndBuffAction)
    {
        _delay = new WaitForSeconds(duration);
    }

    protected override IEnumerator StartBuff()
    {
        if (_buffTarget.FindBuff("Burning"))
        {
            _buffTarget.AddBuff(_buffer, "Explosion", 5.0f, 5.0f);
        }
        if (_buffTarget.FindBuff("Shock"))
        {
            _buffTarget.AddBuff(_buffer, "Miasma", 5.0f ,_buffTarget.MaxHP * 0.09f);
        }
        slowAmount = _buffTarget.Speed * 0.5f;
        _buffTarget.Speed -= slowAmount;
        yield return _delay;
        _buffTarget.Speed += slowAmount;
        OnDisableBuff();
    }
}
