using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnController : MonoBehaviour
{

    public GameObject obstaclePrefab;
    private GameObject[] obstacleSpawnPoints;
    private int ObstacleToSpawn = 1;
    private GameObject selectedSpawnPoint;
    private GameObject alreadyTakenSpawn1;

    void Start()
    {
        obstacleSpawnPoints = GameObject.FindGameObjectsWithTag("ObstacleSpawn");

        while(ObstacleToSpawn != 0)
        {
            if (ObstacleToSpawn == 2)
            {
                selectedSpawnPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Length - 1)];
                alreadyTakenSpawn1 = selectedSpawnPoint;
                GameObject g = GameObject.Instantiate(obstaclePrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
                g.transform.Rotate(0f, 90f, 0f);

                ObstacleToSpawn -= 1;
                
            }

            else if (ObstacleToSpawn == 1)
            {
                selectedSpawnPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Length - 1)];
                if (selectedSpawnPoint != alreadyTakenSpawn1)
                {
                    GameObject g = GameObject.Instantiate(obstaclePrefab, selectedSpawnPoint.transform.position, Quaternion.identity);
                    g.transform.Rotate(0f, 90f, 0f);
                    ObstacleToSpawn -= 1;
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
