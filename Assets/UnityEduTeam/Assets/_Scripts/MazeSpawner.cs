using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MazeSpawner : MonoBehaviour {

	//TODO: Logique à déplacer
	// Test culling
	//---------------------------------------------------
	[SerializeField]private GameObject _player;
	[SerializeField]private float _testDistanceMax = 25.0f;
	[SerializeField]private float _interval = 0.5f; 
	private float nextCheckTime = 0;
	//---------------------------------------------------
	[SerializeField]private List<GameObject> Modules = new List<GameObject>();
	private List<GameObject> SpawnPoints = new List<GameObject>();
	private List<GameObject> MazeModules = new List<GameObject>();
    
	private IObjectPool<Transform> _modulePool;

	// Use this for initialization
	void Start()
	{
		//TODO:a optimiser car avec une list, le résultat n'est pas concluant..
		SpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("ModuleLoc"));

		// Init des modules
		_modulePool = new ObjectPool<Transform>(CreateNewModule, OnEnableModule, OnDisableModule, OnDestroyModule);
		InstantiateMazeModules();
	}

	// petit trick pour simuler le culling
	private void Update()
	{
		
		if (Time.time >= nextCheckTime)
		{
			nextCheckTime = Time.time + _interval;

			foreach (GameObject module in MazeModules)
			{
				float distanceToPlayer = Vector3.Distance(_player.transform.position, module.transform.position);
                
				if (distanceToPlayer < _testDistanceMax)
				{
					module.SetActive(true);
				}
				else
				{
					module.SetActive(false);
				}
			}
		}
	}

	private void InstantiateMazeModules()
	{
		foreach (GameObject spawnPoint in SpawnPoints)
		{
			Transform module = _modulePool.Get();
			module.position = spawnPoint.transform.position;
			module.rotation = Quaternion.identity;
			MazeModules.Add(module.gameObject);
		}

		CheckModules();
	}
	


	// Update is called once per frame
	void CheckModules()
	{
		bool checkPosition = false;
		bool checkRotation = false;

		//Boucle sur les modules du maze
		for (int i = 0; i < MazeModules.Count; i++)
		{
			
			checkPosition = false;
			checkRotation = false;
			Vector3 modulePosition = MazeModules[i].transform.position;
			Quaternion moduleRotation = MazeModules[i].transform.rotation;

			// Boucle sur chaque point de spawn
			for (int j = 0; j < SpawnPoints.Count; j++)
			{
				Vector3 spawnPosition = SpawnPoints[j].transform.position;
				Quaternion spawnRototion = SpawnPoints[j].transform.rotation;

				if (Vector3.SqrMagnitude(spawnPosition - modulePosition) <= 0.01f * 0.01f)
				{
					checkPosition = true;

					if (Quaternion.Angle(spawnRototion, moduleRotation) <= 0.01f)
					{
						checkRotation = true;
					}
					break;
				}
			}
		}
	}
	private Transform CreateNewModule()
	{
		GameObject module = Instantiate(Modules[Random.Range(0, Modules.Count)]);
		return module.transform;
	}

	private void OnEnableModule(Transform module)
	{
		module.gameObject.SetActive(true);
	}

	private void OnDisableModule(Transform module)
	{
		module.gameObject.SetActive(false);
	}

	private void OnDestroyModule(Transform module)
	{
		Destroy(module.gameObject);
	}
	
	public void ReleaseModule(Transform module)
	{
		_modulePool.Release(module);
	}

}