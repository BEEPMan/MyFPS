using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explosion : Damagable
{
    private Collider[] _enemies;

    public Explosion(string buffName, float duration, float damage, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, damage, buffer, buffTarget, OnEndBuffAction)
    {
    }

    protected override IEnumerator StartBuff()
    {
        _enemies = Physics.OverlapSphere(_buffTarget.transform.position, 5.0f, 1 << LayerMask.NameToLayer("Enemy"));
        _buffTarget.TakeDamage(_damage);
        if(!_buffTarget.isStaggerImmune)
            _buffTarget.AddBuff(_buffer, "Stagger", 1f, 0f, (_buffTarget.transform.position - _buffer.transform.position).normalized);
        foreach (var enemy in _enemies)
        {
            EnemyStat stat = enemy.GetComponent<EnemyStat>();
            if (stat.GetInstanceID() == _buffTarget.GetInstanceID()) continue;
            stat.TakeDamage(_damage * 0.5f);
            if (!stat.isStaggerImmune)
            {
                Vector3 force = enemy.transform.position - _buffTarget.transform.position;
                force.y = 0f;
                if (!stat.isStaggerImmune)
                    stat.AddBuff(_buffer, "Stagger", 1f, 0f, force);
            }
        }
        yield return _delay;
        OnDisableBuff();
    }
}
