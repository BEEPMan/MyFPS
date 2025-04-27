using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseController
{
    public StateMachine StateMachine { get; set; }
    public NavMeshAgent Agent { get; set; }
    public GameObject Target { get; set; }
    public Vector3 LastKnownPos { get; set; }
    public string TargetLayer { get; set; }

    public UI_EnemyHPBar View;

    public Path path;
    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;
    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    private Collider[] targets;

    [Header("Drop Table")]
    public int[] probability;

    //for debugging perposes
    [SerializeField]
    private string currentState;

    void Awake()
    {
        TargetLayer = Global.ObjectLayer.Player;
        StateMachine = GetComponent<StateMachine>();
        Agent = GetComponent<NavMeshAgent>();
        path = GameManager.Instance.path;

        ElementalDamage = new int[Enum.GetValues(typeof(EnumTypes.ElementType)).Length];
        BuffManager = new BuffManager(this);

        //SetNearestTarget();
    }

    void OnEnable()
    {
        StateMachine.Init();

        InitStat();

        HP.OnValueChanged += (previous, current) =>
        {
            View.UpdateHPBar(HP.Value, MaxHP.Value);
        };
        Shield.OnValueChanged += (previous, current) =>
        {
            View.UpdateSABar(HP.Value, MaxHP.Value);
        };
        Armor.OnValueChanged += (previous, current) =>
        {
            View.UpdateSABar(HP.Value, MaxHP.Value);
        };
        View.InitHPBer();
        View.InitBuffIcons();
        View.UpdateHPType(HPType);
        if (StateMachine != null)
            StateMachine.ChangeState(new PatrolState());
        BuffManager.buffEnabled += OnBuffEnabled;
        BuffManager.buffDisabled += OnBuffDisabled;
    }

    protected override void InitStat()
    {
        base.InitStat();
    }

    void Update()
    {
        //CanSeeTarget();
        if (BuffManager != null && IsServer)
            BuffManager.OnUpdate();
        currentState = StateMachine.activeState.ToString();
    }

    public void SetNearestTarget()
    {
        targets = Physics.OverlapSphere(transform.position, sightDistance, 1 << LayerMask.NameToLayer(TargetLayer));
        if (targets.Length > 0)
        {
            float minDist = 10000f;
            foreach (var target in targets)
            {
                if (target.gameObject.GetInstanceID() == gameObject.GetInstanceID()) continue;
                float targetDist = Vector3.Distance(transform.position, target.transform.position);
                if (targetDist < minDist)
                {
                    minDist = targetDist;
                    Target = target.gameObject;
                }
            }
            //Agent.SetDestination(Target.transform.position);
        }
    }

    public bool CanSeeTarget()
    {
        if (Target != null)
        {
            if(Vector3.Distance(transform.position, Target.transform.position) < sightDistance)
            {
                Vector3 targetDirection = Target.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo;
                    if(Physics.Raycast(ray, out hitInfo,sightDistance))
                    {
                        if(hitInfo.transform.gameObject == Target)
                        {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            return true;
                        }
                        else if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer(TargetLayer))
                        {
                            Target = hitInfo.transform.gameObject;
                            return true;
                        }
                    }
                }
            }
        }
        else
        {
            SetNearestTarget();
        }
        return false;
    }

    [Rpc(SendTo.Server)]
    protected override void AttackServerRPC()
    {
        GameObject bullet = ObjectPool.Instance.Pop("Bullet", gunBarrel.position, transform.rotation);
        bullet.GetComponent<Bullet>().TargetTag = TargetLayer;
        Vector3 fireDirection = (Target.transform.position - gunBarrel.transform.position).normalized;
        fireDirection = Quaternion.AngleAxis(UnityEngine.Random.Range(-3f, 3f), Vector3.up) * fireDirection * 40;
        bullet.GetComponent<Bullet>().direction = fireDirection;
        Vector3 lookDirection = new Vector3(0f, fireDirection.y, fireDirection.z).normalized;
        bullet.transform.Rotate(lookDirection, Space.Self);
        bullet.GetComponent<Rigidbody>().linearVelocity = fireDirection;
        AttackClientRPC(fireDirection);
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void AttackClientRPC(Vector3 direction)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            GameObject bullet = ObjectPool.Instance.Pop("Bullet", gunBarrel.position, transform.rotation);
            bullet.GetComponent<Bullet>().TargetTag = TargetLayer;
            bullet.GetComponent<Bullet>().direction = direction;
            Vector3 lookDirection = new Vector3(0f, direction.y, direction.z).normalized;
            bullet.transform.Rotate(lookDirection, Space.Self);
            bullet.GetComponent<Rigidbody>().linearVelocity = direction;
        }
    }

    public override void TakeDamage(int damage, EnumTypes.ElementType elementType = EnumTypes.ElementType.None, bool isTrueDamage = false)
    {
        int finalDamage = damage;
        switch (HPType)
        {
            case EnumTypes.HPType.HPOnly:
                if (!isTrueDamage)
                {
                    finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(HPType, elementType)) / 100;
                }
                HP.Value = Mathf.Clamp(HP.Value - finalDamage, 0, MaxHP.Value);
                break;
            case EnumTypes.HPType.Shield:
                if (Shield.Value > 0)
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(HPType, elementType)) / 100;
                    }
                    Shield.Value = Mathf.Clamp(Shield.Value - finalDamage, 0, MaxShield.Value);
                }
                else
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(EnumTypes.HPType.HPOnly, elementType)) / 100;
                    }
                    HP.Value = Mathf.Clamp(HP.Value - finalDamage, 0, MaxHP.Value);
                }
                break;
            case EnumTypes.HPType.Armor:
                if (Armor.Value > 0)
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(HPType, elementType)) / 100;
                    }
                    Armor.Value = Mathf.Clamp(Armor.Value - finalDamage, 0, MaxArmor.Value);
                }
                else
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(EnumTypes.HPType.HPOnly, elementType)) / 100;
                    }
                    HP.Value = Mathf.Clamp(HP.Value - finalDamage, 0, MaxHP.Value);
                }
                break;
        }
        View.PopDamageText(finalDamage);
        PopDamageRPC(finalDamage);
        if (NetworkManager.Singleton.IsServer)
            TriggerElementalEffect(elementType, finalDamage);
        if (HP.Value <= 0)
        {
            Die();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PopDamageRPC(int damage)
    {
        if (!NetworkManager.Singleton.IsHost)
            View.PopDamageText(damage);
    }

    public override void Die()
    {
        if (Utils.GetRandomResult(100))
        {
            DropItemRPC();
        }
        BuffManager.ClearBuffs();
        BuffManager.buffEnabled -= OnBuffEnabled;
        BuffManager.buffDisabled -= OnBuffDisabled;
        StateMachine.ChangeState(new NonState());
        GameManager.Instance.OnEnemyDead(NetworkObject);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DropItemRPC()
    {
        Item item = ItemManager.Instance.GetRandomPickupItem();
        ItemManager.Instance.MakePickupItem(item, transform.position);
    }

    public void OnBuffEnabled(BaseBuff buff)
    {
        OnBuffEnabledServerRPC(buff.BuffName);
    }

    [Rpc(SendTo.Server)]
    public void OnBuffEnabledServerRPC(string buffName)
    {
        View.EnableBuffIcon(buffName);
        if (buffName == "Miasma")
        {
            int count = BuffManager.GetMiasmaCount();
            if (count >= 2)
            {
                View.EnableMiasmaStack();
                View.UpdateMiasmaCount(count);
            }
        }
        OnBuffEnabledClientRPC(buffName);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnBuffEnabledClientRPC(string buffName)
    {
        if(!NetworkManager.Singleton.IsHost)
        {
            View.EnableBuffIcon(buffName);
            if (buffName == "Miasma")
            {
                int count = BuffManager.GetMiasmaCount();
                if (count >= 2)
                {
                    View.EnableMiasmaStack();
                    View.UpdateMiasmaCount(count);
                }
            }
        }
    }

    public void OnBuffDisabled(BaseBuff buff)
    {
        OnBuffDisabledServerRPC(buff.BuffName);
    }

    [Rpc(SendTo.Server)]
    public void OnBuffDisabledServerRPC(string buffName)
    {
        if (buffName == "Miasma")
        {
            int count = BuffManager.GetMiasmaCount();
            if (count == 2)
            {
                View.DisableMiasmaStack();

            }
            else if (count == 1)
            {
                View.DisableBuffIcon(buffName);
            }
            View.UpdateMiasmaCount(count);
        }
        else
        {
            View.DisableBuffIcon(buffName);
        }
        OnBuffDisabledClientRPC(buffName);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnBuffDisabledClientRPC(string buffName)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            if (buffName == "Miasma")
            {
                int count = BuffManager.GetMiasmaCount();
                if (count == 2)
                {
                    View.DisableMiasmaStack();

                }
                else if (count == 1)
                {
                    View.DisableBuffIcon(buffName);
                }
                View.UpdateMiasmaCount(count);
            }
            else
            {
                View.DisableBuffIcon(buffName);
            }
        }
    }
}
