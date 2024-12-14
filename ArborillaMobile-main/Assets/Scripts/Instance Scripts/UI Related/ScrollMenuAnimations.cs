using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMenuAnimations : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private ScrollRect scroll_menu;

    [Header("MOTION PARAMS")]
    [Range(-500, -10)]
    [SerializeField] private float amplitude = -150;

    [Range(.01f, 1)]
    [SerializeField] private float time_constant = .15f;

    [Range(1, 20)]
    [SerializeField] private float pulse = 10;

    [Range(.1f, 5)]
    [SerializeField] private float phase = 3;

    private void OnEnable()
    {
        StartCoroutine(MenuOpenedItemAnimation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator MenuOpenedItemAnimation()
    {
        float time_elapsed = 0;

        while(time_elapsed < 1.10f)
        {
            content.localPosition = new Vector3(content.localPosition.x, DampedHarmonicFunction(time_elapsed, amplitude, time_constant, pulse, phase), 0);

            time_elapsed += Time.deltaTime;

            yield return null;
        }

        content.localPosition = new Vector3(content.localPosition.x, 0, 0);

        scroll_menu.enabled = true;
    }

    private float DampedHarmonicFunction(float t, float A, float T, float w, float o)
    {
        return A * Mathf.Pow((float)Math.E, -t / T) * Mathf.Cos(w * t + o);
    }
}
