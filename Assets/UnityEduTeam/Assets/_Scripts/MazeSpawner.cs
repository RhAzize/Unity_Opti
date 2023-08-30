using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSpawner : MonoBehaviour {

	public List<GameObject> Modules = new List<GameObject>();

	private List<GameObject> SpawnPoints = new List<GameObject>();

	private List<GameObject> MazeModules = new List<GameObject>();
//    public GameObject[] modLocPrefabs; 

	// Use this for initialization
	void Start()
	{
		InstantiateMazeModules();
	}


	private void InstantiateMazeModules()
	{
		SpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("ModuleLoc"));

		foreach (GameObject SpawnPoint in SpawnPoints)
		{
			MazeModules.Add(Instantiate(Modules[Random.Range(0, Modules.Count)], SpawnPoint.transform.position, Quaternion.identity));
		}

		CheckModules();
	}
	


	// Update is called once per frame
	void CheckModules()
	{
		//Vérification de la bonne position des éléments du Maze
		//TODO : en faire une fonction

		//Préparation des variables
		bool checkPosition = false;
		bool checkRotation = false;

		//Boucle sur les modules du maze
		for (int i = 0; i < MazeModules.Count; i++)
		{
			{
				//on remets les variables de test à false
				checkPosition = false;
				checkRotation = false;

				// on boucle sur chaque spawnPoints
				for (int j = 0; j < SpawnPoints.Count; j++)
				{
					// on compare la posiiton du spawnPoint à la position du module
					// on utilise Vector3.distance pour avoir une sécurité par rapport aux erreurs de virgules flottantes d'unity
					if (Vector3.SqrMagnitude(SpawnPoints[j].transform.position - MazeModules[i].transform.position) <=
					    0.01f * 0.01f)
					{
						// on mets la la variable de test de position à vrai vu que l'écart de distance est acceptable
						checkPosition = true;
						// on utilise Quaternion.Angle pour avoir une sécurité par rapport aux erreurs de virgules flottantes d'unity
						if (Quaternion.Angle(SpawnPoints[j].transform.rotation, MazeModules[i].transform.rotation) <=
						    0.01f)
						{
							// on mets la la variable de test de rotation à vrai vu que l'écart d'angle est acceptable
							checkRotation = true;
						}

						// on arrete la boucle vu qu'au moins la position est bonne, pas la peine d'aller plus loin, on économise de la mémoire et du temps CPU
						break;
					}
				}
			}
		}
	}
}