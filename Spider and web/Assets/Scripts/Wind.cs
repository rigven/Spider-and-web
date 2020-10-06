using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    // Cached component references
    private Spider _spider;
    private SpiderSpawner _spiderSpawner;

    // Config params
    [SerializeField] private GameObject _pointPrefab;
    private const int AccelerationTime = 3;

    // State
    private float _speed = 0f;
    private float _amplitude = 0f;
    private float _stepFactor = 0f;
    public bool NeedWind = false;

    // Drawing state
    protected float _lowerEdge = 0f;
    protected float _upperEdge = 0f;
    protected float _lowerEdgeCoeff = 0f;
    protected float _upperEdgeCoeff = 0f;

    // Wind power is determined by a value from 0 to 1. 
    // Edges of the wind (y1 and y2) are generated randomly.
    // The slope coefficient of the first inclined line: k = 1 / y1.
    // The slope coefficient of the second inclined line: k = -1 / (y2 - 2).
    // 
    // Power
    //  |    
    // 1|    /¯¯¯¯¯¯¯\
    //  |   /         \
    //  |  /           \
    //  | /             \
    //  |/_______________\___
    //  0     y1    y2   2   y

    // Start is called before the first frame update
    void Start()
    {
        _spiderSpawner = FindObjectOfType<SpiderSpawner>();
        _spiderSpawner.SpiderCreated += this.StartTrackingSpider;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSpeed();
    }

    private void StartTrackingSpider()
    {
        _spiderSpawner.SpiderCreated -= this.StartTrackingSpider;
        _spider = FindObjectOfType<Spider>();
        _spider.ReachedFirstPoint += this.StartGenerateParameters;
    }

    private void StartGenerateParameters()
    {
        _spider.ReachedFirstPoint -= this.StartGenerateParameters;
        StartCoroutine(GenerateParameters());
    }

    private IEnumerator GenerateParameters()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 20));
            WindIndicatorDrawer.EraseWindIndicator();
            _amplitude = UnityEngine.Random.Range(-1f, 1f);
            _stepFactor = Mathf.Abs(_speed - _amplitude) / AccelerationTime * Mathf.Sign(_amplitude);

            _lowerEdge = UnityEngine.Random.Range(0f, 2f);
            _upperEdge = UnityEngine.Random.Range(_lowerEdge, Mathf.Min(2f, _lowerEdge + 0.3f));

            WindIndicatorDrawer.DrawWindIndicator(_lowerEdge, _upperEdge, _lowerEdgeCoeff, _upperEdgeCoeff, _pointPrefab);
        }
    }

    private void ChangeSpeed()
    {
        float error = float.Epsilon + Mathf.Abs(_stepFactor) * Time.deltaTime;

        // If speed and amplitude are close to zero
        if (Mathf.Abs(_speed) < error && Mathf.Abs(_amplitude) < error)
        {
            return;
        }

        if (Mathf.Abs(_amplitude) > Mathf.Abs(_speed))
        {
            _speed += Time.deltaTime * _stepFactor;
        }
        else
        {
            _amplitude = 0f;
            _speed -= Time.deltaTime * _stepFactor;
        }
    }

    public Vector3 GetSpeed(float y)
    {
        return new Vector3(_speed * GetPower(y), 0f, 0f);
    }

    private float GetPower(float y)
    {
        _lowerEdgeCoeff = 1f / _lowerEdge;
        _upperEdgeCoeff = -1f / (_upperEdge - 2f);

        if (y >= _lowerEdge && y <= _upperEdge)
        {
            return 1f;
        }

        if (y < _lowerEdge)
        {
            return _lowerEdgeCoeff * y;
        }

        return -_upperEdgeCoeff * (y - 2);

    }
}
