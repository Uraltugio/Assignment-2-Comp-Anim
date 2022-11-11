using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PathFollowing : MonoBehaviour
{
    [SerializeField] private List<Transform> pathNodes;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float nodeRadius;
    private Rigidbody _rb;
    private int _currentNode;
    private float _distanceToNode;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
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
    private void SeekNodes(Transform target)
    {
        Vector3 targetDir = (target.position - transform.position).normalized;
        Vector3 targetVelocity = targetDir * maxSpeed;
        Vector3 currentVelocity = _rb.velocity;
        Vector3 steeringVector = targetVelocity - currentVelocity;
        _rb.AddForce(steeringVector);
    }
    
}
