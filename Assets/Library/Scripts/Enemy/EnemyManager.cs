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

		private static int _maxActiveEnemy = 9;
		private int _currentAtiveEnemy;

		private static int _maxAttackToken = 3;
		private int _currentAttackToken;

		private List<EnemyBase> enemies;
		private List<EnemySpawnData> enemiesSpawnOnHold;
		private float _enemySpawnOnHoldDelay = 2;

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

			UpdateTokenOwner();
			foreach (EnemyBase enemy in enemies)
			{
				enemy.UpdateLogic();
				enemy.FixedUpdateLogic();
			}

			TrySpawnOnHolders();

		}

		private void UpdateTokenOwner()
        {
			_currentAttackToken = _maxAttackToken;
			if(enemies.Count <= _maxAttackToken)
			{
				foreach (EnemyBase enemy in enemies)
				{
					enemy.isTokenOwner = true;
				}
				return; 
			}

			List<EnemyBase> enemiesInst = enemies.OrderBy(x => x.distanceToPlayer).ToList();
			for (int i = 0; i < enemiesInst.Count; i++)
			{
				if (_currentAttackToken <= 0) { break; }
				if (enemiesInst[i].isAttacking) { continue; }
				enemiesInst[i].isTokenOwner = true;
                _currentAttackToken--;
            }
        }

		private void TrySpawnOnHolders()
		{
			//Cancel if no enemy to spawn or might exceed max active enemy
			if (enemiesSpawnOnHold == null) { return; }
			if (enemies.Count + 1 > _maxActiveEnemy) { return; } 

			//Spawn and remove used data in the list
			EnemySpawnData firstEnemyData = enemiesSpawnOnHold[0];
			EnemyBase spawnedEnemy = Instantiate(firstEnemyData.enemyToSpawn, firstEnemyData.spawnLocation, firstEnemyData.spawnRotation);
			enemiesSpawnOnHold.RemoveAt(0);
			enemies.Add(spawnedEnemy);
			_currentAtiveEnemy++;

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

			EnemyBase spawnedEnemy = Instantiate(SpawnEnemy, SpawnLocation, SpawnRotation);
			AddActiveEnemy(spawnedEnemy);

		}

		private void OnEnemyDeath(EnemyBase enemyref)
		{
			RemoveActiveEnemy(enemyref);

            if (enemyref.isTokenOwner)
            {
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

