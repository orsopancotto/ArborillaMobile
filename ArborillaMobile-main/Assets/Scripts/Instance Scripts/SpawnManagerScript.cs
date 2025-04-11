using UnityEngine;

[DisallowMultipleComponent]
public class SpawnManagerScript : MonoBehaviour
{
    public static SpawnManagerScript Singleton { get; private set; }
    [SerializeField] private GameObject generic_plant, partial_plant;

    private void Awake()
    {
        Singleton = this;
    }

    /// <summary>
    /// Spawna una nuova pianta tramite UI: chiamato solamente da <see cref="PlaceholderScript.Selected"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="chromes"></param>
    internal void SpawnPlant(Transform parent, PlantGenetics.AllelesCouple chromes)       
    {
        Instantiate(generic_plant, parent)
            .GetComponent<PlantScript>()
            .Initialization(chromes);
    }

    /// <summary>
    /// Overload che spawna la pianta in fase di caricamento: chiamato solamente da <see cref="OasisManagerSO.GeneratePlants(int)"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="_chromes"></param>
    /// <param name="_starting_life_stage"></param>
    /// <param name="_son_chromes"></param>
    /// <param name="_starting_grow_timer"></param>
    internal void SpawnPlant(Transform parent, PlantGenetics.AllelesCouple _chromes, PlantScript.LifeStage _starting_life_stage, PlantGenetics.AllelesCouple _son_chromes, short _starting_grow_timer)     
    {
        Instantiate(generic_plant, parent)
            .GetComponent<PlantScript>()
            .Initialization(
            _chromes,
            _starting_life_stage, 
            _son_chromes, 
            _starting_grow_timer
            );
    }

    /// <summary>
    /// Overload che spawna una partial plant nella serra: chiamato solo da <see cref="GreenhouseManagerSO.InstantiatePlants(int)"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="Chromes"></param>
    /// <param name="HasBeenHarvested"></param>
    internal void SpawnPlant(Transform parent, PlantGenetics.AllelesCouple Chromes, bool HasBeenHarvested)
    {
        Instantiate(partial_plant, parent)
            .GetComponent<PartialPlantScript>()
            .Initialize(
            Chromes,
            HasBeenHarvested
            );
    }
}
