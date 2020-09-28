using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thread : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Wind wind;
    private Vector3 forceGravity;

    private List<ThreadSegment> threadSegments = new List<ThreadSegment>();
    public float threadSegLen = 0.05f;
    private float threadWidth = 0.01f;

    private Vector3 firstPointCoords;

    //TEMP
    bool spiderIsAttached = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        wind = FindObjectOfType<Wind>();
        forceGravity = FindObjectOfType<Gravity>().GetForceGravity();
    }

    void Update()
    {
        DrawThread();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void DrawThread()
    {
        lineRenderer.startWidth = threadWidth;
        lineRenderer.endWidth = threadWidth;

        Vector3[] threadPositions = new Vector3[threadSegments.Count];
        for (int i = 0; i < threadSegments.Count; i++)
        {
            threadPositions[i] = threadSegments[i].posNow;
        }

        lineRenderer.positionCount = threadPositions.Length;
        lineRenderer.SetPositions(threadPositions);
    }

    private void Simulate()
    {
        //SIMULATION
        for (int i = 0; i < threadSegments.Count; i++)
        {
            ThreadSegment segment = threadSegments[i];
            Vector3 velocity = segment.posNow - segment.posOld;
            segment.posOld = segment.posNow;
            segment.posNow += velocity + forceGravity * Time.deltaTime + wind.GetSpeed(segment.posNow.y) * Time.deltaTime;
            threadSegments[i] = segment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 40; i++)
        {
            ApplyConstraints();
        }
    }

    private void ApplyConstraints()
    {
        // Constraint (The first segment is always linked to a point)
        ThreadSegment segment = threadSegments[0];
        segment.posNow = firstPointCoords;
        threadSegments[0] = segment;

        // Constraint (Two points in the thread will always need to keep a certain distance apart)
        for (int i = 0; i < threadSegments.Count - 1; i++)
        {
            ThreadSegment firstSegment = threadSegments[i];
            ThreadSegment secondSegment = threadSegments[i + 1];

            float dist = (firstSegment.posNow - secondSegment.posNow).magnitude;
            float error = Mathf.Abs(dist - threadSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > threadSegLen)
            {
                changeDir = (firstSegment.posNow - secondSegment.posNow).normalized;
            }
            else /*if (dist < threadSegLen)*/
            {
                changeDir = (secondSegment.posNow - firstSegment.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSegment.posNow -= changeAmount * 0.5f;
                threadSegments[i] = firstSegment;
                secondSegment.posNow += changeAmount * 0.5f;
                threadSegments[i + 1] = secondSegment;
            }
            else
            {
                secondSegment.posNow += changeAmount;
                threadSegments[i + 1] = secondSegment;
            }
        }
        if (spiderIsAttached)
        {
            Attach(FindObjectOfType<Spider>());
        }
    }

    public void AddNewPoint(Vector3 coords)
    {
        if (threadSegments.Count == 0)
        {
            firstPointCoords = coords;
        }

        threadSegments.Add(new ThreadSegment(coords));
    }

    public void SetSpiderIsAttached(bool isAttached)    //TODO: delete this after the correct weaving of the web appears
    {
        spiderIsAttached = isAttached;
    }

    private void Attach(Spider spider)
    {
        ThreadSegment threadSegment = threadSegments[threadSegments.Count - 1];

        float dist = (spider.transform.position - threadSegment.posNow).magnitude;

        Vector3 changeDir = (spider.transform.position - threadSegment.posNow).normalized;
        Vector3 changeAmount = changeDir * dist;

        spider.transform.position -= changeAmount * 0.5f;
        threadSegment.posNow += changeAmount * 0.5f;

        threadSegments[threadSegments.Count - 1] = threadSegment;
    }

    public struct ThreadSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public ThreadSegment(Vector3 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}
