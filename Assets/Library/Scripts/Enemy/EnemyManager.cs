using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

		public struct EnemySpawnData
		{
			public EnemyBase enemyToSpawn;
			public Vector3 spawnLocation;
			public Quaternion spawnRotation;
		}

		private int _maxActiveEnemy;
		private int _currentAtiveEnemy;

		private int _maxAttackToken;
		private int _currentAttackToken;

		private List<EnemyBase> enemies;
		private List<EnemyBase> enemiesTokenOwner;
		private List<EnemySpawnData> enemiesSpawnOnHold;

        private void Awake()
        {
			enemies = FindObjectsOfType<EnemyBase>().ToList();
        }

        private void Start()
        {
            //Subcribe OnRequestToken (take enemy)
			//Subscribe OnSpawnRequest (take spawn data)
			//Subcribe OnEnemyDeath
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


		private void TrySpawnOnHolders()
		{
			if (enemies.Count + 1 > _maxActiveEnemy) { return; } //Might exceed max active enemy, cancel
			if (enemiesSpawnOnHold != null)
			{
				//Spawn and remove used data in the list
				EnemySpawnData firstEnemyData = enemiesSpawnOnHold[0];
				EnemyBase spawnedEnemy = Instantiate(firstEnemyData.enemyToSpawn, firstEnemyData.spawnLocation, firstEnemyData.spawnRotation);
				enemiesSpawnOnHold.RemoveAt(0);
				enemies.Add(spawnedEnemy);
				_currentAtiveEnemy++;
			}

		}

		public void OnSpawnRequest(EnemyBase SpawnEnemy, Vector3 SpawnLocation, Quaternion SpawnRotation)
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

			Instantiate(SpawnEnemy, SpawnLocation, SpawnRotation);
			_currentAtiveEnemy++;

		}

		private void OnEnemyDeath(EnemyBase enemyref)
		{
			enemies.Remove(enemyref);
			_currentAtiveEnemy--;

            if (enemyref.isTokenOwner && _currentAttackToken < _maxAttackToken)
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



	}

}

