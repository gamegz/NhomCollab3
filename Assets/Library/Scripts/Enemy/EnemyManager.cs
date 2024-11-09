using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
		public delegate void OnSpawnRequestEvent(EnemyBase enemy, Vector3 location, Quaternion rotation, Action<EnemyBase> onDeathCallBack = null);
		public static OnSpawnRequestEvent onSpawnRequestEvent;

		//public delegate void OnEnemyDeathEvent(EnemyBase enemy);
		//public static OnEnemyDeathEvent onEnemyDeathEvent;
		public struct EnemySpawnData
		{
			public EnemyBase enemyToSpawn;
			public Vector3 spawnLocation;
			public Quaternion spawnRotation;
		}

		private static int _maxActiveEnemy = 9;
		private int _currentAtiveEnemy;

		private static int _maxAttackToken = 3;
		private int _currentAttackToken;

		private List<EnemyBase> enemies;
		private List<EnemyBase> enemiesTokenOwner;
		private List<EnemySpawnData> enemiesSpawnOnHold;
		private float _enemySpawnOnHoldDelay = 2;
		public GameObject PlayerChildObject;

        private void Awake()
        {
			enemies = FindObjectsOfType<EnemyBase>().ToList();
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
			if (enemies == null) { return; }

			foreach (EnemyBase enemy in enemies)
			{
				enemy.UpdateLogic();
				enemy.FixedUpdateLogic();
			}

			TrySpawnOnHolders();

		}

		private void UpdateTokenOwner()
        {

        }

		private void TrySpawnOnHolders()
		{
			//Cancel if no enemy to spawn or might exceed max active enemy
			if (enemiesSpawnOnHold == null) { return; }
			if (enemies.Count + 1 > _maxActiveEnemy) { return; } 

			//Spawn and remove used data in the list
			EnemySpawnData firstEnemyData = enemiesSpawnOnHold[0];
			EnemyBase spawnedEnemy = Instantiate(firstEnemyData.enemyToSpawn, firstEnemyData.spawnLocation, firstEnemyData.spawnRotation);
            AssignPlayerChildObject(spawnedEnemy);
            enemiesSpawnOnHold.RemoveAt(0);
			enemies.Add(spawnedEnemy);
			_currentAtiveEnemy++;

		}

		public void OnSpawnRequest(EnemyBase SpawnEnemy, Vector3 SpawnLocation, Quaternion SpawnRotation, Action<EnemyBase> onDeathCallBack = null)
		{
			if (enemiesSpawnOnHold != null) { return; } //Request denied
			//Might exceed max active enemy, move to on hold
			if (enemies.Count + 1 > _maxActiveEnemy)
			{
				EnemySpawnData enemySpawnData;
				enemySpawnData.enemyToSpawn = SpawnEnemy;
				enemySpawnData.spawnLocation = SpawnLocation;
				enemySpawnData.spawnRotation = SpawnRotation;

				enemiesSpawnOnHold.Add(enemySpawnData);
				return;
			} //Not exceed max enemies, can spawn.

			EnemyBase spawnedEnemy = Instantiate(SpawnEnemy, SpawnLocation, SpawnRotation);
            spawnedEnemy.OnEnemyDeaths += OnEnemyDeath;
			if(onDeathCallBack != null)
			{
				spawnedEnemy.OnEnemyDeaths += onDeathCallBack;
			}
            AssignPlayerChildObject(spawnedEnemy);
            AddActiveEnemy(spawnedEnemy);
		}

		private void AssignPlayerChildObject(EnemyBase enemy)
		{
			if(PlayerChildObject != null)
			{
				enemy.SetPlayerChildObject(PlayerChildObject);
			}
		}

		private void OnEnemyDeath(EnemyBase enemyref)
		{
			RemoveActiveEnemy(enemyref);
			enemyref.OnEnemyDeaths -= OnEnemyDeath;
            if (enemyref.isTokenOwner)
            {
                _currentAttackToken++;
            }
        }

		private void OnTokenRequested(EnemyBase enemy)
		{
			if (enemy == null) { return; }
			if (_currentAttackToken <= _maxAttackToken && _currentAttackToken > 0)
			{
				enemy.isTokenOwner = true;
				_currentAttackToken--;
            }

		}

		private void AddActiveEnemy(EnemyBase enemy)
        {
			enemies.Add(enemy);
			_currentAtiveEnemy++;

        }
		private void RemoveActiveEnemy(EnemyBase enemy)
        {
			enemies.Remove(enemy);
			_currentAtiveEnemy--;
        }

	}

}

