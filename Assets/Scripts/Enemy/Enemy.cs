using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject target;
    private EnemyStat stat;
    private Vector3 lastKnownPos;

    public StateMachine FSM { get => stateMachine; }
    public EnemyStat Stat { get { return stat; } }
    public NavMeshAgent Agent { get => agent; }
    public GameObject Target { get => target; set => target = value; }
    public Vector3 LastKnownPos { get => lastKnownPos; set => lastKnownPos = value; }

    public Transform enemyTransform;

    public Path path;
    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;
    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10f)]
    public float fireRate;

    private Collider[] _enemies;

    [Header("Drop Table")]
    public int[] probability;

    //for debugging perposes
    [SerializeField]
    private string currentState;

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stat = GetComponent<EnemyStat>();
        stateMachine.Init();
        target = Player.Instance.gameObject;
        path = GameManager.Instance.path;
        //target = null;
    }

    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if(stat.FindBuff("Manipulation"))
        {
            _enemies = Physics.OverlapSphere(transform.position, sightDistance, 1 << LayerMask.NameToLayer("Enemy"));
            if(_enemies.Length > 1)
            {
                float minDist = 10000f;
                target = null;
                foreach(var enemy in _enemies)
                {
                    if (enemy.gameObject.GetInstanceID() == gameObject.GetInstanceID()) continue;
                    float enemyDist = Vector3.Distance(transform.position, enemy.transform.position);
                    if(enemyDist < minDist)
                    {
                        minDist = enemyDist;
                        target = enemy.gameObject;
                    }
                }
                agent.SetDestination(target.transform.position);
                return true;
            }
            return false;
        }
        if (target != null)
        {
            if(Vector3.Distance(transform.position, target.transform.position) < sightDistance)
            {
                Vector3 targetDirection = target.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo = new RaycastHit();
                    if(Physics.Raycast(ray, out hitInfo,sightDistance))
                    {
                        if(hitInfo.transform.gameObject == target)
                        {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void MakePickup()
    {
        int value = 100;
        for (int i = 0; i < probability.Length; i++)
        {
            value = Random.Range(1, 100);
            if(value <= probability[i])
            {
                GameObject pickup = ObjectPool.Instance.Pop("Pickup", transform.position, Quaternion.identity);
                Item item = ItemTable.Instance.GetRandomItem((ItemType)i);
                pickup.GetComponent<Pickup>().MakePickup(item);
                pickup.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * 3.0f, ForceMode.Impulse);
            }
        }
    }
}
