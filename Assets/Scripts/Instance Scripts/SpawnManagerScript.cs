using UnityEngine;

[DisallowMultipleComponent]
public class SpawnManagerScript : MonoBehaviour
{
    public static SpawnManagerScript Instance { get; private set; }
    [SerializeField] private GameObject generic_plant;

    private void Awake()
    {
        Instance = this;
    }

    //overload "base" del metodo, utilizzato da UI in fase di semina:
    //genera una pianta specificata per la prima volta
    internal void SpawnPlant (Transform parent, PlantGenetics.AllelesCouple _chromes)       
    {
        Instantiate(generic_plant, parent).GetComponent<PlantScript>().Initialization(_chromes);
    }

    //overload specifico del metodo, utilizzato da PlantDataManager in fase di caricamento
    internal void SpawnPlant (Transform parent, PlantGenetics.AllelesCouple _chromes, PlantScript.LifeStage _starting_life_stage, PlantGenetics.AllelesCouple _son_chromes, short _starting_grow_timer)     
    {
        Instantiate(generic_plant, parent).GetComponent<PlantScript>().Initialization(_chromes, _starting_life_stage, _son_chromes, _starting_grow_timer);
    }
}
