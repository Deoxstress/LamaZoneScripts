using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class MouthController : MonoBehaviour
{

    public bool isSpitting;
    public bool isGoingToOneShot;

    public SpitController spit;
    public SpitController champagneSpit;
    public SpitController killSpit;
    public float spitSpeed;
    private GameObject player;
    private Vector3 playerLocalVelocity;
    public float delayBeforeSpits; // Initial Delay
    public float spitCounter, spitCounterOrigin; // Delay timer for spitting
    public float powerUpModifier;
    public float powerUpTimer;
    public PostProcessVolume postProcessParameters;
    public PostProcessProfile ppProfile;
    private bool scaleFontDown;
    private float timeToScaleDown;
    private bool fontUncapped;
    private int maxFontSize;
    public bool hasPickedChampagne;
    public int initFont;
    public Text multiplier; // variable qui stocke le Texte du multiplicateur, qu'on changera dans la coroutine
    public int multFontSize; // variable qui stocke la FontSize et qui s'incrémente à chaque coupe de champagne ramassée dans PlayerController
    public int baseMultiplier; // Stocke le multiplicateur de base (100%), à afficher à la fin du timer PowerUpTimer
    public float multiplierNumber; // Stocke le chiffre float du multiplicateur, se compare avec countNumber pour décrementer ou incrémenter l'affichage.
    public int countNumber; // variable pour la coroutine, et affichage du multiplicateur
    public float timeTolerp;
    public bool multGoingUp; // Si le multiplicateur augmente, true, si il descend false (quand PowerUpSustainTimer <= 0)
    public static bool isDead;
    private Coroutine scaleMult; // variable pour stopper la coroutine instanciée
    public float powerUpSustainTimer; // variable de soutien du powerUp(3f), instanciée lorsque le player passe sur une coupe cf PlayerController(L 317)
    private float powerUpDecayTimer; // Timer pour diminuer le powerUpModifier
    public int pickedUpChampagne; // Variable qui permet de déterminer le nombre de coupe prises, incrémentée dans PlayerController(L 318)
    public GameObject head;
    public Transform[] spitOrigin; // Empty Gameobject where the spit will go from;
    private int indexOfOrigin;
    public AudioSource audio;
    public AudioClip[] spitSound;
    public AudioClip killingSpit;
    AudioClip currentAudio;

    public GameObject cheeks;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isGoingToOneShot = true;
        postProcessParameters = GameObject.Find("Post process").GetComponent<PostProcessVolume>();
        ppProfile = postProcessParameters.profile;
        multiplier = GameObject.Find("Multiplier").GetComponent<Text>();
        multFontSize = 80;
        initFont = multFontSize;
        maxFontSize = 160;
        hasPickedChampagne = false;
        baseMultiplier = 0;
        countNumber = 1;
        multGoingUp = false;
        multiplier.gameObject.SetActive(false);
        cheeks.SetActive(false);
        isDead = false;
        scaleFontDown = false;
    }

    // Update is called once per frame
    void Update()
    {

        playerLocalVelocity = transform.InverseTransformDirection(player.GetComponent<Rigidbody>().velocity);

        if (playerLocalVelocity.z > 2)
        {
            spitSpeed = 13.5f;
        }

        else if (playerLocalVelocity.z > -1 && playerLocalVelocity.z < 1)
        {
            spitSpeed = 9f;
        }

        else if (playerLocalVelocity.z < -2)
        {
            spitSpeed = 4.5f;
        }

        if (!isSpitting && !isDead || isSpitting && !isDead)
        {
            spitCounter -= Time.deltaTime;

            if (spitCounter < 0 && isSpitting)
            {
                currentAudio = spitSound[Random.Range(0, spitSound.Length)];
                audio.pitch = Random.Range(0.8f, 1.0f);
                audio.PlayOneShot(currentAudio, 1f);
                if (isGoingToOneShot)
                {
                    indexOfOrigin = 0;
                }
                else
                {
                    indexOfOrigin = Random.Range(0, 3);
                }
                if (powerUpModifier > 6.0f)
                {
                    Invoke("CheeksInflation", 0.05f);
                }
                else
                {
                    Invoke("CheeksInflation", 0.1f);
                }
                cheeks.SetActive(true);
                head.transform.position = head.transform.position + head.transform.forward * 0.1f;

                if (isGoingToOneShot)
                {
                    int count = 0;
                    RaycastHit hit;                   
                    Ray ray = new Ray(player.transform.position, player.transform.forward);
                    Debug.DrawRay(player.transform.position, player.transform.forward, Color.red, 3.0f);                   
                    if (Physics.SphereCast(ray, 3.0f, out hit,15.0f))
                    {
                        if (hit.transform.gameObject.tag == "RichFolk" || hit.transform.gameObject.tag == "Guard")
                        {
                            audio.PlayOneShot(killingSpit);                            
                            player.GetComponent<PlayerController>().pdaStarted = true;                            

                        }

                        if(hit.transform.gameObject.tag == "Wall" )
                        {
                            SpitController newSpit = Instantiate(spit, spitOrigin[indexOfOrigin].position, spitOrigin[indexOfOrigin].rotation) as SpitController;
                            newSpit.speed = spitSpeed;
                            newSpit.GetComponent<SpitController>().ySpitRotation = newSpit.transform.rotation.y;
                            spitCounter = delayBeforeSpits / powerUpModifier;
                        }
                        else
                        {
                            SpitController newSpit = Instantiate(spit, spitOrigin[indexOfOrigin].position, spitOrigin[indexOfOrigin].rotation) as SpitController;
                            newSpit.speed = spitSpeed;
                            newSpit.GetComponent<SpitController>().ySpitRotation = newSpit.transform.rotation.y;
                            spitCounter = delayBeforeSpits / powerUpModifier;
                        }
                    }

                    if (!player.GetComponent<PlayerController>().pdaStarted)
                    {
                        isGoingToOneShot = true;
                    }

                }
                
                else if (powerUpTimer <= 0 && !isGoingToOneShot)
                {
                    SpitController newSpit = Instantiate(spit, spitOrigin[indexOfOrigin].position, spitOrigin[indexOfOrigin].rotation) as SpitController;
                    newSpit.speed = spitSpeed;
                    newSpit.GetComponent<SpitController>().ySpitRotation = newSpit.transform.rotation.y;
                    spitCounter = delayBeforeSpits / powerUpModifier;

                }
                else if (powerUpTimer > 0 && !isGoingToOneShot)
                {
                    SpitController newSpit = Instantiate(champagneSpit, spitOrigin[indexOfOrigin].position, spitOrigin[indexOfOrigin].rotation) as SpitController;
                    newSpit.speed = spitSpeed;
                    newSpit.GetComponent<SpitController>().ySpitRotation = newSpit.transform.rotation.y;
                    spitCounter = delayBeforeSpits / powerUpModifier;
                }

            }
        }
        multiplier.fontSize = multFontSize;
        if (player.GetComponent<PlayerController>().pdaStarted&&isGoingToOneShot)
        {
            isGoingToOneShot = false;
            player.GetComponent<PlayerController>().terrorZone.enabled = true;
            StartCoroutine(player.GetComponent<PlayerController>().KillCam());
            player.GetComponent<PlayerController>().canvasBar.SetActive(true);
            player.GetComponent<PlayerController>().canvasBar.transform.Find("LB_Icon").gameObject.SetActive(false);
            SpitController newSpit = Instantiate(killSpit, spitOrigin[0].position, spitOrigin[0].rotation) as SpitController;
            GameObject.Find("CameraHolder").GetComponent<ThirdPersonCam>().killSpit = newSpit.GetComponent<Transform>();
            newSpit.speed = spitSpeed;
            newSpit.GetComponent<SpitController>().ySpitRotation = newSpit.transform.rotation.y;
            spitCounter = delayBeforeSpits / powerUpModifier;
            multiplier.transform.parent.gameObject.SetActive(true);
            multiplier.gameObject.SetActive(true);
            multiplier.text = "";
        }

        if (powerUpSustainTimer >= 0)
        {
            powerUpSustainTimer -= Time.deltaTime;
        }

        if (powerUpModifier > 1f)
        {
            multiplierNumber = powerUpModifier * 35;
        }
        else if (powerUpModifier >= 1f && powerUpModifier <= 1.364f)
        {
            multiplierNumber = 0;
            multiplier.text = "";
        }

        Debug.Log(powerUpModifier);
        Debug.Log(multiplierNumber);


        if (powerUpTimer >= 0 && powerUpSustainTimer <= 0)
        {
            powerUpTimer -= Time.deltaTime;
            powerUpDecayTimer -= Time.deltaTime;
        }

        else if (powerUpTimer > 10f)
        {
            powerUpTimer = 10f;
            if (ppProfile.TryGetSettings<ChromaticAberration>(out ChromaticAberration aberr))
            {
                aberr.intensity.Override(0.4f);
            }
            //postProcessParameters.GetComponent<ChromaticAberration>().intensity.Override(1.0f);
        }

        if (powerUpSustainTimer <= 0 && multGoingUp)
        {
            multGoingUp = false;
            if (pickedUpChampagne < 5)
            {
                powerUpDecayTimer = 0.2f * pickedUpChampagne;
            }
            else if (pickedUpChampagne <= 10 && pickedUpChampagne > 5)
            {
                powerUpDecayTimer = 0.10f * pickedUpChampagne;
            }
            else if (pickedUpChampagne > 10 && pickedUpChampagne < 20)
            {
                powerUpDecayTimer = 0.05f * pickedUpChampagne;
            }
            else if (pickedUpChampagne >= 20)
            {
                powerUpDecayTimer = 0.03f * pickedUpChampagne;
            }

        }


        if (powerUpDecayTimer <= 0 && !multGoingUp && powerUpModifier >= 1.35f)
        {
            if (pickedUpChampagne <= 5)
            {
                powerUpDecayTimer = 0.2f * pickedUpChampagne;
            }
            else if (pickedUpChampagne <= 10 && pickedUpChampagne > 5)
            {
                powerUpDecayTimer = 0.10f * pickedUpChampagne;
            }
            else if (pickedUpChampagne > 10 && pickedUpChampagne < 20)
            {
                powerUpDecayTimer = 0.05f * pickedUpChampagne;
            }
            else if (pickedUpChampagne >= 20)
            {
                powerUpDecayTimer = 0.03f * pickedUpChampagne;
            }
            powerUpModifier -= 0.35f;
            pickedUpChampagne -= 1;
            multFontSize -= 2;
            if (powerUpModifier <= 4.65f)
            {
                if (ppProfile.TryGetSettings<ChromaticAberration>(out ChromaticAberration aberr))
                {
                    aberr.intensity.Override(aberr.intensity.value -= 0.04f);

                }
            }
        }

        else if (powerUpTimer <= 0)
        {
            powerUpModifier = 1f;
            multGoingUp = false;
            pickedUpChampagne = 0;
            multFontSize = 80;
            //postProcessParameters.GetComponent<ChromaticAberration>().intensity.Override(0.0f);
            //postProcessParameters.GetComponent<ChromaticAberration>().enabled.Override(false);
            //postProcessParameters.GetComponent<MotionBlur>().enabled.Override(false);

            if (ppProfile.TryGetSettings<ChromaticAberration>(out ChromaticAberration aberr))
            {
                aberr.intensity.Override(0.0f);
                aberr.enabled.Override(false);
            }
            if (ppProfile.TryGetSettings<MotionBlur>(out MotionBlur mblur))
            {
                mblur.enabled.Override(false);
            }


        }

        else if (powerUpModifier >= 4.65f)
        {
            if (ppProfile.TryGetSettings<ChromaticAberration>(out ChromaticAberration aberr))
            {
                aberr.intensity.Override(0.4f);
            }
        }

        if (multFontSize >= 140 && !fontUncapped)
        {
            multFontSize = 140;
        }

        if (countNumber <= 0)
        {
            StopCoroutine(scaleMult);

        }

        if (multiplierNumber > 30)
        {
            scaleMult = StartCoroutine(ScaleMultiplier());

        }

        if (multFontSize % 5 == 0 && hasPickedChampagne && !scaleFontDown && timeToScaleDown <= 0)
        {
            hasPickedChampagne = false;
            fontUncapped = true;
            initFont = multFontSize;
            multFontSize = maxFontSize;
            scaleFontDown = true;
            timeToScaleDown = 0.75f;
            Debug.Log(initFont);
            Debug.Log(multFontSize);
            Debug.Log(maxFontSize);
        }

        if(scaleFontDown && timeToScaleDown >= 0)
        {
            timeToScaleDown -= Time.deltaTime;
        }

        if(timeToScaleDown <= 0 && scaleFontDown)
        {
            scaleFontDown = false;           
            multFontSize = initFont;
            fontUncapped = false;

        }
    }

    void CheeksInflation()
    {
        cheeks.SetActive(false);
        head.transform.position = head.transform.position - head.transform.forward * 0.1f;
    }

    IEnumerator ScaleMultiplier()
    {

        while (countNumber < multiplierNumber)
        {
            countNumber++;
            Debug.Log("Up" + countNumber);
            Debug.Log("MultiplierNumber" + multiplierNumber);
            multiplier.text = "+ " + countNumber + "%";
            yield return new WaitForSeconds(0.01f);
        }

        while (countNumber > multiplierNumber)
        {
            countNumber--;
            if (!multGoingUp && powerUpModifier >= 1.001f)
            {
                powerUpModifier -= 0.001f;
            }
            Debug.Log("Down " + countNumber);
            multiplier.text = "+ " + countNumber + "%";
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

}