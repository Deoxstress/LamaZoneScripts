using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClockScript : MonoBehaviour
{
    public Transform clockHand;
    public Image redCircle;
    public static bool partyOver;
    private PlayerController playerScript;
    // Start is called before the first frame update
    void Start()
    {
        //clockHand = transform.Find("Aiguille");
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!partyOver)
        {
            clockHand.eulerAngles += new Vector3(0, 0, -Time.deltaTime);
            redCircle.fillAmount += Time.deltaTime / 360;
        }
        if (1>clockHand.eulerAngles.z&& 0<clockHand.eulerAngles.z)
        {
            partyOver = true;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (playerScript.pdaStarted)
        {
            Destroy(gameObject);
        }
    }
}
