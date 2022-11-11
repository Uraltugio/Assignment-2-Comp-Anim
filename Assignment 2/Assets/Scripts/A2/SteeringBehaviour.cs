using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum state { None, Seek, Flee, ProximityFlee, Arrival, Pursuit, Evade}

public class SteeringBehaviour : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform movingTarget;
    private Rigidbody _movingTargetRb;

    [Header("Values")]
    [SerializeField] private float slowRadius;
    [SerializeField] private float fleeRadius;
    [SerializeField] private float maxSpeed;
    [SerializeField] private state currentState;

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _movingTargetRb = movingTarget.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);

        switch (currentState)
        {
            case state.None:
                //Nothing
                break;

            case state.Seek:
                Seeking();
                break;

            case state.Flee:
                Flee();
                break;

            case state.ProximityFlee:
                ProximityFlee();
                break;

            case state.Arrival:
                Arrival();
                break;

            case state.Pursuit:
                Pursuit();
                break;

            case state.Evade:
                Evade();
                break;
        }
    }

    private void Seeking()
    {
        Vector3 targetDir = (target.position - transform.position).normalized;
        Vector3 targetVelocity = targetDir * maxSpeed;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = targetVelocity - currentVelocity;
        _rb.AddForce(steeringVector);
    }

    private void Flee()
    {
        Vector3 targetDir = (target.position - transform.position).normalized;
        Vector3 targetVelocity = targetDir * maxSpeed;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = currentVelocity - targetVelocity;
        _rb.AddForce(steeringVector);
    }

    private void ProximityFlee()
    {
        float distance = (target.position - transform.position).magnitude;

        Debug.Log($"{distance} < {fleeRadius} or {_rb.velocity.magnitude}");
        if (distance < fleeRadius)
        {
            _rb.drag = 0;
            Flee();
        }
        else
        {
            _rb.drag = 3;
        }

    }

    private void Arrival()
    {
        Vector3 targetVelocity;

        float distance = (target.position - transform.position).magnitude;

        if (distance < slowRadius)
        {
            Vector3 targetDir = (target.position - transform.position).normalized;

            targetVelocity = targetDir * (maxSpeed * (distance / slowRadius));
        }
        else
        {
            Vector3 targetDir = (target.position - transform.position).normalized;

            targetVelocity = targetDir * maxSpeed;
        }
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = targetVelocity - currentVelocity;

        _rb.AddForce(steeringVector);
    }

    private void Pursuit()
    {
        // calculating future position
        Vector3 futurePos = movingTarget.position + _movingTargetRb.velocity;
        // replace targets current position with future position
        Vector3 targetDir = (futurePos - transform.position).normalized;
        Vector3 targetVelocity = targetDir * maxSpeed;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = targetVelocity - currentVelocity;
        _rb.AddForce(steeringVector);
    }

    private void Evade()
    {
        // calculating future position
        Vector3 futurePos = movingTarget.position + _movingTargetRb.velocity;
        // replace targets current position with its future position
        Vector3 targetDir = (futurePos - transform.position).normalized;
        Vector3 targetVelocity = targetDir * maxSpeed;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = currentVelocity - targetVelocity;
        _rb.AddForce(steeringVector);
    }
}
