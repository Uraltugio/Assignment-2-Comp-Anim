using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private Racer parent;
    private Transform obstacle;

    float right;

    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponentInParent<Racer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!obstacle) return;
        Vector3 toObstacle = obstacle.position - transform.position;
        //float forward = Vector3.Dot(transform.forward, toObstacle.normalized);
        right = Vector3.Dot(transform.right, toObstacle.normalized);
        //Debug.Log(forward);
        //Debug.Log(right);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            obstacle = other.transform;
        }
        

        if (right > 0 && other.CompareTag("Obstacle")) //Right side & is an Obstacle
        {
            parent.SetInTrigger(true);
            parent.SetColPoint(other.transform.GetChild(1));
        }
        else if (right <= 0 && other.CompareTag("Obstacle")) //Left side & is an Obstacle
        {
            parent.SetInTrigger(true);
            parent.SetColPoint(other.transform.GetChild(0));
        }
    }
}
