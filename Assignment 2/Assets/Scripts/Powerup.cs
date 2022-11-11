using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Powerup : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Steering_Seek player)) //once player hits powerup trigger then it goes back to track
        {
            player.BackToTrack();

            player.maxSpeed += 5;
            
            Destroy(gameObject);
        }
    }
}
