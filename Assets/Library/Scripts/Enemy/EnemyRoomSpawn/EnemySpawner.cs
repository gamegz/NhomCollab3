using Enemy.EnemyManager;
using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public EnemyBase enemyPrefab;
        public float spawnPercent;
    }
    [SerializeField] private List<EnemyType> enemyTypes;
    [SerializeField] private List<Transform> spawnPos;
    [SerializeField] private int setAmountOfWave;
    [SerializeField] private int amountOfEnemyPerWave;
    [SerializeField] private GameObject roomLock;
    private int _currentEnemyCount = 0;
    private int spawnedEnemies = 0;
    private int _currentWaveCount = 0;

    void Start()
    {

    }

    private void OnEnable()
    {
        OnStartSpawning();
        EnemyBase.OnEnemyDeathsEvent += OnEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyBase.OnEnemyDeathsEvent -= OnEnemyDeath;
    }

    void Update()
    {

    }

    private void OnStartSpawning()
    {
        int spawnIndex = 0;
        
        if (_currentWaveCount < setAmountOfWave)
        {
            _currentWaveCount++;
            Debug.Log(_currentWaveCount);
            while (_currentEnemyCount < amountOfEnemyPerWave)
            {
                EnemyBase choosenEnemy = CalculateEnemyPerncentage();
                EnemyManager.onSpawnRequestEvent?.Invoke(choosenEnemy, spawnPos[spawnIndex].position, spawnPos[spawnIndex].rotation);
                _currentEnemyCount++;
                spawnedEnemies = _currentEnemyCount;
                spawnIndex = (spawnIndex + 1) % spawnPos.Count;
            }
            
        }

    }

    private void OnEnemyDeath(EnemyBase enemy)
    {
        spawnedEnemies--;
        if (spawnedEnemies <= 0)
        {
            _currentEnemyCount = spawnedEnemies;
            OnEndCurrentWave();
        }
        if (_currentWaveCount == setAmountOfWave && _currentEnemyCount == 0)
        {
            OnFinishCurrentRoom();
        }
    }

    private void OnEndCurrentWave()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(1);
        OnStartSpawning();
    }

    private void OnFinishCurrentRoom()
    {
        this.gameObject.SetActive(false);
        roomLock.SetActive(false);
    }

    private EnemyBase CalculateEnemyPerncentage()
    {
        float totalPercent = 0;
        foreach (EnemyType enemyType in enemyTypes)
        {
            totalPercent += enemyType.spawnPercent;
        }
        float randomPercentage = Random.value * totalPercent;
        foreach (EnemyType enemyType in enemyTypes)
        {
            if (randomPercentage < enemyType.spawnPercent)
            {
                return enemyType.enemyPrefab;
            }
            else
            {
                randomPercentage -= enemyType.spawnPercent;
            }
        }
        return null;
    }
}
