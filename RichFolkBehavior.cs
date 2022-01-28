using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RichFolkBehavior : MonoBehaviour
{
    public int GuestType;
    private NavMeshAgent richFolk;
    public GameObject[] wayOut;
    public GameObject[] realTimeWayOut;
    private GameObject[] groupOfFolks;
    public Transform goal;
    public Vector3 nearestExit;
    public Vector3 nearestExitAvailable;
    public Vector3 farthestExitAvailable;
    public float delayBeforeMoving = 1.5f;
    public float delayOpeningDoor = 0f;
    public float delayPanicking;
    public float delayForAnotherGroup;
    private bool eliminated, openingDoor, doorOpened, firstDestination, leaving, panicByPlayer, checkingObstacle = false;
    public bool isPanicking, isRich;
    public bool searchingForNewExit;
    private SphereCollider panicZone;
    private CapsuleCollider collisionMask;
    public NavMeshObstacle obstacleEncountered;
    public Material deadMat;
    private GameObject player;
    private float speedInit;
    private float runSpeed;
    public int guestHP;
    private Animator anim;
    public GameObject womanPrefab;
    public GameObject manPrefab;
    public bool hasSpawnedAnew = false;
    public bool isAlone;
    private float guestSpeed;
    public Material skin1;
    public Material skin2;
    public Material skin3;
    public Material skin4;
    public Material skin5;
    public Material material;
    public Material whiteMaterial;
    public GameObject ChampagnePowerUp;
    public int ChampagneValue;
    public GameObject richEffects, spawnerParent;
    private AudioSource audioSource;
    public AudioClip[] sounds;

    void Awake()
    {
        isPanicking = false;

    }

    void Start()
    {
        int randomManOrWoman = Random.Range(0, 2);

        if (randomManOrWoman == 1 && !hasSpawnedAnew)
        {
            GameObject womanPrefabClone = Instantiate(womanPrefab, transform.position, Quaternion.identity);
            womanPrefabClone.transform.rotation = transform.rotation;
            womanPrefabClone.GetComponent<RichFolkBehavior>().ChampagneValue = Random.Range(2, 4);
            womanPrefabClone.GetComponent<RichFolkBehavior>().hasSpawnedAnew = true;
            if (spawnerParent != null)
            {
                spawnerParent.GetComponent<ChatScript>().folks.Add(womanPrefabClone);
            }

            int randomSkin = Random.Range(0, 3);

            if (this.isAlone)
            {
                womanPrefabClone.GetComponent<RichFolkBehavior>().isAlone = true;

            }
            if (this.isRich)
            {
                womanPrefabClone.GetComponent<RichFolkBehavior>().ChampagneValue = Random.Range(3, 6);
                womanPrefabClone.GetComponent<RichFolkBehavior>().isRich = true;
                richEffects = womanPrefabClone.transform.Find("VIP (1)").gameObject;
                richEffects.SetActive(true);
            }

            if (!this.isRich)
            {
                richEffects = womanPrefabClone.transform.Find("VIP (1)").gameObject;
                richEffects.SetActive(false);

                if (randomSkin == 0)
                {
                    womanPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin1;
                }

                else if (randomSkin == 1)
                {
                    womanPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin2;
                }

                else if (randomSkin == 2)
                {
                    womanPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin3;
                }
            }
            else
            {
                womanPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin4;


            }
            Destroy(this.gameObject);
        }

        else if (randomManOrWoman == 0 && !hasSpawnedAnew)
        {
            GameObject manPrefabClone = Instantiate(manPrefab, transform.position, Quaternion.identity);
            manPrefabClone.transform.rotation = transform.rotation;
            manPrefabClone.GetComponent<RichFolkBehavior>().ChampagneValue = Random.Range(2, 4);
            manPrefabClone.GetComponent<RichFolkBehavior>().hasSpawnedAnew = true;
            if (spawnerParent != null)
            {
                spawnerParent.gameObject.GetComponent<ChatScript>().folks.Add(manPrefabClone);
            }

            int randomSkin = Random.Range(0, 3);


            if (this.isAlone)
            {
                manPrefabClone.GetComponent<RichFolkBehavior>().isAlone = true;
            }
            else
            {
                manPrefabClone.GetComponent<RichFolkBehavior>().isAlone = false;
            }
            if (this.isRich)
            {
                manPrefabClone.GetComponent<RichFolkBehavior>().ChampagneValue = Random.Range(3, 6);
                manPrefabClone.GetComponent<RichFolkBehavior>().isRich = true;
                richEffects = manPrefabClone.transform.Find("VIP (1)").gameObject;
                richEffects.SetActive(true);
            }

            if (!this.isRich)
            {
                richEffects = manPrefabClone.transform.Find("VIP (1)").gameObject;
                richEffects.SetActive(false);
                if (randomSkin == 0)
                {
                    manPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin1;
                }

                else if (randomSkin == 1)
                {
                    manPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin2;
                }

                else if (randomSkin == 2)
                {
                    manPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin3;
                }
            }
            else
            {
                manPrefabClone.GetComponentInChildren<SkinnedMeshRenderer>().material = skin5;
            }
            Destroy(this.gameObject);
        }

        if (isPanicking)
        {
            isPanicking = false;
        }
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
        isPanicking = false;
        nearestExit = new Vector3(1000, 1000, 1000);
        nearestExitAvailable = new Vector3(1000, 1000, 1000);
        farthestExitAvailable = gameObject.transform.position;
        richFolk = gameObject.GetComponent<NavMeshAgent>();
        wayOut = GameObject.FindGameObjectsWithTag("Sortie");
        realTimeWayOut = GameObject.FindGameObjectsWithTag("Sortie");
        searchingForNewExit = false;
        panicZone = GetComponentInChildren<SphereCollider>();
        collisionMask = GetComponent<CapsuleCollider>();
        panicZone.enabled = false;
        collisionMask.enabled = true;
        obstacleEncountered = GameObject.Find("AvoidedObstacleBeginning").GetComponent<NavMeshObstacle>();
        audioSource = gameObject.GetComponent<AudioSource>();

        guestSpeed = Random.Range(1.0f, 1.35f);
        richFolk.speed = 1.75f * guestSpeed;
        anim.speed = guestSpeed;
        speedInit = richFolk.speed;
        delayPanicking = 1.3f / guestSpeed;
        //anim.speed += richFolk.speed/6;

        if (isAlone && !isRich)
        {
            anim.SetInteger("Conversation", 6);
        }
        else if (isRich)
        {
            anim.SetInteger("Conversation", 7);
        }
        else
        {
            anim.SetInteger("Conversation", Random.Range(0, 5));
        }
        player = GameObject.Find("Player");

        //richFolk.destination = goal.position;

        groupOfFolks = GameObject.FindGameObjectsWithTag("Group");

        material = GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    void Update()
    {
        if (richFolk.enabled)
        {
            if (delayOpeningDoor < 1.5f && richFolk.isStopped && openingDoor)
            {
                delayOpeningDoor += Time.deltaTime;
            }
            else if (delayOpeningDoor >= 1.5f && richFolk.isStopped && openingDoor)
            {
                anim.SetBool("OpeningDoor", false);
                doorOpened = true;
                delayOpeningDoor = 0f;
                openingDoor = false;
                richFolk.isStopped = false;
            }
            if (delayBeforeMoving > 0 && searchingForNewExit && !openingDoor)
            {
                delayBeforeMoving -= Time.deltaTime;
            }
            else if (delayBeforeMoving <= 0 && searchingForNewExit)
            {
                if (isPanicking)
                {
                    NewDestination();
                }
                else if (!isPanicking)
                {
                    richFolk.isStopped = false;
                    obstacleEncountered.enabled = true;
                    searchingForNewExit = false;
                    delayForAnotherGroup = 0.0f;
                }
                delayBeforeMoving = 1.5f;
                checkingObstacle = false;
                anim.SetBool("NewExit", false);
                //anim.SetBool("IsPanicking", true);
            }
            if (delayPanicking > 0 && isPanicking)
            {
                delayPanicking -= Time.deltaTime;
            }
        }

        if (isRich && !isPanicking && delayForAnotherGroup <= 0 && !ClockScript.partyOver)
        {
            richFolk.isStopped = false;
            richFolk.speed = 1.75f * guestSpeed / 1.35f;
            richFolk.SetDestination(groupOfFolks[Random.Range(0, 26)].transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            delayForAnotherGroup = 20.0f;
            anim.SetBool("ReachedDestination", false);

        }

        if (delayForAnotherGroup >= 0 && isRich && !isPanicking&& !ClockScript.partyOver)
        {
            delayForAnotherGroup -= Time.deltaTime;
            if (Vector3.Distance(transform.position, richFolk.destination) < 2)
            {
                anim.SetBool("ReachedDestination", true);
                richFolk.isStopped = true;
            }
        }

        if (ClockScript.partyOver&&!leaving)
        {
            richFolk.speed = 1.75f * guestSpeed / 1.35f;
            NewDestination();
            anim.SetBool("IsLeaving", true);
            leaving = true;
        }


        if (isPanicking)
        {
            richFolk.speed = 1.75f * guestSpeed;
            panicZone.enabled = true;
            anim.SetBool("IsPanicking", true);
            anim.SetBool("IsLeaving", false);

            if (!firstDestination && !panicByPlayer)
            {
                richFolk.isStopped = true;
                if (delayPanicking <= 0)
                {
                    richFolk.isStopped = false;
                    NewDestination();
                    firstDestination = true;
                }
            }

            else if (!firstDestination && panicByPlayer && guestHP > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((player.transform.position - transform.position).normalized), Time.deltaTime * 10);
                richFolk.isStopped = true;
                if (delayPanicking <= 0)
                {
                    RunToFarthestExit();
                    firstDestination = true;
                }

            }
        }

        else if (!isPanicking)
        {
            panicZone.enabled = false;
        }
    }

    void NewDestination()
    {
        nearestExitAvailable = new Vector3(1000, 1000, 1000);
        foreach (GameObject way in wayOut)
        {
            if (nearestExit != way.transform.position)
            {
                if (Vector3.Distance(way.transform.position, gameObject.transform.position) < Vector3.Distance(nearestExitAvailable, gameObject.transform.position))
                {
                    nearestExitAvailable = way.transform.position;
                }
            }
        }
        richFolk.isStopped = false;
        nearestExit = nearestExitAvailable;
        richFolk.SetDestination(nearestExit);
        searchingForNewExit = false;
        obstacleEncountered.enabled = true;
    }

    void RunToFarthestExit()
    {
        foreach (GameObject way in wayOut)
        {
            if (Vector3.Distance(way.transform.position, gameObject.transform.position) > Vector3.Distance(farthestExitAvailable, gameObject.transform.position))
            {
                farthestExitAvailable = way.transform.position;
            }
        }

        richFolk.isStopped = false;
        richFolk.SetDestination(farthestExitAvailable);
        searchingForNewExit = false;
        obstacleEncountered.enabled = true;

    }

    void SpawnChampagne(int ChampagneValue)
    {
        while (ChampagneValue >= 1)
        {
            GameObject ChampagneClone = Instantiate(ChampagnePowerUp, transform.position, Quaternion.identity);
            Physics.IgnoreCollision(ChampagneClone.GetComponent<Collider>(), ChampagnePowerUp.GetComponent<Collider>());
            ChampagneClone.transform.position = new Vector3(transform.position.x + Random.Range(-2, 3f), ChampagneClone.transform.position.y, transform.position.z + Random.Range(-2f, 3f));
            ChampagneValue -= 1;
        }
    }

    void HitAnim()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().material = material;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Spit" && !eliminated)
        {
            if (guestHP <= 4 && guestHP > 0)
            {     
                if (!player.GetComponent<PlayerController>().pdaStarted)
                {
                    player.GetComponent<PlayerController>().pdaStarted = true;
                }
                Destroy(other.gameObject);
                guestHP -= 1;
                Invoke("HitAnim", 0.15f);
                GetComponentInChildren<SkinnedMeshRenderer>().material = whiteMaterial;
            }

            else if (guestHP <= 0)
            {
                audioSource.PlayOneShot(sounds[Random.Range(0,2)],0.8f);
                Debug.Log("Neutralized");
                SpawnChampagne(ChampagneValue);
                Destroy(other.gameObject);
                richFolk.isStopped = true;
                collisionMask.enabled = false;
                richFolk.enabled = false;
                panicZone.enabled = true;
                anim.SetBool("Hit", true);
                //GetComponent<MeshRenderer>().material = deadMat;
                //this.transform.rotation = Quaternion.Euler(90, 0, 0);
                //this.transform.position = new Vector3(transform.position.x, 1, transform.position.z);

                if (isRich)
                {
                    player.GetComponent<PlayerController>().SpecialGuestHit();
                    richEffects.SetActive(false);
                }
                else
                {
                    player.GetComponent<PlayerController>().GuestHit();
                }
                //PlayerController.hitCounter.text = "Salivés : " + PlayerController.hitCounterNumber;

                eliminated = true;
                CameraShakeBehavior.TriggerCameraShake(0.10f, 0.22f);
            }



        }

        else if (other.gameObject.tag == "KillSpit" && !eliminated)
        {
            Destroy(other.gameObject);
            guestHP -= 4;
            audioSource.PlayOneShot(sounds[Random.Range(0, 2)], 0.8f);
            SpawnChampagne(ChampagneValue);
            Destroy(other.gameObject);
            richFolk.isStopped = true;
            collisionMask.enabled = false;
            richFolk.enabled = false;
            panicZone.enabled = true;
            anim.SetBool("Hit", true);
            //GetComponent<MeshRenderer>().material = deadMat;
            //this.transform.rotation = Quaternion.Euler(90, 0, 0);
            //this.transform.position = new Vector3(transform.position.x, 1, transform.position.z);

            if (isRich)
            {
                player.GetComponent<PlayerController>().SpecialGuestHit();
                richEffects.SetActive(false);
            }
            else
            {
                player.GetComponent<PlayerController>().GuestHit();
            }
            //PlayerController.hitCounter.text = "Salivés : " + PlayerController.hitCounterNumber;

            eliminated = true;
            CameraShakeBehavior.TriggerCameraShake(0.10f, 0.2f);



        }

        else if (other.gameObject.tag == "Door")
        {
            if (other.GetComponent<DoorScript>().keyClosed && !searchingForNewExit)
            {
                delayBeforeMoving = 1.5f;
                richFolk.isStopped = true;
                searchingForNewExit = true;
                checkingObstacle = true;
                obstacleEncountered = other.GetComponentInChildren<NavMeshObstacle>();
                anim.SetBool("NewExit", true);
                //anim.SetBool("IsPanicking", false);
            }
            else if (!other.GetComponent<DoorScript>().keyClosed && !other.GetComponent<DoorScript>().open)
            {
                delayOpeningDoor = 0f;
                richFolk.isStopped = true;
                openingDoor = true;
                anim.SetBool("OpeningDoor", true);
            }
        }

        else if (other.gameObject.tag == "Player" && !checkingObstacle)
        {
            isPanicking = true;
            if (!firstDestination)
            {
                panicByPlayer = true;               
                audioSource.PlayOneShot(sounds[Random.Range(2, 6)],0.6f);
               
            }
            runSpeed = speedInit + 1.35f;
            richFolk.speed = runSpeed;

            if (Vector3.Distance(nearestExitAvailable, gameObject.transform.position) > Vector3.Distance(nearestExitAvailable, other.gameObject.transform.position) && firstDestination)
            {
                RunToFarthestExit();
            }


        }

        //else if (other.gameObject.tag == "Obstacle" && other.GetComponent<HoldingObject>().pdaStarted)
        else if (other.gameObject.tag == "Obstacle"&& !other.GetComponent<HoldingObject>().currentlyHeld)
        {
            anim.SetBool("NewExit", true);
            checkingObstacle = true;
            //richFolk.avoidancePriority = 30;
            //richFolk.radius = 2.0f;            
            delayBeforeMoving = 1.5f;
            richFolk.isStopped = true;
            searchingForNewExit = true;           
        }

        else if (other.gameObject.tag == "PanicZone")
        {
            isPanicking = true;
            audioSource.PlayOneShot(sounds[Random.Range(2, 6)], 0.6f);
        }

        else if (other.gameObject.tag == "AlertZone")
        {
            isPanicking = true;
        }
        else if (other.gameObject.tag == "Sortie")
        {
            if (isRich)
            {
                player.GetComponent<PlayerController>().SpecialGuestEscape();
            }
            else
            {
                player.GetComponent<PlayerController>().GuestEscape();
            }
            Destroy(gameObject);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Door" && collider.GetComponent<DoorScript>().keyClosed && !searchingForNewExit)
        {
            //obstacleEncountered.enabled = false;
        }

        else if (collider.gameObject.tag == "Obstacle" && collider.GetComponent<HoldingObject>().pdaStarted)
        {
            //richFolk.radius = 0.5f;
        }

        else if (collider.gameObject.tag == "Player")
        {
            richFolk.speed = speedInit;
        }

        else if (collider.gameObject.tag == "RichFolk")
        {
            if (collider.gameObject.GetComponent<RichFolkBehavior>().richFolk.enabled == false)
            {
                guestSpeed = 1.75f * guestSpeed;
            }

        }
    }
    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Door")
        {
            if (!collision.GetComponent<DoorScript>().open&&isPanicking && !collision.GetComponent<DoorScript>().keyClosed || !collision.GetComponent<DoorScript>().open&&isRich&& !collision.GetComponent<DoorScript>().keyClosed)
            {
                if (!openingDoor)
                {
                    delayOpeningDoor = 0f;
                    openingDoor = true;
                    richFolk.isStopped = true;
                    anim.SetBool("OpeningDoor", true);
                }
                else if (doorOpened)
                {
                    richFolk.isStopped = false;
                    collision.GetComponent<DoorScript>().playAnim = true;
                    doorOpened = false;
                    openingDoor = false;
                    anim.SetBool("OpeningDoor", false);
                }           
            }
        }

        else if (collision.gameObject.tag == "RichFolk")
        {
            if (collision.gameObject.GetComponent<RichFolkBehavior>().richFolk.enabled == false)
            {
                guestSpeed = 1.45f * guestSpeed;
            }
        }

    }


}



