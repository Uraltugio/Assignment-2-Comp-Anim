using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp : MonoBehaviour
{

    [SerializeField] List<Transform> lerpPoints;
    private static float timeMin = 0f;
    float time = timeMin;
    [SerializeField] float lerpTime = 10f;
    int index = 0;
    int p0, p1, p2, p3;


    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Cos(Time.realtimeSinceStartup) * 0.5f + 0.5f;

        transform.position = Hermite(lerpPoints[p0].position, lerpPoints[p1].position, lerpPoints[p2].position, lerpPoints[p3].position, t);

        time += Time.smoothDeltaTime;

        if(time >= lerpTime)
        {
            time = timeMin;
            index++;
            index %= lerpPoints.Count;

            p1 = index;
            p0 = (index - 1 + lerpPoints.Count) % lerpPoints.Count;
            p2 = (index + 1) % lerpPoints.Count;
            p3 = (index + 2) % lerpPoints.Count;
        }


        //Debug.Log(t);
    }

    private Vector3 Hermite(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float tt = t * t;
        float ttt = t * t * t;

        Vector3 r0 = -1.0f * p0 + 3.0f * p1 + -3.0f * p2 + p3;
        Vector3 r1 = 2.0f * p0 + -5.0f * p1 + 4.0f * p2 - p3;
        Vector3 r2 = -1.0f * p0 + p2;
        Vector3 r3 = 2.0f * p1;

        return 0.5f * (ttt * r0 + tt * r1 + t * r2 + r3);
    }

    /* private Vector3 DeCastleJau(Transform a, Transform b, Transform c, Transform d, float t)
    {
        Vector3 pointA = a.position;
        Vector3 pointB = b.position;
        Vector3 pointC = c.position;
        Vector3 pointD = d.position;

        Vector3 ab = Vector3.Lerp(pointA, pointB, t);
        Vector3 bc = Vector3.Lerp(pointB, pointC, t);
        Vector3 cd = Vector3.Lerp(pointC, pointD, t);

        Vector3 ab_bc = Vector3.Lerp(ab, bc, t);
        Vector3 bc_cd = Vector3.Lerp(bc, cd, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);

    } */
}
