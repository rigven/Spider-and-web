using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LineBuiding : MonoBehaviour
{
    // Cached component references
    private Camera _camera;
    private SpiderSpawner _spiderSpawner;

    // Config params
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private GameObject _noClickArea;
    [Header("Segment prefabs")]
    [SerializeField] private GameObject _firstSegmentPrefab;
    [SerializeField] private GameObject _secondSegmentPrefab;
    [Header("Line parents")]
    [SerializeField] private GameObject _firstLineParent;
    [SerializeField] private GameObject _secondLineParent;
    [Header("Line building properties")]
    [SerializeField] private float _yStep = 0.25f;
    [SerializeField] private float _minDistanceBetweenLines = 0.5f;
    [Header("Other sizes")]
    [SerializeField] private float _screenLength = 3.6f;
    [SerializeField] private float _screenHeight = 2f;
    [SerializeField] private float _clickAreaLength = 0.6f;
    [SerializeField] private float _clickAreaHeight = 0.12f;

    // State
    private List<Vector2> _firstLineCoords = new List<Vector2>();
    private List<Vector2> _secondLineCoords = new List<Vector2>();
    private bool _firstLineIsReady;

    private void Start()
    {
        _camera = FindObjectOfType<Camera>();
        _spiderSpawner = FindObjectOfType<SpiderSpawner>();
    }

    private void OnMouseDown()
    {
        if (!_firstLineIsReady)
        {
            DrawLine(_firstLineCoords);
        }
        else
        {
            ChangeSizeOfNoClickArea(_secondLineCoords.Count + 1);
            DrawLine(_secondLineCoords);
        }
    }

    private void DrawLine(List<Vector2> coords)
    {
        coords.Add(_camera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
        MoveClickAreaDown();

        if (coords.Count == 1)
        {
            _instructionText.text = "Select the next point";
            return;
        }

        DrawSegment(coords[coords.Count - 2], coords[coords.Count - 1]);

        TryFinishLine();
    }

    private void MoveClickAreaDown()
    {
        if (!_firstLineIsReady)
        {
            transform.position = new Vector2(_firstLineCoords[_firstLineCoords.Count - 1].x, transform.position.y - _yStep);
        }
        else
        {
            transform.position = new Vector2(_secondLineCoords[_secondLineCoords.Count - 1].x, transform.position.y - _yStep);
        }
        
        transform.localScale = new Vector2(_clickAreaLength, transform.localScale.y);
    }

    private void DrawSegment(Vector2 firstPoint, Vector2 secondPoint)
    {
        GameObject newSegment;

        Vector3 midpoint = new Vector3((firstPoint.x + secondPoint.x) / 2, (firstPoint.y + secondPoint.y) / 2, -2);
        // Calculating the angle of rotation
        float hypotenuse = (firstPoint - secondPoint).magnitude;
        float cathetus = firstPoint.y - secondPoint.y;
        float angle = Mathf.Acos(cathetus / hypotenuse) * 180 / Mathf.PI;   // Angle in degrees
        if (secondPoint.x < firstPoint.x)
        {
            angle *= -1;
        }

        if (!_firstLineIsReady)
        {
            newSegment = Instantiate(_firstSegmentPrefab, midpoint, Quaternion.Euler(0f, 0f, angle));
            newSegment.transform.parent = _firstLineParent.transform;
        }
        else
        {
            newSegment = Instantiate(_secondSegmentPrefab, midpoint, Quaternion.Euler(0f, 0f, angle));
            newSegment.transform.parent = _secondLineParent.transform;
        }

    }

    private void TryFinishLine()
    {
        // If first line is finished
        if (_firstLineCoords.Count * _yStep > 2)
        {
            // If the first line is not marked as finished yet
            if (!_firstLineIsReady)
            {
                _firstLineIsReady = true;
                _instructionText.text = "Select the starting point of the second line";
                MoveAreasForSecondLine();
            }
            // If second line is finished too
            else if (_secondLineCoords.Count * _yStep > 2)
            {
                _instructionText.text = "Watch the spider weave its web";
                _spiderSpawner.InstantiateSpider(_firstLineCoords, _secondLineCoords);
                StartCoroutine(HideUI());
            }
        }
    }
    
    private void MoveAreasForSecondLine()
    {
        transform.position = new Vector2(_screenLength / 2, _screenHeight);
        transform.localScale = new Vector2(_screenLength, _clickAreaHeight);

        _noClickArea.transform.position = new Vector3(_screenLength, _screenHeight / 2, -0.5f);
        ChangeSizeOfNoClickArea(0);
    }

    /// <summary>Changes the size of the area that is not available for drawing so that the lines do not intersect.</summary>
    private void ChangeSizeOfNoClickArea(int pointNumber)
    {
        if (pointNumber <= _firstLineCoords.Count - 1)
        {
            _noClickArea.transform.localScale = new Vector2((_screenLength - _firstLineCoords[pointNumber].x + _minDistanceBetweenLines) * 2, _noClickArea.transform.localScale.y);
        }
    }

    private IEnumerator HideUI()
    {
        yield return new WaitForSeconds(3f);
        _noClickArea.SetActive(false);
        gameObject.SetActive(false);
        _instructionText.gameObject.SetActive(false);
    }
}
