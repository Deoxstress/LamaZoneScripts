using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpawnController : MonoBehaviour
{
    public GameObject guardPrefab;
    private GameObject[] GuardSpawns;
    private int incrementGuard;
    private float pdaTimer;
    private PlayerController player;
    private Vector3 farthestSpawn;
    private float timeThresholdToSpawnGuard;

    void Start()
    {
        GuardSpawns = GameObject.FindGameObjectsWithTag("GuardSpawn");
        player = GameObject.FindObjectOfType<PlayerController>();
        incrementGuard = 0;
        farthestSpawn = Vector3.zero;
        timeThresholdToSpawnGuard = 48.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.pdaStarted)
        {
            pdaTimer += Time.deltaTime;
            
            if(pdaTimer >= timeThresholdToSpawnGuard)
            {
                pdaTimer = 0.0f;
                foreach (GameObject Spawn in GuardSpawns)
                {
                    if (Vector3.Distance(Spawn.transform.position, player.transform.position) > Vector3.Distance(farthestSpawn, player.transform.position))
                    {
                        farthestSpawn = Spawn.transform.position;
                    }
                }

                GameObject guardClone = Instantiate(guardPrefab, farthestSpawn, Quaternion.identity);

                guardClone.GetComponent<GuardBehavior>().hasBeenSpawned = true;
                guardClone.GetComponent<GuardBehavior>().farthestSpawn = Vector3.zero;
                incrementGuard++;
                //timeThresholdToSpawnGuard = timeThresholdToSpawnGuard / (1.33f * (incrementGuard + 1));
                timeThresholdToSpawnGuard = timeThresholdToSpawnGuard / incrementGuard;

                if (timeThresholdToSpawnGuard <= 2.0f)
                {
                    timeThresholdToSpawnGuard = 2.3f;
                }
            }
        }


    }
}
