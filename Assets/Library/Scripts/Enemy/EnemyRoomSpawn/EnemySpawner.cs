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
    [SerializeField] private GameObject Door1;
    [SerializeField] private GameObject Door2;
    [SerializeField] private GameObject Door3;
    [SerializeField] private GameObject Door4;
    [SerializeField] private GameObject RoomTrigger1;
    [SerializeField] private GameObject RoomTrigger2;
    [SerializeField] private GameObject RoomTrigger3;
    [SerializeField] private GameObject RoomTrigger4;
    [SerializeField] private ExtraRoom linkedExtraRoom;
    
    [SerializeField] private List<SectionReset> sectionResetRef;
    [SerializeField] private List<int> extraRoomIndex;
    private int _currentEnemyCount = 0;
    private int spawnedEnemies = 0;
    private int _currentWaveCount = 0;

    void Start()
    {

    }

    private void OnEnable()
    {
        OnResetEnemyWhenEnterRoom();
        OnStartSpawning();
        if (RoomTrigger1 != null)
        {
            RoomTrigger1.GetComponent<BoxCollider>().enabled = false;
        }

        if (RoomTrigger2 != null)
        {
            RoomTrigger2.GetComponent<BoxCollider>().enabled = false;
        }
        
        if (RoomTrigger3 != null)
        {
            RoomTrigger3.GetComponent<BoxCollider>().enabled = false;
        }

        if (RoomTrigger4 != null)
        {
            RoomTrigger4.GetComponent<BoxCollider>().enabled = false;
        }
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

    public void OnFinishCurrentRoom()
    {
        ExpOrb[] expOrbs = FindObjectsOfType<ExpOrb>();
        foreach (ExpOrb expOrb in expOrbs)
        {
            expOrb.AllowToMoveTowardPlayer();
        }

        for (int i = 0; i < sectionResetRef.Count; i++)
        {
            var sectionReset = sectionResetRef[i];
            int index = extraRoomIndex[i];
            
            if (sectionResetRef != null && index >= 0 && index < sectionReset.ExtraRoomList.Count)
            {
                Debug.Log(index);
                sectionReset.ExtraRoomList[index].MarkAsCleared();
            }
        }

        if (Door1 != null)
        {
            Door1.SetActive(false);
        }

        if (Door2 != null)
        {
            Door2.SetActive(false);
        }

        if (Door3 != null)
        {
            Door3.SetActive(false);
        }

        if (Door4 != null)
        {
            Door4.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }

    private void OnResetEnemyWhenEnterRoom()
    {
        _currentWaveCount = 0;
        _currentEnemyCount = 0;
        spawnedEnemies = 0;
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
