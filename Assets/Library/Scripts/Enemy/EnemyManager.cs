using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static UnityEngine.EventSystems.EventTrigger;

namespace Enemy.EnemyManager
{
	//Keeping track of enemy token
	//Keeping track of active enemy
	//Listen to enemy token request and Update token count if needed
	//Listen to enemy death and return token
	//Handle spawning request
	//Handle updating all enemy logic
	public class EnemyManager : MonoBehaviour
	{
		public delegate void OnSpawnRequestEvent(EnemyBase enemy, Vector3 location, Quaternion rotation/*, Action<EnemyBase> onDeathCallBack = null*/);
		public static OnSpawnRequestEvent onSpawnRequestEvent;

		//public delegate void OnEnemyDeathEvent(EnemyBase enemy);
		//public static OnEnemyDeathEvent onEnemyDeathEvent;
		public struct EnemySpawnData
		{
			public EnemyBase enemyToSpawn;
			public Vector3 spawnLocation;
			public Quaternion spawnRotation;
		}

		[SerializeField] private int _maxActiveEnemy = 9;
		[SerializeField] private int _maxAttackToken = 3;
		private int _currentAttackToken;

		private List<EnemyBase> activeEnemies = new List<EnemyBase>();
		private List<EnemySpawnData> enemiesSpawnOnHold = new List<EnemySpawnData>();
		private float _enemySpawnOnHoldDelay = 2;
		private GameObject _player;
		StatsUpgrade _playerStatsUpgrade;

		[Header("ExpPrefab")]
		[SerializeField] private GameObject _expPrefab;

        private void Awake()
        {
			activeEnemies = FindObjectsOfType<EnemyBase>().ToList();
			_player = GameObject.FindGameObjectWithTag("Player");
			_playerStatsUpgrade = _player.GetComponent<StatsUpgrade>();
			foreach(EnemyBase enemy in activeEnemies)
            {
				enemy.playerRef = _player;
            }
        }

        private void Start()
        {
			//Subcribe OnRequestToken (take enemy)
			//Subscribe OnSpawnRequest (take spawn data)
			//Subcribe OnEnemyDeath
			onSpawnRequestEvent += OnSpawnRequest;
        }

        private void OnDestroy()
        {
            onSpawnRequestEvent -= OnSpawnRequest;
        }

        private void OnDisable()
        {
            onSpawnRequestEvent -= OnSpawnRequest;
        }

        private void Update()
		{
			if (activeEnemies == null) { return; }

			UpdateTokenOwner();
			foreach (EnemyBase enemy in activeEnemies)
			{
				enemy.UpdateLogic();
				enemy.FixedUpdateLogic();
			}

			TrySpawnOnHolders();

		}

		private void UpdateTokenOwner()
        {
			_currentAttackToken = _maxAttackToken;
			if(activeEnemies.Count <= _maxAttackToken)
			{
				foreach (EnemyBase enemy in activeEnemies)
				{
					enemy.isTokenOwner = true;
				}
				return; 
			}

			List<EnemyBase> enemiesInst = activeEnemies.OrderBy(x => x.distanceToPlayer).ToList();
			for (int i = 0; i < enemiesInst.Count; i++)
			{
				//enemiesInst[i].isTokenOwner = false;
				//if (_currentAttackToken <= 0) { return; }
				if(enemiesInst[i].isAttacking) { continue; }
				if(!enemiesInst[i].IsTokenUser) { continue; }
				if(enemiesInst[i].isTokenOwner) {
					_currentAttackToken--;
					continue; 
				}
				enemiesInst[i].isTokenOwner = true;
				_currentAttackToken--;
				if (_currentAttackToken <= 0) {
					enemiesInst[i].isTokenOwner = false;
					enemiesInst[i].attackCoolDownCount = enemiesInst[i].attackCooldown;
				}
			}
        }

		private void TrySpawnOnHolders()
		{
			//Cancel if no enemy to spawn or might exceed max active enemy
			if (enemiesSpawnOnHold.Count <= 0) { return; }
			if (activeEnemies.Count >= _maxActiveEnemy) { return; } 

			//Spawn and remove used data in the list
			EnemySpawnData firstEnemyData = enemiesSpawnOnHold[0];
			enemiesSpawnOnHold.RemoveAt(0);
			EnemyBase spawnedEnemy = Instantiate(firstEnemyData.enemyToSpawn, firstEnemyData.spawnLocation, firstEnemyData.spawnRotation);
            spawnedEnemy.playerRef = _player;
            spawnedEnemy.expPrefab = _expPrefab;
            spawnedEnemy.playerStatsRef = _playerStatsUpgrade;
            activeEnemies.Add(spawnedEnemy);
			spawnedEnemy.OnEnemyDeaths += OnEnemyDeath;
			Debug.Log(enemiesSpawnOnHold);

		}

		public void OnSpawnRequest(EnemyBase SpawnEnemy, Vector3 SpawnLocation, Quaternion SpawnRotation)
		{
			//Might exceed max active enemy, move to on hold
			if (activeEnemies.Count >= _maxActiveEnemy)
			{
				EnemySpawnData enemySpawnData;
				enemySpawnData.enemyToSpawn = SpawnEnemy;
				enemySpawnData.spawnLocation = SpawnLocation;
				enemySpawnData.spawnRotation = SpawnRotation;
				enemiesSpawnOnHold.Add(enemySpawnData);
                Debug.Log("Enemies spawn on hold: " + enemiesSpawnOnHold);
                return;
			} //Not exceed max enemies, can spawn.

			EnemyBase spawnedEnemy = Instantiate(SpawnEnemy, SpawnLocation, SpawnRotation);
            spawnedEnemy.OnEnemyDeaths += OnEnemyDeath;	
            spawnedEnemy.playerRef = _player;
			spawnedEnemy.expPrefab = _expPrefab;
            spawnedEnemy.playerStatsRef = _playerStatsUpgrade;
            activeEnemies.Add(spawnedEnemy);
		}


		private void OnEnemyDeath(EnemyBase enemyref)
		{
			activeEnemies.Remove(enemyref);
			enemyref.OnEnemyDeaths -= OnEnemyDeath;
            if (enemyref.isTokenOwner)
            {
				enemyref.isTokenOwner = false;
                _currentAttackToken++;
            }
        }

		private void OnTokenRequested(EnemyBase enemy)
		{
			if (enemy == null) { return; }
            if (_currentAttackToken > 0) {

                enemy.isTokenOwner = true;
                _currentAttackToken--;

            }           
        }

	}

}

