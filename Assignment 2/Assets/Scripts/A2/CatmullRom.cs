using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRom : MonoBehaviour
{
    public List<Transform> points;
    float t;
    float h = 0.5f; //Divide the matrix
    float sampleRate = 16;

    public bool looping = false;
    //public float travelTime;

    int cS = 0; //Control Start
    int s = 1; //Start
    int e = 2; //End
    int cE = 3; //Control End

    [SerializeField] private float speed = 0.5f;
    private float current; //Starts at 0, progress towards 1(target)
    private float target = 1;

    // Update is called once per frame
    void Update()
    {
        if (!looping)
        {
            t = Mathf.Cos(Time.realtimeSinceStartup) * 0.5f + 0.5f;
            transform.position = Catmull(points[0].position, points[1].position,
                points[2].position, points[3].position, t);
        }
        else if (looping)
        {
            loopCatmull();
        }
    }

    public Vector3 Catmull(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 r0 = -1.0f * p0 + 3.0f * p1 + -3.0f * p2 + 1.0f * p3;
        Vector3 r1 = 2.0f * p0 + -5.0f * p1 + 4.0f * p2 + -1.0f * p3;
        Vector3 r2 = -1.0f * p0 + 1.0f * p2;
        Vector3 r3 = 2.0f * p1;

        return (t * t * t) * (h * (r0)) + 
            (t * t) * (h * (r1)) + 
            t * (h * (r2)) +
            h * (r3);
    }

    public void loopCatmull()
    {
        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);

        transform.position = Catmull(points[cS].position, points[s].position,
            points[e].position, points[cE].position, current);

        if (current == 1f)
        {
            current = 0;

            cS++;
            s++;
            e++;
            cE++;

            if (cS >= points.Count)
            {
                cS = 0;
            }
            else if (s >= points.Count)
            {
                s = 0;
            }
            else if (e >= points.Count)
            {
                e = 0;
            }
            else if (cE >= points.Count)
            {
                cE = 0;
            }

            //Debug
            Debug.Log("CS: " + cS);
            Debug.Log("S: " + s);
            Debug.Log("E: " + e);
            Debug.Log("CE: " + cE);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 a, b, p0, p1, p2, p3;
        for (int i = 0; i < points.Count; i++)
        {
            a = points[i].position;
            p0 = points[(points.Count + i - 1) % points.Count].position;
            p1 = points[i].position;
            p2 = points[(i + 1) % points.Count].position;
            p3 = points[(i + 2) % points.Count].position;
            for (int j = 1; j <= sampleRate; ++j)
            {
                b = Catmull(p0, p1, p2, p3, (float)j / sampleRate);
                Gizmos.DrawLine(a, b);
                a = b;
            }
        }
    }
}
