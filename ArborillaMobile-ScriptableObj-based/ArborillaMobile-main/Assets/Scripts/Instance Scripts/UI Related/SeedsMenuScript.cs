using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SeedsMenuScript : MonoBehaviour
{
    [SerializeField] private RectTransform seeds_menu_content;
    [SerializeField] private GameObject relative_btn_mask;

    private Dictionary<PlantGenetics.AllelesCouple, (Text counter, ButtonSpriteSwap btn)> seeds_menu_counters = new();
    private bool is_selecting = false;
    private PlantGenetics.AllelesCouple cached_requested_chromosomes;

    internal Action<PlantGenetics.AllelesCouple> OnPlantingPhaseEnter;
    internal Action OnPlantingPhaseExit;

    private void OnEnable()
    {
        relative_btn_mask.SetActive(false);
    }

    private void OnDisable()
    {
        relative_btn_mask.SetActive(true);
    }

    private void Start()
    {
        Initialization();

        InventoryManagerSO.Singleton.OnFruitsCollectionUpdated += OnFruitsCollectionUpdated_UpdateSeedsInventory;
    }

    private void Initialization()       //istanzio i bottoni necessari in base ai frutti presenti nell'inventario
    {
        ButtonSpriteSwap loaded_btn;

        foreach (KeyValuePair<PlantGenetics.AllelesCouple, short> pair in InventoryManagerSO.Singleton.fruitsCollection)
        {
            loaded_btn = Instantiate(Resources.Load($"Prefabs/UI/Seeds/{pair.Key} Button") as GameObject, seeds_menu_content).GetComponent<ButtonSpriteSwap>();

            loaded_btn.onClick.AddListener(delegate { OnClick_PlantingPhase(pair.Key); });

            seeds_menu_counters.Add(pair.Key, (loaded_btn.GetComponentInChildren<Text>(), loaded_btn));

            seeds_menu_counters[pair.Key].counter.text = pair.Value.ToString();

            if (pair.Value <= 0) DisableButton(loaded_btn);
        }
    }

    private void OnClick_PlantingPhase(PlantGenetics.AllelesCouple _requested_chromosomes)       //chiamata da OnClick, gestisce gli stati della fase di selezione spot
    {
        if (!is_selecting)      //avvio per la prima volta la selezione
        {
            is_selecting = true;

            EnterPlantingPhase(_requested_chromosomes);
        }

        else if(is_selecting && _requested_chromosomes != cached_requested_chromosomes)     //la selezione non è conclusa e cambio pianta da generare
        {
            EnterPlantingPhase(_requested_chromosomes);
        }

        else        //annullo la selezione
        {
            ExitPlantingPhase(_requested_chromosomes);
        }
    }

    internal void ExitPlantingPhase(PlantGenetics.AllelesCouple current_request)      //uscita fase di piantagione
    {
        is_selecting = false;

        cached_requested_chromosomes = PlantGenetics.AllelesCouple.none;

        seeds_menu_counters[current_request].btn.SetAsDeselected();

        OnPlantingPhaseExit?.Invoke();      //iscritti i placeholder
    }

    private void EnterPlantingPhase(PlantGenetics.AllelesCouple new_request)       //entra in fase di piantagione dei semi richiesti
    {
        cached_requested_chromosomes = new_request;

        OnPlantingPhaseEnter?.Invoke(new_request);      //iscritti i placeholder
    }

    private void DisableButton(Button btn)
    {
        btn.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);     //maschera oscurante attivata
        btn.interactable = false;
    }

    private void EnableButton(Button btn)
    {
        btn.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(false);        //maschera oscurante disattivata
        btn.interactable = true;
    }

    private void OnFruitsCollectionUpdated_UpdateSeedsInventory(PlantGenetics.AllelesCouple key, short updated_amount)      //aggiorno i bottoni del menù in seguito ad un cambiamento dell'inventario relativo
    {

        if (seeds_menu_counters.ContainsKey(key))       //se il bottone era già presente nel menu allora ne aggiorno l'interfaccia
        {
            if(updated_amount > 0)      
            {
                seeds_menu_counters[key].counter.text = updated_amount.ToString();

                if (!seeds_menu_counters[key].btn.interactable) 
                    EnableButton(seeds_menu_counters[key].btn);
            }

            else
            {
                seeds_menu_counters[key].counter.text = "0";

                DisableButton(seeds_menu_counters[key].btn);
            }
        }

        else        //altrimenti aggiungo il nuovo bottone al menù
        {
            ButtonSpriteSwap new_button = Instantiate(Resources.Load($"Prefabs/UI/Seeds/{key} Button") as GameObject, seeds_menu_content).GetComponent<ButtonSpriteSwap>();

            new_button.onClick.AddListener(delegate { OnClick_PlantingPhase(key); });

            seeds_menu_counters.Add(key, (new_button.GetComponentInChildren<Text>(), new_button));

            seeds_menu_counters[key].counter.text = "1";
        }
    }

    private void OnDestroy()
    {
        InventoryManagerSO.Singleton.OnFruitsCollectionUpdated -= OnFruitsCollectionUpdated_UpdateSeedsInventory;
    }

}
