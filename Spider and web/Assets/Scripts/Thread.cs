using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thread : MonoBehaviour
{
    // Cached component references
    private LineRenderer _lineRenderer;
    private Spider _spider;
    private Wind _wind;
    private Vector3 _forceGravity;

    // Config params
    public const float ThreadSegLen = 0.05f;
    private const float ThreadWidth = 0.01f;

    // State
    private List<ThreadSegment> _threadSegments = new List<ThreadSegment>();
    private Vector3 _firstPointCoords;

    // TEMP
    bool _spiderIsAttached = false;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _spider = FindObjectOfType<Spider>();
        _wind = FindObjectOfType<Wind>();
        _forceGravity = FindObjectOfType<Gravity>().GetForceGravity();
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
        _lineRenderer.startWidth = ThreadWidth;
        _lineRenderer.endWidth = ThreadWidth;

        Vector3[] threadPositions = new Vector3[_threadSegments.Count];
        for (int i = 0; i < _threadSegments.Count; i++)
        {
            threadPositions[i] = _threadSegments[i].PosNow;
        }

        _lineRenderer.positionCount = threadPositions.Length;
        _lineRenderer.SetPositions(threadPositions);
    }

    private void Simulate()
    {
        //SIMULATION
        for (int i = 0; i < _threadSegments.Count; i++)
        {
            ThreadSegment segment = _threadSegments[i];
            Vector3 velocity = segment.PosNow - segment.PosOld;
            segment.PosOld = segment.PosNow;
            segment.PosNow += velocity + _forceGravity * Time.deltaTime + _wind.GetSpeed(segment.PosNow.y) * Time.deltaTime;
            _threadSegments[i] = segment;
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
        ThreadSegment segment = _threadSegments[0];
        segment.PosNow = _firstPointCoords;
        _threadSegments[0] = segment;

        // Constraint (Two points in the thread will always need to keep a certain distance apart)
        for (int i = 0; i < _threadSegments.Count - 1; i++)
        {
            ThreadSegment firstSegment = _threadSegments[i];
            ThreadSegment secondSegment = _threadSegments[i + 1];

            float dist = (firstSegment.PosNow - secondSegment.PosNow).magnitude;
            float error = Mathf.Abs(dist - ThreadSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > ThreadSegLen)
            {
                changeDir = (firstSegment.PosNow - secondSegment.PosNow).normalized;
            }
            else
            {
                changeDir = (secondSegment.PosNow - firstSegment.PosNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSegment.PosNow -= changeAmount * 0.5f;
                _threadSegments[i] = firstSegment;
                secondSegment.PosNow += changeAmount * 0.5f;
                _threadSegments[i + 1] = secondSegment;
            }
            else
            {
                secondSegment.PosNow += changeAmount;
                _threadSegments[i + 1] = secondSegment;
            }
        }
        if (_spiderIsAttached)
        {
            Attach(_spider);
        }
    }

    public void AddNewPoint(Vector3 coords)
    {
        if (_threadSegments.Count == 0)
        {
            _firstPointCoords = coords;
        }

        _threadSegments.Add(new ThreadSegment(coords));
    }

    public void SetSpiderIsAttached(bool isAttached)    //TODO: delete this after the correct weaving of the web appears
    {
        _spiderIsAttached = isAttached;
    }

    private void Attach(Spider spider)
    {
        ThreadSegment threadSegment = _threadSegments[_threadSegments.Count - 1];

        float dist = (spider.transform.position - threadSegment.PosNow).magnitude;

        Vector3 changeDir = (spider.transform.position - threadSegment.PosNow).normalized;
        Vector3 changeAmount = changeDir * dist;

        spider.transform.position -= changeAmount * 0.5f;
        threadSegment.PosNow += changeAmount * 0.5f;

        _threadSegments[_threadSegments.Count - 1] = threadSegment;
    }

    public struct ThreadSegment
    {
        public Vector3 PosNow;
        public Vector3 PosOld;

        public ThreadSegment(Vector3 pos)
        {
            this.PosNow = pos;
            this.PosOld = pos;
        }
    }
}
