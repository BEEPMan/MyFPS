using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Buff
{
    public bool Stackable { get { return _isStackable; } }
    public int MaxStack { get { return _maxStack; } }
    public IEnumerator Coroutine { get { return _coroutine; } }

    protected string _buffName;
    protected float _duration;
    protected Stat _buffer;
    protected EnemyStat _buffTarget;
    protected WaitForSeconds _delay;
    protected IEnumerator _coroutine;
    protected bool _isStackable;
    protected int _maxStack;

    public event Action<string> RemoveBuff;
    public event Action<string> OnEndBuff;

    public Buff(string buffName, float duration, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction)
    {
        _delay = new WaitForSeconds(duration);
        _buffName = buffName;
        _duration = duration;
        _buffer = buffer;
        _buffTarget = buffTarget;
        OnEndBuff += OnEndBuffAction;
        _coroutine = StartBuff();
    }

    public void OnDisableBuff()
    {
        OnEndBuff?.Invoke(_buffName);
        RemoveBuff.Invoke(_buffName);
    }

    protected abstract IEnumerator StartBuff();
}
