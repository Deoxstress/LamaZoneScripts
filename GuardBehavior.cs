using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class GuardBehavior : MonoBehaviour
{

    public GameObject[] SpawnGuards;
    private GameObject player;
    private GameObject[] allGuards;
    private NavMeshAgent guard;
    public bool isOnAlert, hasPunched;
    public int hitPoints;
    public float delayBeforeSpawning;
    public float speed, delayOpeningDoor;
    public SphereCollider alertZone;
    private bool isDead;
    public GameObject CollisionToIgnore;
    public bool hasBeenSpawned, isOpeningDoor;    
    private CapsuleCollider collisionMask;
    private Animator anim;
    public GameObject currentDoor;
    public Vector3 farthestSpawn;
    public bool isPunching;
    private AudioSource audioSource;
    public AudioClip[] sounds;
    public AudioClip[] punches;
    public Material material, whiteMaterial;

    void Start()
    {
        delayOpeningDoor = 1.5f;
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        guard = gameObject.GetComponent<NavMeshAgent>();
        allGuards = GameObject.FindGameObjectsWithTag("Guard");
        SpawnGuards = GameObject.FindGameObjectsWithTag("GuardSpawn");
        farthestSpawn = Vector3.zero;
        audioSource = gameObject.GetComponent<AudioSource>();
        collisionMask = GetComponent<CapsuleCollider>();
        collisionMask.enabled = true;

        alertZone = GetComponentInChildren<SphereCollider>();
        alertZone.enabled = false;

        if (hasBeenSpawned)
        {
            alertZone.enabled = true;
            guard.enabled = true;
            isOnAlert = true;
        }

        hitPoints = 7;
        speed = 3.5f;
        guard.speed = speed;

        GetComponentInChildren<SkinnedMeshRenderer>().material = material;

    }

    // Update is called once per frame
    void Update()
    {
        if (delayOpeningDoor > 0f&&isOpeningDoor)
        {
            delayOpeningDoor -= Time.deltaTime;
        }
        else if (delayOpeningDoor <= 0f)
        {
            anim.SetBool("Opening_Door", false);
            currentDoor.GetComponent<DoorScript>().keyClosed = false;
            currentDoor.GetComponent<DoorScript>().navMesh.enabled = false;
            isOpeningDoor = false;
            guard.isStopped = false;
        }
        if (isOnAlert && guard.enabled)
        {
            if (!player.GetComponent<PlayerController>().isDying)
            {
                guard.SetDestination(player.transform.position);
                anim.SetBool("Alert", true);
            }
            else
            {
                guard.isStopped = true;
                anim.SetBool("Idle", true);
                anim.SetBool("Alert", false);
            }

        }

        if (hitPoints == 2)
        {

            speed = 3.0f;
            guard.speed = speed;
        }

        else if (hitPoints == 1)
        {

            speed = 2.5f;
            guard.speed = speed;
        }


        else if (hitPoints <= 0 && guard.enabled&&!isDead)
        {
            speed = 0;
            isPunching = false;
            guard.speed = speed;
            isDead = true;
            anim.SetBool("Death", true);
            audioSource.PlayOneShot(sounds[Random.Range(0, 2)],1.2f);

        }

        if(delayBeforeSpawning >= 0)
        {
            delayBeforeSpawning -= Time.deltaTime;
        }

        

        if(isDead && delayBeforeSpawning <= 0)
        {
            isPunching = false;
            guard.isStopped = true;
            guard.enabled = false;
            isDead = false;
            collisionMask.enabled = false;
            if (!player.GetComponent<PlayerController>().isDying)
            {
                Vector3 farthestSpawn = Vector3.zero;
                player.GetComponent<PlayerController>().guardHit++;
                foreach (GameObject Spawn in SpawnGuards)
                {
                    if (Vector3.Distance(Spawn.transform.position, player.transform.position) > Vector3.Distance(farthestSpawn, player.transform.position))
                    {
                        farthestSpawn = Spawn.transform.position;
                    }
                }
                GameObject guardClone = Instantiate(gameObject, farthestSpawn, Quaternion.identity);

                guardClone.GetComponent<GuardBehavior>().hasBeenSpawned = true;
                guardClone.GetComponent<GuardBehavior>().farthestSpawn = Vector3.zero;
                guard.GetComponentInChildren<SkinnedMeshRenderer>().material = material;
                //this.transform.rotation = Quaternion.Euler(90, 0, 0);
                //this.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                this.GetComponent<CapsuleCollider>().enabled = false;
                this.GetComponent<Rigidbody>().freezeRotation = true;

                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

        }

    }

    void HitAnim()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().material = material;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "PanicZone")
        {
            alertZone.enabled = true;
            isOnAlert = true;

            foreach (GameObject guards in allGuards)
            {
                guards.GetComponent<GuardBehavior>().alertZone.enabled = true;
                guards.GetComponent<GuardBehavior>().isOnAlert = true;
            }
        }

        else if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerController>().terrorZone.enabled)
        {
            alertZone.enabled = true;
            isOnAlert = true;
            foreach (GameObject guards in allGuards)
            {
                guards.GetComponent<GuardBehavior>().alertZone.enabled = true;
                guards.GetComponent<GuardBehavior>().isOnAlert = true;
            }
        }

        else if(other.gameObject.tag == "Spit")
        {
            Destroy(other.gameObject);
            if (hitPoints == 1)
            {
                delayBeforeSpawning = 2.0f;
            }
            if (!player.GetComponent<PlayerController>().pdaStarted)
            {
                player.GetComponent<PlayerController>().pdaStarted = true;
            }
            hitPoints -= 1;
            Invoke("HitAnim", 0.15f);
            GetComponentInChildren<SkinnedMeshRenderer>().material = whiteMaterial;
        }     
        else if (other.gameObject.tag == "Door"&&other.GetComponent<DoorScript>().keyClosed)
        {
            currentDoor = other.gameObject;
            anim.SetBool("Opening_Door", true);
            delayOpeningDoor = 3f;
            guard.isStopped = true;
            isOpeningDoor = true;
        }
        
        else if (other.gameObject.tag == "Obstacle")
        {
            other.GetComponent<NavMeshObstacle>().enabled = false;
        }

        else if (other.gameObject.tag == "KillSpit")
        {
            Destroy(other.gameObject);
            hitPoints -= 1;
            if (hitPoints == 1)
            {
                delayBeforeSpawning = 2.0f;
            }
            Invoke("HitAnim", 0.15f);
            GetComponentInChildren<SkinnedMeshRenderer>().material = whiteMaterial;
        }


    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Obstacle")
        {
            other.GetComponent<NavMeshObstacle>().enabled = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerController>().terrorZone.enabled)
        {
            guard.isStopped = true;
            anim.SetInteger("Punch", Random.Range(1, 3));
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerController>().terrorZone.enabled)
        {
            if (collision.gameObject.GetComponent<PlayerController>() && collision.gameObject.GetComponent<PlayerController>().invulnerabilityTime <= 0.0f&& !player.GetComponent<PlayerController>().isDying)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((collision.transform.position - transform.position).normalized), Time.deltaTime * 10);
                if (isPunching)
                {
                    collision.gameObject.GetComponent<PlayerController>().playerHp -= 1;
                    collision.gameObject.GetComponent<PlayerController>().isHit = false;
                    CameraShakeBehavior.TriggerCameraShake(0.14f, 0.36f);
                    collision.gameObject.GetComponent<PlayerController>().invulnerabilityTime = 3.0f;
                    Debug.Log(collision.gameObject.GetComponent<PlayerController>().playerHp);
                    audioSource.PlayOneShot(punches[Random.Range(0, 3)], 0.8f);
                }
            }
            if (player.GetComponent<PlayerController>().isDying)
            {
                guard.isStopped = true;
                anim.SetBool("Idle", true);
                anim.SetBool("Alert", false);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerController>().terrorZone.enabled)
        {
            guard.isStopped = false;
            anim.SetInteger("Punch", 0);
            transform.Find("Idle_Guard").GetComponent<GuardPunch>().hasPunched = false;

        }
    }
}
