using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) return null;
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public Path path;

    [SerializeField]
    private int maxEnemyCount = 4;
    [SerializeField]
    private float spawnCoolDown = 3f;

    private int _enemyCount = 2;

    private float spawnTimer = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnCoolDown && _enemyCount < maxEnemyCount)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPoint = new Vector3(Random.Range(-7f, 5f), 1f, Random.Range(8f, 18f));
        GameObject go = ObjectPool.Instance.Pop("Enemy", spawnPoint, Quaternion.identity);
        go.GetComponent<EnemyStat>().Init();
        _enemyCount++;
    }

    public void OnEnemyDead()
    {
        _enemyCount--;
    }
}
