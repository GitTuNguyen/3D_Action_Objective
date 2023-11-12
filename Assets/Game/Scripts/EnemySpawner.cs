using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabList;
    [SerializeField]
    private List<SpawnPoint> spawnPointList;
    public List<GameObject> spawnedEnemyList;
    
    [SerializeField]
    private bool isSpawner = false;
    public UnityEvent OnAllEnemyDead;
    private bool isGateOpened = false;
    private void Update() {
        if (!isSpawner || spawnedEnemyList.Count > 0 || isGateOpened)
        {
            return;
        } else {
            isGateOpened = true;
            OnAllEnemyDead.Invoke();
        }
    } 
    private void Awake() {
        var spawnPointArray = GetComponentsInChildren<SpawnPoint>();
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
    }
    private void SpawnEnemy()
    {
        isSpawner = true;
        foreach(SpawnPoint spawnPoint in spawnPointList)
        {
            GameObject enemy = GetRandomObject(enemyPrefabList);
            var enemySpawned = Instantiate(enemy, spawnPoint.transform.position, Quaternion.identity);
            spawnedEnemyList.Add(enemySpawned);
            Character character = enemySpawned.GetComponent<Character>();
            if (character != null)
            {
                character.SetSpawner(this);
            }
        }
        
    }

    private GameObject GetRandomObject(List<GameObject> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && !isSpawner)
        {
            SpawnEnemy();
        }
    }
}
