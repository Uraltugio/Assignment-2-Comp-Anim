using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSensor : MonoBehaviour
{
    private Steering_Seek steering;

    // Start is called before the first frame update
    void Start()
    {
        steering = GetComponentInParent<Steering_Seek>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            steering.SeekPowerup(other.transform);
        }
    }
}
