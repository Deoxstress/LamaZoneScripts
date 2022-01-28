using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour
{

    private Animator doorAnimator;
    public bool allowOpenClose;
    public bool open;
    public bool keyClosed, playAnim;
    private PlayerController playerScript;
    private float timer, delayOpeningDoor;
    public NavMeshObstacle navMesh;
    Canvas canvas;
    Text text;
    Image lockingBar, LBIcon;
    Sprite lb_button, blank_Image;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public MeshRenderer door;
    public Material material, lockMaterial;
    private int languageIndex;
    private List<string> commands = new List<string> {"Close", "Fermer", "Open", "Ouvrir", "Open/Lock", "Ouvrir/Fermer à clé" };
    public Text tutorial;

    void Start()
    {
        doorAnimator = gameObject.GetComponent<Animator>();
        open = true;
        allowOpenClose = false;
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        canvas = GameObject.Find("CanvasBouton").GetComponent<Canvas>();
        text = GameObject.Find("LB_Text").GetComponent<Text>();
        lockingBar = GameObject.Find("LockingBar").GetComponent<Image>();
        LBIcon = GameObject.Find("LB_Icon").GetComponent<Image>();
        if (PlayerPrefs.GetInt("Controls") == 0)
        {
            lb_button = Resources.Load<Sprite>("LB_NORMAL");
        }
        else if (PlayerPrefs.GetInt("Controls") == 1)
        {
            lb_button = Resources.Load<Sprite>("MOUSE_NORMAL");
        }
        blank_Image = Resources.Load<Sprite>("BLANK_IMAGE");
        navMesh = GetComponentInChildren<NavMeshObstacle>();
        navMesh.enabled = false;
        timer = 0;
        lockingBar.fillAmount = 0;
        material = door.material;
        tutorial = GameObject.Find("Tutorial").GetComponent<Text>();

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
        if(Input.GetButton("Interact")&&!playerScript.isHoldingObject && !playerScript.pdaStarted)
        {
            if (playerScript.keyNumber>0&& !keyClosed&& !open)
            {
                timer += Time.deltaTime;
                lockingBar.fillAmount = timer / 2;
                if (allowOpenClose&& timer >= 2)
                {
                    lockingBar.fillAmount = 0;
                    timer = 0;
                    keyClosed = true;
                    audioSource.pitch = Random.Range(0.8f, 1.0f);
                    audioSource.PlayOneShot(audioClips[0],1.5f);
                    playerScript.keyNumber--;
                    playerScript.keyNumber_Text.text = "x " + playerScript.keyNumber;
                    text.text = "";
                    tutorial.text = "";
                    LBIcon.sprite = blank_Image;
                    door.material = lockMaterial;
                }
            }           
        }
        if (Input.GetButtonUp("Interact")&&!playerScript.pdaStarted)
        {
            timer = 0;
            lockingBar.fillAmount = 0;
            if (!playerScript.isHoldingObject && allowOpenClose)
            {
                playAnim = true;
            }
        }
        if (open && !keyClosed&&playAnim)
        {
            doorAnimator.SetBool("ResetToIdle", false);
            doorAnimator.SetBool("TriggerOpen", false);
            doorAnimator.SetBool("TriggerClose", true);
            open = false;
            playAnim = false;
            audioSource.pitch = Random.Range(0.8f, 1.0f);
            audioSource.PlayOneShot(audioClips[1],1.5f);

        }
        else if (!open && !keyClosed&&playAnim)
        {
            doorAnimator.SetBool("TriggerOpen", true);
            doorAnimator.SetBool("ResetToIdle", true);
            open = true;
            playAnim = false;
            audioSource.pitch = Random.Range(0.8f, 1.0f);
            audioSource.PlayOneShot(audioClips[2], 1.5f);
        }

        if (doorAnimator.GetBool("ResetToIdle"))
        {
            doorAnimator.SetBool("TriggerClose", false);          
        }

        if (keyClosed)
        {
            door.material = lockMaterial;
        }
        else
        {
            door.material = material;
        }
        
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player"&& !playerScript.nearObject&&!keyClosed&&!other.isTrigger && !playerScript.pdaStarted)
        {
            allowOpenClose = true;
            LBIcon.sprite = lb_button;
            if (open)
            {
                text.text = commands[languageIndex];
            }
            else if (!open)
            {
                if (playerScript.keyNumber > 0&&!playerScript.pdaStarted)
                {
                    text.text = commands[languageIndex+4];
                }
                else
                {
                    text.text = commands[languageIndex+2];
                }
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            allowOpenClose = false;
            timer = 0;
            lockingBar.fillAmount = 0;
            if (!playerScript.isHoldingObject&& !playerScript.pdaStarted && !playerScript.nearObject)
            {
                text.text = "";
                LBIcon.sprite = blank_Image;
            }
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.isTrigger && !playerScript.pdaStarted)
        {
            timer = 0;
            lockingBar.fillAmount = 0;
        }
        if (collision.gameObject.tag == "Guard" && !keyClosed && !open)
        {
            playAnim = true;

        }
        else if (collision.gameObject.tag == "Guard" && keyClosed)
        {
            navMesh.enabled = true;

        }
        /*if (collision.gameObject.tag == "RichFolk" && !keyClosed && !open && playerScript.pdaStarted)
        {
            playAnim = true;
        }*/
    }

}
