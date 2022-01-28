using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHolderController : MonoBehaviour
{

    private float lifetime = 1.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
