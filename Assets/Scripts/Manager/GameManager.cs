using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject PlayerPrefab;
    public Transform SpawnPoint;

    public InputManager Input { get; set; }

    public PlayerController Player { get; set; }

    public ulong PlayerID { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public Path path;

    [SerializeField]
    private int maxEnemyCount = 0;
    [SerializeField]
    private float spawnCoolDown = 3f;
    [SerializeField]
    private GameObject m_Enemy_Prefab;

    private int _enemyCount = 0;

    private float spawnTimer = 0f;

    void Start()
    {

    }

    void Update()
    {
        if (Input != null)
            Input.OnUpdate();
        if (NetworkManager.Singleton.IsServer)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnCoolDown && _enemyCount < maxEnemyCount)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
    }

    public void OnPlayerSpawned(PlayerController player)
    {
        Player = player;
        Player.InitUI();
        Input = new InputManager();
        Input.Init();
        PlayerID = player.NetworkObjectId;
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(-7f, 5f), 1f, UnityEngine.Random.Range(8f, 18f));
        NetworkObject go = NetworkObjectPool.Instance.GetNetworkObject("Enemy", spawnPoint, Quaternion.identity);
        //go.GetComponent<EnemyController>().Init();
        go.Spawn();
        _enemyCount++;
        SpawnEnemyRPC(spawnPoint);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnEnemyRPC(Vector3 spawnPoint)
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            //NetworkObject go = NetworkObjectPool.Instance.GetNetworkObject("Enemy", spawnPoint, Quaternion.identity);
            //go.GetComponent<EnemyController>().Init();
            _enemyCount++;
        }
    }

    public void OnEnemyDead(NetworkObject enemy)
    {
        _enemyCount--;
        enemy.Despawn();
        //spawnTimer = 0f;
    }
}
