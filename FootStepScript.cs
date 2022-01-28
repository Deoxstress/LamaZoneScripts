using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepScript : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] clips;
    private AudioClip currentClip;
    public float volume;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Step()
    {
        AudioClip currentClip = clips[Random.Range(0, clips.Length)];
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(currentClip,volume);
    }
}
