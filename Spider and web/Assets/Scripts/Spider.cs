using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] Thread threadPrefab;
    [SerializeField] float weavingSpeed = 0.1f;
    [SerializeField] float hoveringFirstPoint = 0.75f;
    [SerializeField] float weight = 2f;

    Rigidbody2D rigidbody;
    private Vector3 forceGravity;
    private Wind wind;

    Thread currentThread;
    Vector3 previousThreadPoint, currentThreadPointIndex;
    int wovenThreadsNumber = 0;
    bool weavesThread = true;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        wind = FindObjectOfType<Wind>();
        forceGravity = FindObjectOfType<Gravity>().GetForceGravity();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        transform.position += (forceGravity * weight + wind.GetSpeed(transform.position.y)) * Time.deltaTime;
    }

    void Update()
    {
        Move();
        WeaveThread();
    }

    private void Move()
    {
        // Initial descent down
        if (wovenThreadsNumber == 0)
        {
            GoDownFromCeiling();
        }
    }

    private void GoDownFromCeiling()
    {
        rigidbody.velocity = new Vector2(0f, -weavingSpeed);

        if (transform.position.y < hoveringFirstPoint)
        {
            weavesThread = false;
            currentThread.SetSpiderIsAttached(true);
            rigidbody.velocity = new Vector2(0f, 0f);
        }
    }

    private void WeaveThread()
    {
        if (weavesThread)
        {
            if (currentThread == null)
            {
                currentThread = InstantiateNewThread();
            }

            if ((transform.position - previousThreadPoint).magnitude >= currentThread.threadSegLen)
            {
                currentThread.AddNewPoint(transform.position);
                previousThreadPoint = transform.position;
            }
        }
    }

    private Thread InstantiateNewThread()
    {
        return Instantiate(threadPrefab, transform.position, Quaternion.identity);
    }
}
