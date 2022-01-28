using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpitController : MonoBehaviour
{

    public float speed;
    private float lifetime = 1.25f;
    public GameObject player;
    private Rigidbody spitRb;

    public float spitGravityScale = 0.30f;
    public static float globalGravity = -9.81f;
    private float xSpitRotation;
    public float ySpitRotation;
    public bool isVFX;
    public GameObject vfx_explosion_prefab;
    



    void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());
        spitRb = gameObject.GetComponent<Rigidbody>();
        xSpitRotation = transform.rotation.x;

        if(isVFX)
        {
           
        }
        

    }

    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        Debug.Log(spitRb.velocity);
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }

        if(isVFX)
        {
            
        }


    }

    void FixedUpdate()
    {
        if (lifetime <= 0.65f)
        {
            ApplyGravity();

            xSpitRotation += 2.5f;

            Vector3 rotation = transform.localEulerAngles;

            transform.localRotation = Quaternion.Euler(xSpitRotation, rotation.y, rotation.z);

        }

    }

    void ApplyGravity()
    {
        Vector3 gravity = globalGravity * spitGravityScale * Vector3.up;
        spitRb.AddForce(gravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }

        else if (collision.gameObject.tag == "Ground")
        {

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NoShoot")
        {
            Destroy(gameObject);
        }

        else if (other.gameObject.tag == "RichFolk"|| other.gameObject.tag == "Guard")
        {
            CameraShakeBehavior.TriggerCameraShake(0.08f, 0.18f);
        }
    }

    void OnDestroy()
    {
        Instantiate(vfx_explosion_prefab, transform.position, Quaternion.identity);
    }
}
