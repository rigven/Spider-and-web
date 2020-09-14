using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private float speed = 0f;
    private float amplitude = 0f;
    private float stepFactor = 0f;
    const int accelerationTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateParameters());
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSpeed();
    }

    private IEnumerator GenerateParameters()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 15));
            amplitude = UnityEngine.Random.Range(-1f, 1f);

            stepFactor = Mathf.Abs(speed - amplitude) / accelerationTime * Mathf.Sign(amplitude);
        }
    }

    private void ChangeSpeed()
    {
        float error = float.Epsilon + Mathf.Abs(stepFactor) * Time.deltaTime;

        // If speed and amplitude are close to zero
        if (Mathf.Abs(speed) < error && Mathf.Abs(amplitude) < error)
        {
            return;
        }

        if (Mathf.Abs(amplitude) > Mathf.Abs(speed))
        {
            speed += Time.deltaTime * stepFactor;
        }
        else
        {
            amplitude = 0f;
            speed -= Time.deltaTime * stepFactor;
        }
    }


    public Vector3 GetSpeed()
    {
        return new Vector3(speed, 0f, 0f);
    }
}
