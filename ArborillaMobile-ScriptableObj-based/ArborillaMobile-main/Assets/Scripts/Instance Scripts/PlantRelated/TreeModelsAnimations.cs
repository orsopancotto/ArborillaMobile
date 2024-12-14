using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeModelsAnimations : MonoBehaviour
{
    [Header("MOTION PARAMS")]
    [Range(-1, 0)]
    [SerializeField] private float amplitude = -.1f;

    [Range(.01f, 1)]
    [SerializeField] private float time_constant = .15f;

    [Range(1, 50)]
    [SerializeField] private float pulse = 24.3f;

    [Range(.1f, 5)]
    [SerializeField] private float phase = 3;

    private void Start()
    {
        StartCoroutine(StageChangedAnimation());
    }


    private IEnumerator StageChangedAnimation()
    {
        float time_elapsed = 0;

        Vector3 initial_scale = transform.localScale;

        while(time_elapsed < .72f)
        {
            time_elapsed += Time.deltaTime;

            transform.localScale = initial_scale * DampedHarmonicFunction(time_elapsed, amplitude, time_constant, pulse, phase);

            yield return null;
        }

        transform.localScale = initial_scale;
    }

    private float DampedHarmonicFunction(float t, float A, float T, float w, float o)
    {
        return (A * Mathf.Pow((float)Math.E, -t / T) * Mathf.Cos(w * t + o) + 1);
    }

}
