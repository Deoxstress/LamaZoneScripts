using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichFolkSpawnerController : MonoBehaviour
{

    public GameObject[] Groups;
    public int amountOfGroupOf5 = 4;   
    public int amountOfGroupOf4 = 6;    
    public int amountOfGroupOf3 = 14;    
    public int amountOfGroupOf2 = 5;    
    public int amountOfGroupOf1 = 4;

    private GameObject[] SpawnFolks;
    
    private int randomIndex;



    void Start()
    {
        

        SpawnFolks = GameObject.FindGameObjectsWithTag("RichFolkSpawn");

        foreach(GameObject Spawn in SpawnFolks)
        {
            randomIndex = Random.Range(0, 5);


            if (randomIndex == 0)
            {
                if (amountOfGroupOf1 <= 0)
                {
                    randomIndex = 1;

                    if (amountOfGroupOf2 <= 0)
                    {
                        randomIndex = 2;

                        if (amountOfGroupOf3 <= 0)
                        {
                            randomIndex = 3;

                            if (amountOfGroupOf4 <= 0)
                            {
                                randomIndex = 4;

                                if (amountOfGroupOf5 <= 0)
                                {

                                }

                                else
                                {
                                    amountOfGroupOf5 -= 1;
                                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                                }
                            }

                            else
                            {
                                amountOfGroupOf4 -= 1;
                                Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                            }
                        }

                        else
                        {
                            amountOfGroupOf3 -= 1;
                            Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                        }
                    }

                    else
                    {
                        amountOfGroupOf2 -= 1;
                        Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                    }
                }

                else
                {
                    amountOfGroupOf1 -= 1;
                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                    Groups[randomIndex].GetComponent<RichFolkBehavior>().isAlone = true;
                }
            }

            else if (randomIndex == 1)
            {
                if (amountOfGroupOf2 <= 0)
                {
                    randomIndex = 2;

                    if(amountOfGroupOf3 <= 0)
                    {
                        randomIndex = 3;

                        if(amountOfGroupOf4 <= 0)
                        {
                            randomIndex = 4;

                            if(amountOfGroupOf5 <= 0)
                            {
                                randomIndex = 0;

                                if (amountOfGroupOf1 <= 0)
                                {

                                }

                                else
                                {
                                    amountOfGroupOf1 -= 1;
                                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                                    Groups[randomIndex].GetComponent<RichFolkBehavior>().isAlone = true;
                                }
                            }

                            else
                            {
                                amountOfGroupOf5 -= 1;
                                Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                            }
                        }

                        else
                        {
                            amountOfGroupOf4 -= 1;
                            Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                        }
                    }

                    else
                    {
                        amountOfGroupOf3 -= 1;
                        Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                    }
                }

                else
                {
                    amountOfGroupOf2 -= 1;
                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                }
            }

            else if (randomIndex == 2)
            {
                if (amountOfGroupOf3 <= 0)
                {
                    randomIndex = 1;

                    if (amountOfGroupOf2 <= 0)
                    {
                        randomIndex = 3;

                        if (amountOfGroupOf4 <= 0)
                        {
                            randomIndex = 4;

                            if (amountOfGroupOf5 <= 0)
                            {
                                randomIndex = 0;

                                if (amountOfGroupOf1 <= 0)
                                {

                                }

                                else
                                {
                                    amountOfGroupOf1 -= 1;
                                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                                    Groups[randomIndex].GetComponent<RichFolkBehavior>().isAlone = true;
                                }
                            }

                            else
                            {
                                amountOfGroupOf5 -= 1;
                                Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                            }
                        }

                        else
                        {
                            amountOfGroupOf4 -= 1;
                            Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                        }
                    }

                    else
                    {
                        amountOfGroupOf2 -= 1;
                        Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                    }
                }

                else
                {
                    amountOfGroupOf3 -= 1;
                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                }
            }

            else if (randomIndex == 3)
            {
                if (amountOfGroupOf4 <= 0)
                {
                    randomIndex = 1;

                    if (amountOfGroupOf2 <= 0)
                    {
                        randomIndex = 2;

                        if (amountOfGroupOf3 <= 0)
                        {
                            randomIndex = 4;

                            if (amountOfGroupOf5 <= 0)
                            {
                                randomIndex = 0;

                                if (amountOfGroupOf1 <= 0)
                                {

                                }

                                else
                                {
                                    amountOfGroupOf1 -= 1;
                                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                                    Groups[randomIndex].GetComponent<RichFolkBehavior>().isAlone = true;
                                }
                            }

                            else
                            {
                                amountOfGroupOf5 -= 1;
                                Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                            }
                        }

                        else
                        {
                            amountOfGroupOf3 -= 1;
                            Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                        }
                    }

                    else
                    {
                        amountOfGroupOf2 -= 1;
                        Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                    }
                }

                else
                {
                    amountOfGroupOf4 -= 1;
                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                }
            }

            else if (randomIndex == 4)
            {
                if (amountOfGroupOf5 <= 0)
                {
                    randomIndex = 1;

                    if (amountOfGroupOf2 <= 0)
                    {
                        randomIndex = 3;

                        if (amountOfGroupOf4 <= 0)
                        {
                            randomIndex = 2;

                            if (amountOfGroupOf3 <= 0)
                            {
                                randomIndex = 0;

                                if (amountOfGroupOf1 <= 0)
                                {

                                }

                                else
                                {
                                    amountOfGroupOf1 -= 1;
                                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                                    Groups[randomIndex].GetComponent<RichFolkBehavior>().isAlone = true;
                                }
                            }

                            else
                            {
                                amountOfGroupOf3 -= 1;
                                Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                            }
                        }

                        else
                        {
                            amountOfGroupOf4 -= 1;
                            Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                        }
                    }

                    else
                    {
                        amountOfGroupOf2 -= 1;
                        Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                    }
                }

                else
                {
                    amountOfGroupOf5 -= 1;
                    Instantiate(Groups[randomIndex], Spawn.transform.position, Quaternion.identity);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
