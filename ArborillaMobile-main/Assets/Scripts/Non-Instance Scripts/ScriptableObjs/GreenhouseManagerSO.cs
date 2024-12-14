using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Greenhouse Manager", menuName = "Scriptable Objects/Greenhouse Manager")]
public class GreenhouseManagerSO : ScriptableObject, IDataPersistance
{
    public static GreenhouseManagerSO Singleton { get; private set; }

    [SerializeField] private GameObject partial_plant_prefab;

    internal Dictionary<string, (PlantGenetics.AllelesCouple Chromes, bool HasBeenHarvested)> greenhouse_plants;

    private void OnEnable()
    {
        Singleton = this;
    }

    public void InitializeObj()
    {
        GameManager.Singleton.OnSceneLoaded += InstantiatePlants;
    }

    public void LoadData()
    {
        greenhouse_plants = GameData.currentSessionData.greenhousePlants;
    }

    public void SaveData()
    {
        GameData.currentSessionData.greenhousePlants = greenhouse_plants;
    }

    private void InstantiatePlants(int scene_index)
    {
        if (scene_index != 3) return;

        foreach (KeyValuePair<string, (PlantGenetics.AllelesCouple Chromes, bool HasBeenHarvested)> pair in greenhouse_plants)
        {
            SpawnManagerScript.Singleton.SpawnPlant(
                GameObject.Find(pair.Key).GetComponent<Transform>(),
                pair.Value.Chromes,
                pair.Value.HasBeenHarvested
                );
        }

    }
}
