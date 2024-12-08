using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[DisallowMultipleComponent]
public class GreenhouseManagerScript : MonoBehaviour, IDataPersistance
{
    [SerializeField] private GameObject partial_plant_prefab;
    internal static GreenhouseManagerScript Singleton { get; private set; }

    internal Dictionary<string /*spot*/, (PlantGenetics.AllelesCouple Chromes, bool HasBeenHarvested)> greenhouse_plants;

    private void Awake()
    {
        Singleton = this;
    }

    public void LoadData()
    {
        greenhouse_plants = GameData.currentSessionData.greenhousePlants;

        GameObject loaded_plant;

        foreach(KeyValuePair<string, (PlantGenetics.AllelesCouple Chromes, bool HasBeenHarvested)> pair in greenhouse_plants)
        {
            loaded_plant = Instantiate(partial_plant_prefab, GameObject.Find(pair.Key).GetComponent<Transform>());

            loaded_plant.GetComponent<PartialPlantScript>().Initialize(pair.Value.Chromes, pair.Value.HasBeenHarvested);
        }
    }

    public void SaveData()
    {
        GameData.currentSessionData.greenhousePlants = greenhouse_plants;
    }
}
