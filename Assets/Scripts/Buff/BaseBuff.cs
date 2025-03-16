using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class BaseBuff
{
    public string BuffName { get; protected set; }
    public float Duration { get; set; }
    public bool IsStackable { get; protected set; }
    public bool IsExpired { get; protected set; } = false;

    public float ElapsedTime;

    public BaseBuff(string buffName, float duration, bool isStackable = false)
    {
        BuffName = buffName;
        Duration = duration;
        IsStackable = isStackable;
        ElapsedTime = 0f;
    }

    public abstract void ApplyEffect();

    public virtual void UpdateBuff(float deltaTime)
    {
        ElapsedTime += deltaTime;
        if(ElapsedTime >= Duration)
        {
            IsExpired = true;
        }
    }

    public abstract void OnExpire();
}

public class Burning : BaseBuff
{
    private BaseController _target;

    public int Damage { get; protected set; }
    private float nextTick;

    public Burning(BaseController target, float duration, int damage) : base("Burning", duration, false)
    {
        _target = target;
        Damage = damage;
    }

    public override void ApplyEffect()
    {
        nextTick = 1f;
    }

    public override void UpdateBuff(float deltaTime)
    {
        base.UpdateBuff(deltaTime);
        if(ElapsedTime >= nextTick)
        {
            _target.TakeDamage(Damage);
            nextTick += 1f;
        }
    }

    public override void OnExpire()
    {
        _target.TakeDamage(Damage);
    }
}

public class Decay : BaseBuff
{
    private BaseController _target;

    public int SlowAmount { get; protected set; }

    public Decay(BaseController target, float duration, int slowAmount) : base("Decay", duration, false)
    {
        _target = target;
        SlowAmount = slowAmount;
    }

    public override void ApplyEffect()
    {
        _target.SpeedFactor -= SlowAmount;
    }

    public override void OnExpire()
    {
        _target.SpeedFactor += SlowAmount;
    }
}

public class Shock : BaseBuff
{
    private BaseController _target;

    public int DamageAmount { get; protected set; }

    public Shock(BaseController target, float duration, int multiplyAmount) : base("Shock", duration, false)
    {
        _target = target;
        DamageAmount = multiplyAmount;
    }

    public override void ApplyEffect()
    {
        _target.TakenDamage += DamageAmount;
    }

    public override void OnExpire()
    {
        _target.TakenDamage -= DamageAmount;
    }
}

public class Combustion : BaseBuff
{
    private EnemyController _target;

    private Collider[] _enemies;
    public int Damage { get; protected set; }

    public Combustion(EnemyController target, int damage) : base("Combustion", 0.1f, false)
    {
        _target = target;
        Damage = damage;
    }

    public override void ApplyEffect()
    {
        _enemies = Physics.OverlapSphere(_target.transform.position, 5.0f, 1 << LayerMask.NameToLayer("Enemy"));
        _target.TakeDamage(Damage);
        _target.BuffManager.AddBuff(new Stagger(_target, Vector3.zero));
        foreach (var enemy in _enemies)
        {
            EnemyController stat = enemy.GetComponent<EnemyController>();
            if (stat.GetInstanceID() == _target.GetInstanceID()) continue;
            stat.TakeDamage(Damage / 2);
            Vector3 force = enemy.transform.position - _target.transform.position;
            force.y = 0f;
            force = force.normalized * 5f;
            stat.BuffManager.AddBuff(new Stagger(stat, force));
        }
    }

    public override void OnExpire()
    {
    }
}

public class Manipulation : BaseBuff
{
    private EnemyController _target;

    public Manipulation(EnemyController target, float duration) : base("Manipulation", duration, false)
    {
        _target = target;
    }

    public override void ApplyEffect()
    {
        _target.TargetLayer = Global.ObjectLayer.Enemy;
        _target.Target = null;
    }

    public override void OnExpire()
    {
        _target.TargetLayer = Global.ObjectLayer.Player;
        _target.Target = null;
    }
}

public class Miasma : BaseBuff
{
    private EnemyController _target;

    public int Damage { get; protected set; }
    private float nextTick;

    public Miasma(EnemyController target, float duration) : base("Miasma", duration, true)
    {
        _target = target;
    }

    public override void ApplyEffect()
    {
        if (_target == null) return;
        Damage = (int)(_target.HP.Value * 0.09f);
        nextTick = 1f;
    }

    public override void UpdateBuff(float deltaTime)
    {
        base.UpdateBuff(deltaTime);
        if (ElapsedTime >= nextTick)
        {
            _target.TakeDamage(Damage, EnumTypes.ElementType.None, true);
            nextTick += 1f;
        }
    }

    public override void OnExpire()
    {
        _target.TakeDamage(Damage, EnumTypes.ElementType.None, true);
    }
}

public class Freeze : BaseBuff
{
    private EnemyController _target;

    public Freeze(EnemyController target, float duration) : base("Freeze", duration, false)
    {
        _target = target;
    }

    public override void ApplyEffect()
    {
        _target.Agent.enabled = false;
        _target.StateMachine.enabled = false;
    }

    public override void OnExpire()
    {
        _target.Agent.enabled = true;
        _target.StateMachine.enabled = true;
    }
}

public class Stagger : BaseBuff
{
    private BaseController _target;
    private Vector3 _force;
    private Rigidbody _rb;

    private bool isKnockBack;

    public Stagger(BaseController target, Vector3 force) : base("Stagger", 5.0f, false)
    {
        _target = target;
        _force = force;
        _rb = _target.GetComponent<Rigidbody>();
    }

    public override void ApplyEffect()
    {
        if (_target is EnemyController _enemy)
        {
            _enemy.StateMachine.ChangeState(new NonState());
        }
        else if(_target is PlayerController _player)
        {
            _player.isStaggered = true;
        }
        KnockBack().Forget();
        isKnockBack = true;
    }

    private async UniTaskVoid KnockBack()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = false;
        _rb.AddForce(_force, ForceMode.Impulse);
        Debug.Log(_force);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        if (_target is EnemyController)
            _rb.isKinematic = true;
        else if (_target is PlayerController _player)
        {
            _rb.linearVelocity = Vector3.zero;
            _player.isStaggered = false;
            IsExpired = true;
        }
    }

    public override void UpdateBuff(float deltaTime)
    {
        base.UpdateBuff(deltaTime);
        if(ElapsedTime > 1f && isKnockBack)
        {
            if (_target is EnemyController _enemy)
            {
                _enemy.StateMachine.ChangeState(new SearchState());
            }
            isKnockBack = false;
        }
    }

    public override void OnExpire()
    {
        
    }
}