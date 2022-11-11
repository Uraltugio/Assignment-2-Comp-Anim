using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CatmullRomSpeedControlled : MonoBehaviour
{
	public Transform[] points;
	public float speed = 1f;
	[Range(1, 32)]
	public int sampleRate = 16;

	[System.Serializable]
	class SamplePoint
	{
		public float samplePosition;
		public float accumulatedDistance;

		public SamplePoint(float samplePosition, float distanceCovered)
		{
			this.samplePosition = samplePosition;
			this.accumulatedDistance = distanceCovered;
		}
	}
	//list of segment samples makes it easier to index later
	//imagine it like List<SegmentSamples>, and segment sample is a list of SamplePoints
	List<List<SamplePoint>> table = new List<List<SamplePoint>>();

	float distance = 0f;
	float accumDistance = 0f;
	int currentIndex = 0;
	int currentSample = 0;

	private void Start()
	{
		//make sure there are 4 points, else disable the component
		if (points.Length < 4)
		{
			enabled = false;
		}

		int size = points.Length;

		//calculate the speed graph table

		for (int i = 0; i < size; ++i)
		{
			List<SamplePoint> segment = new List<SamplePoint>();

            Vector3 p0 = points[(i - 1 + points.Length) % points.Length].position;
            Vector3 p1 = points[i].position;
            Vector3 p2 = points[(i + 1) % points.Length].position;
            Vector3 p3 = points[(i + 2) % points.Length].position;

            //calculate samples
            segment.Add(new SamplePoint(0f, accumDistance));
			Vector3 prevPos = Hermite(p0, p1, p2, p3, 0);
            for (int sample = 1; sample <= sampleRate; ++sample)
			{
				float t = (float)sample / sampleRate; // interpolation parameter (in speed graph it represents u)

				Vector3 curPos = Hermite(p0, p1, p2, p3, t);

				Vector3 resultant = (curPos - prevPos);

				accumDistance += resultant.magnitude; 

				segment.Add(new SamplePoint(t, accumDistance));

				prevPos = curPos;
            }
			table.Add(segment);
		}
	}

	private void Update()
	{
		distance += speed * Time.deltaTime;

		Debug.Log("distance: " + distance);
		Debug.Log("table: "+ table[currentIndex][(currentSample + 1)].accumulatedDistance);
		Debug.Log("Current Index: " + currentIndex);
		//check if we need to update our samples
		while(distance > table[currentIndex][(currentSample + 1)].accumulatedDistance)
		{
			
            //current sample is greater than samplerate, increment your currentindex (if its below the max), if interval changes, reset the distance, reset current index
            if (currentSample >= (sampleRate - 1))
			{
                //checks if index is beyond range, if it does reset current index and distance alongside current sample
                if (++currentIndex > table.Count - 1) 
				{
                    currentIndex = 0;
                    distance = 0;
                }
				currentSample = 0; // current sample needs to be reset every time a new table is made 
			}
			else
			{
				currentSample++;
			}
			
		}


		Vector3 p0 = points[(currentIndex - 1 + points.Length) % points.Length].position;
		Vector3 p1 = points[currentIndex].position;
		Vector3 p2 = points[(currentIndex + 1) % points.Length].position;
		Vector3 p3 = points[(currentIndex + 2) % points.Length].position;

		transform.position = Hermite(p0, p1, p2, p3, GetAdjustedT());
	}

	float GetAdjustedT()
	{
		SamplePoint current = table[currentIndex][currentSample];
		SamplePoint next = table[currentIndex][currentSample + 1];

		return Mathf.Lerp(current.samplePosition, next.samplePosition,
			(distance - current.accumulatedDistance) / (next.accumulatedDistance - current.accumulatedDistance)
		);
	}


	private void OnDrawGizmos()
	{
		Vector3 a, b, p0, p1, p2, p3;
		for (int i = 0; i < points.Length; i++)
		{
			a = points[i].position;
			p0 = points[(points.Length + i - 1) % points.Length].position;
			p1 = points[i].position;
			p2 = points[(i + 1) % points.Length].position;
			p3 = points[(i + 2) % points.Length].position;
			for (int j = 1; j <= sampleRate; ++j)
			{
				b = Hermite(p0, p1, p2, p3, (float)j / sampleRate);
				Gizmos.DrawLine(a, b);
				a = b;
			}
		}
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
}
