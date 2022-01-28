using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneManagement : MonoBehaviour
{
    public GameObject canvas, player, gate,fadeScreen, camera, destination, flag1, flag2;
    public Animator player_anim, gate_anim, fade_anim;
    private Vector2 move;
    public bool playOrQuit, stickDownLast,stickSideLast,inputFromJoystick, options, launchingGame, cameraMoving;
    public Text text1, text2, text_instructions;
    public AudioSource audioSource;
    public AudioClip[] menuSounds;
    private float audioVolume;
    public int cursorIndexVertical = 1;
    public int cursorIndexHorizontal;
    public Button[] languageButtons;
    public Button[] controlButtons;
    List<string> frenchButton = new List<string> { "Commencer la partie","Quitter", "Controles Clavier/Souris", "Controles Manette" };
    List<string> englishButton = new List<string> { "Start Game","Quit", "Mouse/Keyboard Controls", "Controller Controls" };
    List<string> quote = new List<string> { "Vengeance is a dish best served cold", "La vengeance est un plat qui se mange froid", };
    Vector3 yVelocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("Controls", 0);
        if (PlayerPrefs.HasKey("Language"))
        {

        }
        else
        {
            PlayerPrefs.SetInt("Language", 0);
        }
        audioVolume = 1f;
        cursorIndexHorizontal = 1;
        fadeScreen.SetActive(false);
        if (PlayerPrefs.GetInt("Language") == 0)
        {
            text2.text = englishButton[1];
            text1.text = englishButton[0];
        }
        else
        {
            text2.text = frenchButton[1];
            text1.text = frenchButton[0];
        }
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        if (!options)
        {
            Application.Quit();
        }
        else
        {
            PlayerPrefs.SetInt("Controls", 0);
            StartCoroutine(StartGame());
        }
    }
    public void Options()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        if (!options)
        {
            if (PlayerPrefs.GetInt("Language") == 0)
            {
                text2.text = englishButton[3];
                text1.text = englishButton[2];
            }
            else
            {
                text2.text = frenchButton[3];
                text1.text = frenchButton[2];
            }
            options = true;
        }
        else
        {
            PlayerPrefs.SetInt("Controls", 1);
            StartCoroutine(StartGame());          
        }
    }
    public void French()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        PlayerPrefs.SetInt("Language", 1);
        if (options)
        {
            text2.text = frenchButton[3];
            text1.text = frenchButton[2];
        }
        else
        {
            text2.text = frenchButton[1];
            text1.text = frenchButton[0];
        }
    }
    public void English()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        PlayerPrefs.SetInt("Language", 0);
        if (options)
        {
            text2.text = englishButton[3];
            text1.text = englishButton[2];
        }
        else
        {
            text2.text = englishButton[1];
            text1.text = englishButton[0];
        }
    }
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            if (!stickDownLast)
            {
                stickDownLast = true;
                audioSource.PlayOneShot(menuSounds[0], 0.8f);
                if (cursorIndexVertical < 1)
                {
                    cursorIndexVertical = 1;
                }
                else
                {
                    cursorIndexVertical = 0;
                }
            }
            if (!inputFromJoystick)
            {
                inputFromJoystick=true;
            }

        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (!stickDownLast)
            {
                stickDownLast = true;
                audioSource.PlayOneShot(menuSounds[0], 0.8f);
                if (cursorIndexVertical > 0)
                {
                    cursorIndexVertical = 0;
                }
                else
                {
                    cursorIndexVertical = 1;
                }
            }
            if (!inputFromJoystick)
            {
                inputFromJoystick=true;
            }
        }
        else
        {
            stickDownLast = false;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (!stickSideLast)
            {
                stickSideLast = true;
                audioSource.PlayOneShot(menuSounds[0], 0.8f);
                if (cursorIndexHorizontal < 2)
                {
                    cursorIndexHorizontal++;
                }
                else
                {
                    cursorIndexHorizontal = 0;
                }
            }
            if (!inputFromJoystick)
            {
                inputFromJoystick=true;
            }

        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            if (!stickSideLast)
            {
                stickSideLast = true;
                audioSource.PlayOneShot(menuSounds[0], 0.8f);
                if (cursorIndexHorizontal > 0)
                {
                    cursorIndexHorizontal--;
                }
                else
                {
                    cursorIndexHorizontal = 2;
                }
            }
            if (!inputFromJoystick)
            {
                inputFromJoystick=true;
            }
        }
        else
        {
            stickSideLast = false;
        }
        if (inputFromJoystick)
        {
            if (cursorIndexHorizontal == 1)
            {
                if (cursorIndexVertical == 0)
                {
                    controlButtons[0].Select();
                }
                else
                {
                    controlButtons[1].Select();
                }
            }

            if (cursorIndexHorizontal == 0)
            {
                languageButtons[0].Select();
            }
            else if (cursorIndexHorizontal == 2)
            {
                languageButtons[1].Select();
            }
        }
        if (cameraMoving)
        {           
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, destination.transform.position, ref yVelocity, 0.8f);
            camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, destination.transform.rotation, 10 * Time.deltaTime);
            audioSource.volume -= 0.2f * Time.deltaTime;
        }
        if (launchingGame)
        {
            player.transform.position += transform.forward * Time.deltaTime * 3;
            camera.transform.position += transform.forward * Time.deltaTime * 5;
            audioSource.volume -= 0.2f*Time.deltaTime;
            text_instructions.text = quote[PlayerPrefs.GetInt("Language")];
            text_instructions.color = Color.Lerp(text_instructions.color, new Color(1f, 1f, 1f, 1f), 0.35f * Time.deltaTime);
        }
    }

    IEnumerator StartGame()
    {
        cameraMoving = true;
        fadeScreen.SetActive(true);
        text1.text = "";
        text2.text = "";
        flag1.SetActive(false);
        flag2.SetActive(false);
        yield return new WaitForSeconds(6.0f);
        cameraMoving = false;
        launchingGame = true;
        text1.text = "";
        text2.text = "";
        player_anim.SetBool("Walk_Forward", true);
        gate_anim.SetBool("OpeningDoor", true);
        fade_anim.SetBool("Fading", true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("BlockingLD");
    }

}
