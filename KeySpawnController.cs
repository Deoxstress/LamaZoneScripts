using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawnController : MonoBehaviour
{

    public GameObject keyPrefab;
    private GameObject[] keySpawnPoints;
    private int keysToSpawn = 2;
    private GameObject selectedSpawnPoint;
    private GameObject alreadyTakenSpawn1;
    private GameObject alreadyTakenSpawn2;
    private GameObject instantiatedKey;
    public List<GameObject> keysOnScene;

    void Start()
    {

        keySpawnPoints = GameObject.FindGameObjectsWithTag("KeySpawn");

        while(keysToSpawn != 0)
        {
            if(keysToSpawn == 3)
            {
                selectedSpawnPoint = keySpawnPoints[Random.Range(0, keySpawnPoints.Length - 1)];
                alreadyTakenSpawn1 = selectedSpawnPoint;
                instantiatedKey = Instantiate(keyPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
                keysToSpawn -= 1;
            }
            
            else if(keysToSpawn == 2)
            {
                selectedSpawnPoint = keySpawnPoints[Random.Range(0, keySpawnPoints.Length - 1)];
                if(selectedSpawnPoint != alreadyTakenSpawn1)
                {
                    alreadyTakenSpawn2 = selectedSpawnPoint;
                    instantiatedKey = Instantiate(keyPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
                    keysToSpawn -= 1;
                }
                
            }

            else if (keysToSpawn == 1)
            {
                selectedSpawnPoint = keySpawnPoints[Random.Range(0, keySpawnPoints.Length - 1)];
                if (selectedSpawnPoint != alreadyTakenSpawn1 && selectedSpawnPoint != alreadyTakenSpawn2)
                {
                    instantiatedKey = Instantiate(keyPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
                    keysToSpawn -= 1;
                }

            }

        }
        keysOnScene.AddRange(GameObject.FindGameObjectsWithTag("Key"));

    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player").GetComponent<PlayerController>().pdaStarted)
        {
            foreach (GameObject obj in keysOnScene)
            {
                Destroy(obj);
            }
        }
    }
}
