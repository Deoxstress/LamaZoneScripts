using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatScript : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] voices;
    public float timer;
    public PlayerController player;
    public GameObject playerObject;
    public List <GameObject> folks;
    public int conversation;
    public int folkIndex;
    public List<int> conversationIndex = new List<int>() {0, 0, 8, 16, 16};
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        timer = Random.Range(0, 5);
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        playerObject = GameObject.Find("Main Camera");

        foreach (GameObject obj in folks)
        {
            folkIndex+=obj.GetComponent<RichFolkBehavior>().GuestType;
        }
        if (folkIndex < folks.Count)
        {
            if (folkIndex == 0)
            {
                conversation = 0;
            }
            else
            {
                conversation = 2;
            }
        }
        else
        {
            conversation = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.pdaStarted)
        {
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else if (timer <= 0.0f)
            {
                audioSource.PlayOneShot(voices[Random.Range(conversationIndex[conversation], conversationIndex[conversation+2])],0.8f);
                timer = 12f;
            }
        }
        else if (player.pdaStarted||ClockScript.partyOver)
        {
            audioSource.Stop();
        }
       
    }
}
