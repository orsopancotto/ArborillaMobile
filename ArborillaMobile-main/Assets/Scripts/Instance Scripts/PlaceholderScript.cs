using UnityEngine;

public class PlaceholderScript : MonoBehaviour, IUniversalInteractions
{
    internal PlantGenetics.AllelesCouple requested_chromosomes;
    [SerializeField] private SeedsMenuScript seeds_menu_script;

    private void Start()
    {
        seeds_menu_script.OnPlantingPhaseEnter += OnSelectionPhaseEntered_EnableObj;
        seeds_menu_script.OnPlantingPhaseExit += OnPlantingPhaseExit_DisablePlaceholder;
        gameObject.SetActive(false);
    }

    private void OnEnable()     //all'attivazione del placeholder controlla se è occupato o meno
    {
        if (IsSpotAvailable())
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, .35f);
        }

        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, .35f);
        }
    }

    private bool IsSpotAvailable()     //determina se il posto è libero
    {
        if (transform.parent.GetComponentInChildren<PlantScript>() == null) return true;

        else return false;
    }

    public void Grabbed() { }

    public void InteractionEnded() { }

    public void Selected()      //se selezionato ordina lo spawn della pianta richiesta, in questa posizione
    {
        if (!IsSpotAvailable()) return;

        SpawnManagerScript.Instance.SpawnPlant(GetComponentsInParent<Transform>()[1], requested_chromosomes);

        InventoryManagerScript.Singleton.UpdateFruitsCollection(requested_chromosomes, -1);

        seeds_menu_script.ExitPlantingPhase(requested_chromosomes);
    }

    public void Move(Vector3 v, float f) { }

    private void OnSelectionPhaseEntered_EnableObj(PlantGenetics.AllelesCouple _requested_chromes)
    {
        requested_chromosomes = _requested_chromes;
        if(!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    private void OnPlantingPhaseExit_DisablePlaceholder()
    {
        requested_chromosomes = PlantGenetics.AllelesCouple.none;
        gameObject.SetActive(false);
    }


}
