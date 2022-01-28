using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardPunch : MonoBehaviour
{
    private GameObject guardParent;
    public AudioSource audioSource;
    public AudioClip[] punches;
    public bool hasPunched;
    // Start is called before the first frame update
    void Start()
    {
        guardParent = transform.parent.gameObject;
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    public void Sound()
    {
        if (!hasPunched)
        {
            audioSource.PlayOneShot(punches[Random.Range(0, 4)], 0.8f);
            hasPunched = true;
        }
    }

    public void Punch()
    {
        guardParent.GetComponent<GuardBehavior>().isPunching = true;
        hasPunched = false;
    }
    public void Retreat()
    {
        guardParent.GetComponent<GuardBehavior>().isPunching = false;
        hasPunched = false;
    }

}
