using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    Transform _colPoint; //Should get the child 'Left'(Index 0) or 'Right'(Index 1)
    private Rigidbody _rb;
    [SerializeField] private Steering_Seek steering; //Max speed

    bool _inTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_inTrigger) //Detects upcoming collision in trigger
        {
            float distance = Vector3.Distance(this.transform.position, _colPoint.transform.position);

            _rb.AddForce(Seek(_colPoint.position, _rb, steering.maxSpeed));

            if (distance < _colPoint.GetComponent<SphereCollider>().radius)
            {
                _inTrigger = false;
            }
        }

    }

    public Vector3 Seek(Vector3 target, Rigidbody current, float speed)
    {
        Vector3 targetDirection = (target - current.position).normalized;
        Vector3 currentVelocity = current.velocity;
        Vector3 desiredVelocity = targetDirection * speed - currentVelocity;
        return desiredVelocity;
    }

    public void SetInTrigger(bool state)
    {
        _inTrigger = state;
    }

    public void SetColPoint(Transform target)
    {
        _colPoint = target;
    }
}
