using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{

    public List<GameObject> enemyPrefabs;

    //Optimisation:Stockage dans une list des spawnpoints pour éviter la recherche
    [SerializeField]private List<Transform> enemySpawnPoints;
 
    //Suivi des enemy actif
    private List<GameObject> activeEnemies;
    
    //System de Pool
    private IObjectPool<Transform> _enemyPool;


    void Start()
    {
        activeEnemies = new List<GameObject>();

        // Initialise le pool
        _enemyPool = new ObjectPool<Transform>(CreateNewEnemy, OnEnableEnemy, OnDisableEnemy, OnDestroyEnemy);
        
        
        //On instancie
        SpawnEnemy(enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)]);
        
        /*foreach (Transform spawnPoint in enemySpawnPoints)
        {
            SpawnEnemy(spawnPoint.transform);
        }*/
        
        //Debug.Log("pool : " + enemyPrefabs.Count);
        //Debug.Log("Spawn : " + enemySpawnPoints.Count);
        /*for (int i = 0; i < GameObject.FindGameObjectsWithTag("EnemySpawnPoint").Length; i++)
        {
            Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)],
                GameObject.FindGameObjectsWithTag("EnemySpawnPoint")[i].transform.position,
                GameObject.FindGameObjectsWithTag("EnemySpawnPoint")[i].transform.rotation);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (enemySpawnPoints.Count > activeEnemies.Count)
        {
            Transform randomSpawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            SpawnEnemy(randomSpawnPoint.transform);
        }
        // vérification si un enemy est mort et le cas échéant en faire spawn un nouveau à une position aléatoire
        // pour cela on compare le nombre théorique d'enemy avec le nombre actuel
        /*while (GameObject.FindGameObjectsWithTag("EnemySpawnPoint").Length >
               GameObject.FindGameObjectsWithTag("Enemy").Length)
        {
            int RandomNumber = Random.Range(0, EnemyPrefabs.Count);
            Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)],
                GameObject.FindGameObjectsWithTag("EnemySpawnPoint")[RandomNumber].transform.position,
                GameObject.FindGameObjectsWithTag("EnemySpawnPoint")[RandomNumber].transform.rotation);
        }*/
    }
    private Transform CreateNewEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]);
        
        return enemy.transform;
    }

    private void OnEnableEnemy(Transform enemy)
    {
        enemy.gameObject.SetActive(true);
        activeEnemies.Add(enemy.gameObject);
    }

    private void OnDisableEnemy(Transform enemy)
    {
        enemy.gameObject.SetActive(false);
        activeEnemies.Remove(enemy.gameObject);
    }

    private void OnDestroyEnemy(Transform enemy)
    {
        Destroy(enemy);
    }

    private void SpawnEnemy(Transform obj)
    {
        Transform enemy = _enemyPool.Get();
        //Debug.Log("SpawnEnemy: " + obj.position);

        enemy.transform.position = obj.position;  
        enemy.transform.rotation = obj.rotation; 
    }


    public void ReleaseEnemy(Transform enemy)
    {
        _enemyPool.Release(enemy);
    }
}
