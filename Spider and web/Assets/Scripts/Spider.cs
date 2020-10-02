﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    // Cached component references
    private Rigidbody2D _rigidbody;
    private Vector3 _forceGravity;
    private Wind _wind;

    // Config params
    [SerializeField] private Thread _threadPrefab;
    private const float DescentSpeed = 0.1f;
    private const float HoveringFirstPoint = 0.75f;
    private const float Weight = 0.5f;

    // State
    private Thread _currentThread;
    private Vector3 _previousThreadPoint;
    private int _wovenThreadsNumber = 0;
    private bool _weavesThread = true;
    private float _shiftFromLastPoint = 0f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _wind = FindObjectOfType<Wind>();
        _forceGravity = FindObjectOfType<Gravity>().GetForceGravity();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        transform.position += (_forceGravity * Weight + _wind.GetSpeed(transform.position.y)) * Time.deltaTime;
    }

    void Update()
    {
        Move();
        WeaveThread();
    }

    private void Move()
    {
        // Initial descent down
        if (_wovenThreadsNumber == 0)
        {
            GoDownFromCeiling();
        }
    }

    private void GoDownFromCeiling()
    {
        _rigidbody.velocity = new Vector2(0f, -DescentSpeed);

        if (transform.position.y < HoveringFirstPoint)
        {
            _weavesThread = false;
            _currentThread.SetSpiderIsAttached(true);
            _rigidbody.velocity = new Vector2(0f, 0f);
        }
    }

    private void WeaveThread()
    {
        if (_weavesThread)
        {
            if (_currentThread == null)
            {
                _currentThread = InstantiateNewThread();
            }

            if ((transform.position - _previousThreadPoint).magnitude >= Thread.ThreadSegLen)
            {
                _currentThread.AddNewPoint(transform.position);
                _previousThreadPoint = transform.position;
            }
        }
    }

    private Thread InstantiateNewThread()
    {
        return Instantiate(_threadPrefab, transform.position, Quaternion.identity);
    }

    
}
