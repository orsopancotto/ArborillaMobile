using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PollenMenuScript : MonoBehaviour
{
    private Dictionary<PlantGenetics.AllelesCouple, TextMeshProUGUI> pollen_inventory = new();
    [SerializeField] private RectTransform pollen_menu_content;
    [SerializeField] private GameObject relative_btn_mask;

    private void OnEnable()
    {
        relative_btn_mask.SetActive(false);
    }

    private void OnDisable()
    {
        relative_btn_mask.SetActive(true);
    }

    private async void Start()
    {
        await InventoryManagerScript.Instance.DataLoaded();

        Initialization();

        InventoryManagerScript.Instance.OnPollenCollectionUpdated += OnPollenCollectionUpdated_UpdateUI;
    }

    private void Initialization()        //inizializzo seeds_inventory con i dati salvati dalla sessione precedente, e ne aggiorno la UI
    {
        if(InventoryManagerScript.Instance._pollen_collection.Count > 0)
        {
            foreach (KeyValuePair<PlantGenetics.AllelesCouple, short> pair in InventoryManagerScript.Instance._pollen_collection)
            {
                pollen_inventory.Add(
                    pair.Key,
                    Instantiate(Resources.Load($"Prefabs/UI/Pollen & Fruits/{pair.Key} Panel") as GameObject, pollen_menu_content).GetComponentInChildren<TextMeshProUGUI>()
                    );

                pollen_inventory[pair.Key].SetText($"{pair.Key}: {pair.Value}");
            }
        }
    }

    private void OnPollenCollectionUpdated_UpdateUI(PlantGenetics.AllelesCouple key, short _updated_amount)       //risposta ad evento di raccolta polline; aggiorno UI con nuovo valore
    {
        if (pollen_inventory.ContainsKey(key))       //se possiedo già polline di questo tipo, aggiorno l'elemento di UI corrispondente
        {
            pollen_inventory[key].SetText($"{key}: {_updated_amount}");
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-tmp) al dizionario e ne aggiorno UI
        {
            pollen_inventory.Add(
                key,
                Instantiate(Resources.Load($"Prefabs/UI/Pollen & Fruits/{key} Panel") as GameObject, pollen_menu_content).GetComponentInChildren<TextMeshProUGUI>()
                );

            pollen_inventory[key].SetText($"{key}: {_updated_amount}");
        }
    }

}
