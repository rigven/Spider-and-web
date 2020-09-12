using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LineBuiding : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI instructionText;
    [SerializeField] GameObject noClickArea;
    [Header("Segment prefabs")]
    [SerializeField] GameObject firstSegmentPrefab;
    [SerializeField] GameObject secondSegmentPrefab;
    [Header("Line parents")]
    [SerializeField] GameObject firstLineParent;
    [SerializeField] GameObject secondLineParent;
    [Header("Line building properties")]
    [SerializeField] float yStep = 0.25f;
    [SerializeField] float minDistanceBetweenLines = 0.5f;
    [Header("Other sizes")]
    [SerializeField] float screenLength = 3.6f;
    [SerializeField] float screenHeight = 2f;
    [SerializeField] float clickAreaLength = 0.6f;
    [SerializeField] float clickAreaHeight = 0.12f;

    Camera camera;
    GameController gameController;

    List<Vector2> firstLineCoords = new List<Vector2>();
    List<Vector2> secondLineCoords = new List<Vector2>();
    bool firstLineIsReady;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        gameController = FindObjectOfType<GameController>();
    }

    private void OnMouseDown()
    {
        if (!firstLineIsReady)
        {
            DrawLine(firstLineCoords);
        }
        else
        {
            ChangeSizeOfNoClickArea(secondLineCoords.Count + 1);
            DrawLine(secondLineCoords);
        }
    }

    private void DrawLine(List<Vector2> coords)
    {
        coords.Add(camera.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
        MoveClickAreaDown();

        if (coords.Count == 1)
        {
            instructionText.text = "Select the next point";
            return;
        }

        DrawSegment(coords[coords.Count - 2], coords[coords.Count - 1]);

        TryFinishLine();
    }

    private void MoveClickAreaDown()
    {
        if (!firstLineIsReady)
        {
            transform.position = new Vector2(firstLineCoords[firstLineCoords.Count - 1].x, transform.position.y - yStep);
        }
        else
        {
            transform.position = new Vector2(secondLineCoords[secondLineCoords.Count - 1].x, transform.position.y - yStep);
        }
        
        transform.localScale = new Vector2(clickAreaLength, transform.localScale.y);
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

        if (!firstLineIsReady)
        {
            newSegment = Instantiate(firstSegmentPrefab, midpoint, Quaternion.Euler(0f, 0f, angle));
            newSegment.transform.parent = firstLineParent.transform;
        }
        else
        {
            newSegment = Instantiate(secondSegmentPrefab, midpoint, Quaternion.Euler(0f, 0f, angle));
            newSegment.transform.parent = secondLineParent.transform;
        }

    }

    private void TryFinishLine()
    {
        // If first line is finished
        if (firstLineCoords.Count * yStep > 2)
        {
            // If the first line is not marked as finished yet
            if (!firstLineIsReady)
            {
                firstLineIsReady = true;
                instructionText.text = "Select the starting point of the second line";
                MoveAreasForSecondLine();
            }
            // If second line is finished too
            else if (secondLineCoords.Count * yStep > 2)
            {
                instructionText.text = "Watch the spider weave its web";
                gameController.InstantiateSpider();
                StartCoroutine(HideUI());
            }
        }
    }
    
    private void MoveAreasForSecondLine()
    {
        transform.position = new Vector2(screenLength / 2, screenHeight);
        transform.localScale = new Vector2(screenLength, clickAreaHeight);

        noClickArea.transform.position = new Vector3(screenLength, screenHeight / 2, -0.5f);
        ChangeSizeOfNoClickArea(0);
    }

    /// <summary>
    /// Changes the size of the area that is not available for drawing so that the lines do not intersect.
    /// </summary>
    private void ChangeSizeOfNoClickArea(int pointNumber)
    {
        if (pointNumber <= firstLineCoords.Count - 1)
        {
            noClickArea.transform.localScale = new Vector2((screenLength - firstLineCoords[pointNumber].x + minDistanceBetweenLines) * 2, noClickArea.transform.localScale.y);
        }
    }

    private IEnumerator HideUI()
    {
        yield return new WaitForSeconds(3f);
        noClickArea.SetActive(false);
        gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
    }
}
