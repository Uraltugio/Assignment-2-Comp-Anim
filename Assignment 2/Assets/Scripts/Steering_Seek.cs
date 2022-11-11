using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Steering_Seek : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform movingTarget;
    private Rigidbody _movingTargetRb;

    [Header("Values")]
    [SerializeField] private float slowRadius;
    [SerializeField] private float fleeRadius;
    public float maxSpeed;
    [SerializeField] private SteeringBehaviour steeringType;
    
    [Header("Seeking Path Nodes")]
    [SerializeField] private List<Transform> pathNodes;
    [SerializeField] private float nodeRadius;
    private int _currentNode;
    private float _distanceToNode;

    // Actions
    private delegate void SteeringTypes();

    private SteeringTypes _activeSteering;

    private enum SteeringBehaviour
    {
        Seek, Flee, ProximityFlee, Pursue, Arrive, Evade, SeekPath 
    };

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        ChooseSteeringType();
        _movingTargetRb = movingTarget.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);

        _activeSteering.Invoke();
    }

    private void SeekNodes(Transform target)
    {
        Vector3 targetDir = (target.position - transform.position).normalized;
        Vector3 targetVelocity = targetDir * maxSpeed;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = targetVelocity - currentVelocity;
        _rb.AddForce(steeringVector);
    }

    private void SeekPathNodes()
    {
        transform.LookAt(pathNodes[_currentNode]);
        
        _distanceToNode = (pathNodes[_currentNode].position - transform.position).magnitude;
        if (_distanceToNode <= nodeRadius)
        {
            _currentNode++;

            if (_currentNode == pathNodes.Count)
            {
                _currentNode = 0;
            }
        }
        SeekNodes(pathNodes[_currentNode]);        
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

    private void Arrive()
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

    public void SeekPowerup(Transform powerup)
    {
        target = powerup;
        steeringType = SteeringBehaviour.Seek;
        ChooseSteeringType();
    }

    public void BackToTrack()
    {
        steeringType = SteeringBehaviour.SeekPath;
        ChooseSteeringType();
    }

    private void ChooseSteeringType()
    {
        switch (steeringType)
        {
            case SteeringBehaviour.Seek:
                _activeSteering = Seeking;
                break;

            case SteeringBehaviour.Flee:
                _activeSteering = Flee;
                break;

            case SteeringBehaviour.ProximityFlee:
                _activeSteering = ProximityFlee;
                break;

            case SteeringBehaviour.Arrive:
                _activeSteering = Arrive;
                break;

            case SteeringBehaviour.Pursue:
                _activeSteering = Pursuit;
                break;

            case SteeringBehaviour.Evade:
                _activeSteering = Evade;
                break;
            
            case SteeringBehaviour.SeekPath:
                _activeSteering = SeekPathNodes;
                break;


        }
    }
}
