using System;
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
    private bool _firstPointReached = false;
    //private bool _goalAchieved = false;
    private float _shiftFromLastPoint = 0f;

    // Event
    public delegate void ReachedFirstPointEventHandler();
    public event ReachedFirstPointEventHandler ReachedFirstPoint;

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

        if (!_firstPointReached && (transform.position.y < HoveringFirstPoint))
        {
            FinishInitialDescend();
        }
    }

    private void WeaveThread()
    {
        //Debug.Log(ReachedFirstPoint);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the spider touched the line on the first descent
        if (_wovenThreadsNumber == 0)
        {
            if (!_firstPointReached)
            {
                FinishWeawing();
                ReachedFirstPoint();
            }

            _currentThread.Attach(_currentThread.GetLastPointNumber(), collision.contacts[0].point);
            _currentThread.SetSpiderIsAttached(false);
            _wovenThreadsNumber++;

            return;
        }
    }

    private void FinishInitialDescend()
    {
        ReachedFirstPoint();
        _firstPointReached = true;
        FinishWeawing();

        _currentThread.SetSpiderIsAttached(true);
        _rigidbody.velocity = new Vector2(0f, 0f);
    }

    private void FinishWeawing()
    {
        _weavesThread = false;
    }

    private Thread InstantiateNewThread()
    {
        return Instantiate(_threadPrefab, transform.position, Quaternion.identity);
    }

    public Thread GetCurrentThread()
    {
        return _currentThread;
    }
}
