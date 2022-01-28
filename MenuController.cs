using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
public class MenuController : MonoBehaviour
{
    public GameObject retryButton;
    public GameObject menuButton;
    public GameObject quitButton;
    public GameObject resumeButton;
    public PostProcessVolume postProcessParameters;
    public PostProcessProfile ppProfile;
    public GameObject canvasToHide;
    public PlayerController player;
    public static bool isPaused;
    public int cursorIndex;
    public Button[] selectButton;
    public Text[] selectButtonText;
    public bool stickDownLast;
    private int languageIndex;
    public AudioSource audioSource;
    public AudioClip[] menuSounds;
    private List<string> commands = new List<string> { "Resume", "Reprendre", "Retry", "Recommencer","Menu","Menu","Quit","Quitter" };

    void Start()
    {
        cursorIndex = 0;
        selectButton[0].GetComponent<Image>().color = new Color32(220, 220, 220, 255);
        selectButton[0].Select();
        retryButton.SetActive(false);
        menuButton.SetActive(false);
        quitButton.SetActive(false);
        resumeButton.SetActive(false);
        isPaused = false;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        audioSource = GameObject.Find("Player").GetComponentInChildren<AudioSource>();
        postProcessParameters = GameObject.Find("Post process").GetComponent<PostProcessVolume>();
        ppProfile = postProcessParameters.profile;
        if (ppProfile.TryGetSettings<DepthOfField>(out DepthOfField dof))
        {
            dof.enabled.Override(false);
        }
        if (PlayerPrefs.GetInt("Language") == 0)
        {
            languageIndex = 0;
        }
        else
        {
            languageIndex = 1;
        }
        selectButtonText[0].text = commands[languageIndex];
        selectButtonText[1].text = commands[languageIndex+2];
        selectButtonText[2].text = commands[languageIndex+4];
        selectButtonText[3].text = commands[languageIndex+6];
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if (!isPaused)
            {
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(menuSounds[1], 0.6f);
                isPaused = true;
                retryButton.SetActive(true);
                menuButton.SetActive(true);
                quitButton.SetActive(true);
                resumeButton.SetActive(true);
                Time.timeScale = 0;
                cursorIndex = 0;
                selectButton[0].GetComponent<Image>().color = new Color32(220, 220, 220, 255);
                selectButton[0].Select();
                if (!player.pdaStarted)
                {
                    canvasToHide.SetActive(false);
                }
                if (ppProfile.TryGetSettings<DepthOfField>(out DepthOfField dof))
                {
                    dof.enabled.Override(true);
                }
                player.camera_audioSource.Pause();
            }
            else
            {
                Resume();
            }
        }
        if (Input.GetAxisRaw("Vertical") < 0&&isPaused)
        {
            if (!stickDownLast)
            {
                selectButton[cursorIndex].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                if (cursorIndex < 3)
                {
                    cursorIndex += 1;
                }
                else
                {
                    cursorIndex = 0;
                }
                stickDownLast = true;
                audioSource.PlayOneShot(menuSounds[0], 0.8f);
                selectButton[cursorIndex].GetComponent<Image>().color = new Color32(200, 200, 200, 255);
            }

        }
        else if (Input.GetAxisRaw("Vertical") > 0&&isPaused)
        {
            if (!stickDownLast)
            {
                selectButton[cursorIndex].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                if (cursorIndex > 0)
                {
                    cursorIndex -= 1;
                }
                else
                {
                    cursorIndex = 3;
                }
                stickDownLast = true;
                audioSource.PlayOneShot(menuSounds[0], 0.8f);
                selectButton[cursorIndex].GetComponent<Image>().color = new Color32(200, 200, 200, 255);
            }
        }
        else
        {
            stickDownLast = false;
        }
        selectButton[cursorIndex].Select();
    }

    public void Retry()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void ToMenu()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
        Time.timeScale = 1;
        isPaused = false;
        selectButton[cursorIndex].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        cursorIndex = 0;
        selectButton[0].GetComponent<Image>().color = new Color32(220, 220, 220, 255);
        selectButton[0].Select();
        if (ppProfile.TryGetSettings<DepthOfField>(out DepthOfField dof))
        {
            dof.enabled.Override(false);
        }
        if (!player.pdaStarted)
        {
            canvasToHide.SetActive(true);
        }
        retryButton.SetActive(false);
        menuButton.SetActive(false);
        quitButton.SetActive(false);
        resumeButton.SetActive(false);
        player.camera_audioSource.Play();
    }

    public void QuitGame()
    {
        audioSource.PlayOneShot(menuSounds[1], 0.6f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
