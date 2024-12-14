using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceCheckScript : MonoBehaviour
{
    [SerializeField] private Text fps_displayer;

    [Range(.01f, .1f)]
    [SerializeField] private float refresh_rate = .05f;

    [SerializeField] private bool DoPerformanceTest;

    private void Start()
    {
        StartCoroutine(CountFPS());
    }

    private IEnumerator CountFPS()
    {
#if UNITY_EDITOR
        if (DoPerformanceTest)
        {
            while (Time.timeSinceLevelLoad < 12)
            {
                fps_displayer.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";

                yield return new WaitForSeconds(refresh_rate);
            }

            EditorApplication.ExitPlaymode();
        }

        else
        {
            while (true)
            {
                fps_displayer.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";

                yield return new WaitForSeconds(refresh_rate);
            }
        }
#else
            while (true)
            {
                fps_displayer.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";

                yield return new WaitForSeconds(refresh_rate);
            }

#endif
    }

    private void OnApplicationQuit()
    {
        Debug.Log($"{(short)(Time.frameCount / Time.unscaledTime)}");
    }
}
