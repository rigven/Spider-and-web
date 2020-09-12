using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thread : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<ThreadSegment> threadSegments = new List<ThreadSegment>();
    public float threadSegLen = 0.05f;
    private float threadWidth = 0.01f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawThread();
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

    public void AddNewPoint(Vector2 coords)
    {
        threadSegments.Add(new ThreadSegment(coords));
        Debug.Log(coords);
    }

    public struct ThreadSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public ThreadSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}
