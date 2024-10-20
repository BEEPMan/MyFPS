using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stagger : Buff
{
    private Enemy _enemy;
    private Rigidbody _rigidbody;
    private Vector3 _force;

    public Stagger(string buffName, float duration, Vector3 force, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, buffer, buffTarget, OnEndBuffAction)
    {
        _enemy = _buffTarget.gameObject.GetComponent<Enemy>();
        _rigidbody = _buffTarget.GetComponent<Rigidbody>();
        _force = force;
    }

    protected override IEnumerator StartBuff()
    {
        _buffTarget.isStaggerImmune = true;
        _enemy.FSM.ChangeState(new NonState());
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(_force, ForceMode.Impulse);
        yield return _delay;
        //_rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        _enemy.FSM.ChangeState(new PatrolState());
        OnDisableBuff();
    }
}
