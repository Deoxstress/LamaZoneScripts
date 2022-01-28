using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class HoldingObject : MonoBehaviour
{
    GameObject hand,hand2, objectPosition, player;
    public bool isHeld = false;
    Rigidbody rb;
    SphereCollider collisionSphere;
    private PlayerController playerScript;
    Canvas canvas;
    Text text;
    Image LBIcon;
    Sprite hold_LB, release_LB, blank_Image;
    public bool pdaStarted = false;
    public bool heldOnce, currentlyHeld;
    float releaseTimer;
    private NavMeshObstacle obstacle_nav;
    public Vector3 coordinates;
    private int languageIndex;
    private List<string> commands = new List<string> {"Hold", "Porter", "Let go", "Lâcher","Blocking objects can block an exit or a corridor for the guests","Les objets bloquants peuvent bloquer une sortie ou un couloir pour les personnes"};
    public AudioClip[] holdSounds;
    public AudioSource audioSource;
    public Text tutorial;
    // Start is called before the first frame update
    void Start()
    {
        hand = GameObject.Find("Player").GetComponent<PlayerController>().arm;
        hand2 = GameObject.Find("Player").GetComponent<PlayerController>().arm2;
        player = GameObject.Find("Player");
        audioSource = player.GetComponentInChildren<AudioSource>();
        objectPosition = GameObject.Find("Hold_Object_Position");
        rb = GetComponentInChildren<Rigidbody>();
        collisionSphere = GetComponentInChildren<SphereCollider>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        canvas = GameObject.Find("CanvasBouton").GetComponent<Canvas>();
        text = GameObject.Find("LB_Text").GetComponent<Text>();
        LBIcon = GameObject.Find("LB_Icon").GetComponent<Image>();
        if (PlayerPrefs.GetInt("Controls") == 0)
        {
            hold_LB = Resources.Load<Sprite>("HOLD_LB");
            release_LB = Resources.Load<Sprite>("RELEASE_LB");
        }
        else  if (PlayerPrefs.GetInt("Controls") == 1)
        {
            hold_LB = Resources.Load<Sprite>("HOLD_MOUSE");
            release_LB = Resources.Load<Sprite>("RELEASE_MOUSE");
        }
        blank_Image = Resources.Load<Sprite>("BLANK_IMAGE");
        obstacle_nav = GetComponent<NavMeshObstacle>();
        tutorial = GameObject.Find("Tutorial").GetComponent<Text>();
        obstacle_nav.enabled = false;


        if (PlayerPrefs.GetInt("Language") == 0)
        {
            languageIndex = 0;
        }
        else
        {
            languageIndex = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (releaseTimer > 0)
        {
            releaseTimer -= Time.deltaTime;
        }
        if (isHeld&&!playerScript.pdaStarted&&releaseTimer<=0){
            if (!heldOnce)
            {
                heldOnce=true;
            }
            objectPosition.transform.position = (hand.transform.position + hand2.transform.position) / 2;
            objectPosition.transform.LookAt(player.transform.position);
            obstacle_nav.enabled = false;
            if (Input.GetButtonDown("Interact"))
            {
                currentlyHeld = true;
                text.text = commands[languageIndex+2];
                LBIcon.sprite = release_LB;
                playerScript.isHoldingObject = true;
                collisionSphere.enabled = false;
                //transform.parent = hand.transform;
                //transform.position = new Vector3(hand.transform.position.x, 2.5f, hand.transform.position.z+0.8f);
                //transform.position = hand.transform.position + hand.transform.forward*0.75f+hand.transform.up;
                this.rb.useGravity = false;
                audioSource.PlayOneShot(holdSounds[0], 0.6f);
            }
            if (Input.GetButtonUp("Interact"))
            {
                currentlyHeld = false;
                playerScript.isHoldingObject = false;
                collisionSphere.enabled = true;
                //transform.parent = null;
                //transform.position = hand.transform.position + hand.transform.forward + hand.transform.up;
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                rb.useGravity = true;
                releaseTimer = 1f;
                audioSource.PlayOneShot(holdSounds[1], 0.6f);
            }
            
            if (currentlyHeld)
            {
                transform.position = objectPosition.transform.position-objectPosition.transform.forward*0.5f;
                Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                transform.LookAt(targetPosition);
            }
        }
        else if (playerScript.pdaStarted)
        {
            pdaStarted = true;
            if (!heldOnce)
            {
                Destroy(gameObject);
            }
            obstacle_nav.enabled = true;
        }

        if (transform.position.y < 1&&currentlyHeld)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player"&& !playerScript.pdaStarted)
        {            
            isHeld = true;
            canvas.gameObject.SetActive(true);
            text.text = commands[languageIndex];
            LBIcon.sprite = hold_LB;
            playerScript.nearObject = true;
        }
    }
    void OnTriggerStay (Collider other)
    {
        if (other.tag == "Player" && !playerScript.pdaStarted && releaseTimer <= 0&& !playerScript.isHoldingObject)
        {
            text.text = commands[languageIndex];
            LBIcon.sprite = hold_LB;
            tutorial.text = commands[languageIndex + 4];
        }
        if (other.tag == "Player" && !playerScript.pdaStarted && releaseTimer > 0 && !playerScript.isHoldingObject)
        {
            text.text = "";
            LBIcon.sprite = hold_LB;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isHeld = false;
            playerScript.nearObject = false;
            text.text = "";
            tutorial.text = "";
            LBIcon.sprite = blank_Image;
        }
    }
}
