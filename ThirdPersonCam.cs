using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform Player;
    public Transform killSpit;
    public bool isInKillCam, endGame;
    

    //public float distanceOffset = 8.0f;
    public float distanceOffset;
    public float cameraXangle;
    float zoomSpeed = 2f;

    void Start()
    {
        isInKillCam = false;
    }


    void Update()
    {


    }

    void LateUpdate()
    {

        if (!isInKillCam)
        {
            CamControl();
        }


        else if (isInKillCam && killSpit!=null)
        {
            
            KillCamControl(killSpit);
        }
        
        if (endGame&& distanceOffset >=4)
        {
            distanceOffset -= 1/2f * Time.deltaTime;
        }
        
    }

    // Update is called once per frame
    void CamControl()
    {
        Vector3 dir = new Vector3(0, 0, -distanceOffset);
      
        //transform.LookAt(Target);

        Quaternion rotation = Quaternion.Euler(cameraXangle, 0, 0);

        transform.position = Player.position + rotation * dir;


        transform.LookAt(Player.position);

        //Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void KillCamControl(Transform Spit)
    {
        Vector3 dir = new Vector3(0, 0, -distanceOffset);

        Quaternion rotation = Quaternion.Euler(cameraXangle, killSpit.localEulerAngles.y, 0);

        transform.position = killSpit.position + rotation * dir;       

        transform.LookAt(killSpit.position);

    }
}
