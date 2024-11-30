using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerScript : MonoBehaviour
{
    [SerializeField] private GameObject SceneLoader;

    private void Awake()
    {
        Instantiate(SceneLoader);
    }
}
