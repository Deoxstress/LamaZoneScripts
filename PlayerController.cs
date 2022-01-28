using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{

    public int playerHp;
    public float invulnerabilityTime;
    public float speed;
    private Rigidbody rb;
    public bool isMoving, isInvulnerable;
    private Vector3 playerMovement;
    private Vector3 playerVelocity;
    private float currentYRotation;
    private Camera mainCam;
    private GameObject cameraHolder;
    public SphereCollider terrorZone;

    public MouthController mouthSpitter;
    private float elapsedSpitTime;

    public bool useController;
    public bool isHoldingObject;
    public bool pdaStarted, isHit, nearObject;
    public int keyNumber, finalScore;
    public Text keyNumber_Text, spitInputText, tutorialText;
    public static Text hitCounter;
    public int hitCounterNumber, specialhitCounterNumber, guardHit;
    //public GameObject[] guestUI, specialGuestUI;
    public List<GameObject> guestUI, specialGuestUI, guardsCollider;
    public GameObject guestUIparent, specialGuestUIparent;
    public Sprite guestHit, specialGuestHit;
    int guestEscaped, specialGuestEscaped;
    Color tempA, tempB;
    Vector3 localVelocity;
    Animator anim;
    public AudioSource audioSource;
    public AudioSource camera_audioSource;
    public AudioClip clip;
    public AudioClip[] musics;
    public AudioClip[] endSounds;
    public bool isDead, isDying, deathByGuard;
    public GameObject canvasPolice, canvasScore, canvasBar, policeAnimation;
    private int guestScore, specialGuestScore, guardScore;
    public Text partyOverText, guestScoreText, specialGuestScoreText, guardScoreText, rank, clue;
    public Renderer lama_renderer;
    List<string> rankNumber = new List<string> { "F", "E", "D", "C", "B", "A", "S" };
    private List<string> rankLanguage = new List<string> {"Party Over !","Fin de la Partie !","Guest Score : ","Invités Abattus : ","Special Guest Score : ", "Invités Spéciaux Abattus : ","Guard Score : ","Gardes Abattus : ","Rank : ","Rang : "};
    public List<string> rankClue;
    public GameObject arm,arm2, head;
    private int languageIndex;
    public Image rb_Icon;
    public GameObject deathAnim;
    Coroutine blinkCoroutine = null;
    void Start()
    {
        MenuController.isPaused = false;
        canvasPolice.SetActive(false);
        canvasScore.SetActive(false);
        camera_audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        mainCam = FindObjectOfType<Camera>();
        cameraHolder = GameObject.Find("CameraHolder");
        terrorZone = gameObject.GetComponent<SphereCollider>();
        terrorZone.enabled = false;
        playerHp = 3;
        invulnerabilityTime = 0.0f;
        keyNumber_Text = GameObject.Find("Key_Number").GetComponent<Text>();
        /*guestUI = GameObject.FindGameObjectsWithTag("InviteUI");
        specialGuestUI = GameObject.FindGameObjectsWithTag("SpecialInviteUI");*/
        foreach (Transform child in guestUIparent.transform)
        {
            GameObject obj = child.gameObject;
            guestUI.Add(obj);
        }
        foreach (Transform child in specialGuestUIparent.transform)
        {
            GameObject obj = child.gameObject;
            specialGuestUI.Add(obj);
        }

        //hitCounter.text = "Salivés : 0";

        /*if (Input.GetJoystickNames().Length > 0)
        {
            useController = true;
        }
        else
        {
            useController = false;
        }*/

        if (PlayerPrefs.GetInt("Controls") == 0)
        {
            useController = true;
            rb_Icon.sprite = Resources.Load<Sprite>("RB_BUTTON_HOLD");
        }
        else if (PlayerPrefs.GetInt("Controls") == 1)
        {
            useController = false;
            rb_Icon.sprite = Resources.Load<Sprite>("HOLD_LEFT_MOUSE");
        }

        tempA = guestUI[guestUI.Count - guestEscaped - 1].GetComponent<Image>().color;
        tempA.a = 0.3f;
        tempB = specialGuestUI[specialGuestUI.Count - specialGuestEscaped - 1].GetComponent<Image>().color;
        tempB.a = 0.3f;

        elapsedSpitTime = 0.0f;

        camera_audioSource.loop = true;
        camera_audioSource.clip = musics[0];
        camera_audioSource.Play();

        if (PlayerPrefs.GetInt("Language") == 0)
        {
            languageIndex = 0;
            spitInputText.text = "Spit";
            tutorialText.text = "Organize the mansion and spit on as much guests as you possibly can";
        }
        else
        {
            languageIndex = 1;
            spitInputText.text = "Cracher";
            tutorialText.text = "organisez le manoir et crachez sur autant de personnes que possible";
        }
        policeAnimation.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        currentYRotation = transform.rotation.y;
        if (!isDead)
        {
            MovePlayer();
            if (!useController)
            {
                SpitToDirection();
            }
            if (useController)
            {
                Vector3 playerDirection = Vector3.right * Input.GetAxisRaw("RHorizontal") + Vector3.forward * -Input.GetAxisRaw("RVertical");
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3.right* Input.GetAxisRaw("RHorizontal") + Vector3.forward * -Input.GetAxisRaw("RVertical")) - transform.position), Time.deltaTime * 10);

                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    //transform.rotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirection, Vector3.up), Time.deltaTime * 10);
                }
            }

        }

        if (hitCounterNumber + specialhitCounterNumber + guestEscaped + specialGuestEscaped == 115 && !isDying)
        {
            StartCoroutine(End());
            isDying = true;
        }

        if (playerMovement == Vector3.zero)
        {
            isMoving = false;
            anim.SetBool("Idle", true);
            anim.SetBool("Walk_Backwards", false);
            anim.SetBool("Walk_Forward", false);
            anim.SetBool("Left_Strafe", false);
            anim.SetBool("Right_Strafe", false);
        }

        else if (playerMovement != Vector3.zero)
        {
            isMoving = true;
            anim.SetBool("Idle", false);
        }

        if (Input.GetButtonDown("Spit") && !MenuController.isPaused)
        {            
            mouthSpitter.isSpitting = true;

        }

        else if (Input.GetButton("Spit"))
        {
            elapsedSpitTime += Time.deltaTime;

            if (elapsedSpitTime >= 0.6f && elapsedSpitTime < 0.9f)
            {
                mouthSpitter.delayBeforeSpits = 0.55f;

            }

            /*else if (elapsedSpitTime >= 0.9f && elapsedSpitTime < 2.5f)
            {
                mouthSpitter.delayBeforeSpits = 0.40f;
            }

            else if (elapsedSpitTime >= 2.5f && elapsedSpitTime < 4f)
            {
                mouthSpitter.delayBeforeSpits = 0.25f;
            }

            else if (elapsedSpitTime >= 4f)
            {
                mouthSpitter.delayBeforeSpits = 0.125f;
            }
            */

        }

        else if (Input.GetButtonUp("Spit"))
        {
            mouthSpitter.delayBeforeSpits = 0.70f;
            //terrorZone.enabled = false;
            elapsedSpitTime = 0.0f;
            mouthSpitter.isSpitting = false;
        }

        if (isHoldingObject)
        {
            anim.SetBool("Carrying", true);
        }
        else
        {
            anim.SetBool("Carrying", false);
        }
        if (invulnerabilityTime > 0.0f)
        {
            invulnerabilityTime -= Time.deltaTime;
            if (invulnerabilityTime <= 3.0f)
            {
                speed = 0.0f;
                if (!isHit)
                {
                    anim.SetTrigger("Hit");
                    isHit = true;
                }
            }
            if (invulnerabilityTime <= 2.0f)
            {
                speed = 5.0f;
                if (!isInvulnerable)
                {
                    isInvulnerable = true;                   
                    blinkCoroutine = StartCoroutine(Blink());
                }
            }
        }
        else
        {           
            if (isInvulnerable)
            {
                foreach (GameObject collision in guardsCollider)
                {
                    Physics.IgnoreCollision(collision.GetComponent<Collider>(), GetComponent<Collider>(), false);
                }
                StopCoroutine(blinkCoroutine);
                isInvulnerable = false;
                lama_renderer.enabled = true;
            }
        } 

        if (playerHp <= 0 && !isDying)
        {
            deathByGuard = true;
            StartCoroutine(End());
            isDying = true;
        }

    }
    IEnumerator Blink()
    {       
        if (isInvulnerable)
        {
            while (true)
            {
                lama_renderer.enabled = false;
                yield return new WaitForSeconds(0.2f);
                lama_renderer.enabled = true;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }


    public IEnumerator KillCam()
    {
        camera_audioSource.Stop();
        cameraHolder.GetComponent<ThirdPersonCam>().distanceOffset = 5;
        cameraHolder.GetComponent<ThirdPersonCam>().cameraXangle = 25;
        cameraHolder.GetComponent<ThirdPersonCam>().isInKillCam = true;
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(1);
        cameraHolder.GetComponent<ThirdPersonCam>().distanceOffset = 10;
        cameraHolder.GetComponent<ThirdPersonCam>().cameraXangle = 50;
        cameraHolder.GetComponent<ThirdPersonCam>().isInKillCam = false;
        Time.timeScale = 1f;
        camera_audioSource.loop = true;
        camera_audioSource.clip = musics[1];
        camera_audioSource.Play();
    }

    void FixedUpdate()
    {
        rb.velocity = playerVelocity;
        localVelocity = transform.InverseTransformDirection(rb.velocity);
        AnimationController();
        //transform.Translate(playerMovement * speed * Time.deltaTime, Space.World);
        //transform.Translate(playerMovement * speed * Time.deltaTime, Space.Self);
        if (Input.GetKeyDown("b"))
        {
            Debug.Log(localVelocity.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Key")
        {
            Destroy(collision.gameObject);
            keyNumber++;
            keyNumber_Text.text = "x " + keyNumber;           
            audioSource.PlayOneShot(endSounds[4],2.0f);
            if (languageIndex==0)
            {
                tutorialText.text = "Keys can lock doors, and make guests search for another exit";
            }
            else
            {
                tutorialText.text = "Les clefs peuvent fermer les portes et rediriger les gens vers une autre sortie";
            }

        }

        else if (collision.gameObject.tag == "Champagne")
        {
            mouthSpitter.powerUpSustainTimer = 4.0f;
            mouthSpitter.pickedUpChampagne += 1;
            mouthSpitter.powerUpModifier += 0.365f;
            mouthSpitter.multFontSize += 2;
            mouthSpitter.multGoingUp = true;
            mouthSpitter.hasPickedChampagne = true;
            camera_audioSource.PlayOneShot(endSounds[5],0.8f);
            if (mouthSpitter.ppProfile.TryGetSettings<ChromaticAberration>(out ChromaticAberration aberr))
            {
                Debug.Log(aberr);
                aberr.enabled.Override(true);
                FloatParameter intensityOfAberr = aberr.intensity;
                aberr.intensity.Override(0.04f + intensityOfAberr);
            }
            else if (mouthSpitter.ppProfile.TryGetSettings<MotionBlur>(out MotionBlur mblur))
            {
                mblur.enabled.Override(true);
            }

            mouthSpitter.powerUpTimer += 2.5f;

            if (mouthSpitter.powerUpTimer > 10f)
            {
                mouthSpitter.powerUpTimer = 10f;
            }
            
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Guard" && invulnerabilityTime > 0.0f)
        {
            guardsCollider.Add(collision.gameObject);
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(),true);
        }
    }


    void MovePlayer()
    {
        playerMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        playerVelocity = playerMovement * speed;
    }
    void AnimationController()
    {
        if (localVelocity.x < -2)
        {
            anim.SetBool("Left_Strafe", true);
            anim.SetBool("Right_Strafe", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Walk_Forward", false);
            anim.SetBool("Walk_Backwards", false);
        }
        else if (localVelocity.x > 2)
        {
            anim.SetBool("Right_Strafe", true);
            anim.SetBool("Left_Strafe", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Walk_Forward", false);
            anim.SetBool("Walk_Backwards", false);
        }
        if (localVelocity.z > 2)
        {
            anim.SetBool("Walk_Forward", true);
            anim.SetBool("Walk_Backwards", false);
            anim.SetBool("Idle", false);
            if (localVelocity.x < -2)
            {
                anim.SetBool("Left_Strafe", true);
                anim.SetBool("Right_Strafe", false);
                anim.SetBool("Walk_Forward", false);
            }
            else if (localVelocity.x > 2)
            {
                anim.SetBool("Right_Strafe", true);
                anim.SetBool("Left_Strafe", false);
                anim.SetBool("Walk_Forward", false);
            }
            else
            {
                anim.SetBool("Walk_Forward", true);
                anim.SetBool("Right_Strafe", false);
                anim.SetBool("Left_Strafe", false);
            }
        }
        else if (localVelocity.z < -2)
        {
            anim.SetBool("Walk_Forward", false);
            anim.SetBool("Idle", false);
            if (localVelocity.x < -2)
            {
                anim.SetBool("Left_Strafe", true);
                anim.SetBool("Right_Strafe", false);
                anim.SetBool("Walk_Backwards", false);
            }
            else if (localVelocity.x > 2)
            {
                anim.SetBool("Right_Strafe", true);
                anim.SetBool("Left_Strafe", false);
                anim.SetBool("Walk_Backwards", false);
            }
            else
            {
                anim.SetBool("Walk_Backwards", true);
                anim.SetBool("Right_Strafe", false);
                anim.SetBool("Left_Strafe", false);
            }
        }
    }

    void SpitToDirection()
    {

        Ray cameraRay = mainCam.ScreenPointToRay(Input.mousePosition);

        Plane hypotheticalPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (hypotheticalPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLookAt = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLookAt, Color.blue);

            //transform.LookAt(new Vector3(pointToLookAt.x, transform.position.y, pointToLookAt.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(pointToLookAt.x, transform.position.y, pointToLookAt.z) - transform.position), Time.deltaTime * 10);
        }
    }
    public void GuestHit()
    {
        guestUI[hitCounterNumber].GetComponent<Image>().sprite = guestHit;
        hitCounterNumber++;
    }
    public void SpecialGuestHit()
    {
        specialGuestUI[specialhitCounterNumber].GetComponent<Image>().sprite = specialGuestHit;
        specialhitCounterNumber++;
    }
    public void GuestEscape()
    {
        guestEscaped++;
        guestUI[guestUI.Count - guestEscaped].GetComponent<Image>().color = tempA;
    }
    public void SpecialGuestEscape()
    {
        specialGuestEscaped++;
        specialGuestUI[specialGuestUI.Count - specialGuestEscaped].GetComponent<Image>().color = tempB;
    }

    IEnumerator End()
    {
        MouthController.isDead = true;
        camera_audioSource.Stop();
        cameraHolder.GetComponent<ThirdPersonCam>().endGame = true;
        if (deathByGuard || ClockScript.partyOver)
        {
            isDead = true;
            playerVelocity = new Vector3(0, 0, 0);
            if (!ClockScript.partyOver)
            {
                anim.SetBool("Death", true);
            }
            yield return new WaitForSeconds(5.0f);
        }
        else
        {
            policeAnimation.SetActive(true);
            camera_audioSource.pitch = 1.0f;
            camera_audioSource.clip = endSounds[0];
            camera_audioSource.Play();
            canvasPolice.SetActive(true);
            yield return new WaitForSeconds(5.0f);
            camera_audioSource.PlayOneShot(endSounds[1]);
            yield return new WaitForSeconds(1.5f);
            camera_audioSource.PlayOneShot(endSounds[2], 6.0f);
            yield return new WaitForSeconds(4.0f);
            anim.SetBool("Idle", true);
            yield return new WaitForSeconds(2.0f);
            playerVelocity = new Vector3(0, 0, 0);
            isDead = true;
            anim.SetBool("Idle", true);           
            camera_audioSource.PlayOneShot(endSounds[3], 0.5f);
            Instantiate(deathAnim, transform.position, transform.rotation);
            anim.SetBool("Death", true);
        }
        canvasScore.SetActive(true);
        partyOverText.text = rankLanguage[languageIndex];
        guestScoreText.text = rankLanguage[languageIndex+2] + guestScore.ToString();
        while (guestScore < hitCounterNumber)
        {
            guestScore++;
            guestScoreText.text = rankLanguage[languageIndex+2] + guestScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        specialGuestScoreText.text = rankLanguage[languageIndex + 4] + specialGuestScore.ToString();
        while (specialGuestScore < specialhitCounterNumber)
        {
            specialGuestScore++;
            specialGuestScoreText.text = rankLanguage[languageIndex + 4] + specialGuestScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        guardScoreText.text = rankLanguage[languageIndex + 6] + guardScore.ToString();
        while (guardScore < guardHit)
        {
            guardScore++;
            guardScoreText.text = rankLanguage[languageIndex + 6] + guardScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        finalScore = Mathf.RoundToInt((guestScore + specialGuestScore * 1.5f + guardScore * 0.5f) / 10);
        if (finalScore > 6)
        {
            finalScore = 6;
        }
        rank.text = rankLanguage[languageIndex + 8] + rankNumber[finalScore];
        yield return new WaitForSeconds(1.5f);
        clue.text = rankClue[finalScore*2+languageIndex];
        yield return new WaitForSeconds(3.0f);
        canvasScore.GetComponentInChildren<Animator>().SetBool("Fading", true);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("MenuTest2");

    }
}


