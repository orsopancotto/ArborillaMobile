using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImpollinationCanvasScript : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private PlantScript parent_PlantScript;

    private List<Button> buttons_cached = new();

    private void OnEnable()     //carica i bottoni necessari a seconda degli elementi presenti nell'inventario
    {
        Button btn;

        foreach(var pair in InventoryManagerSO.Singleton.pollenCollection)
        {
            if (pair.Value > 0)
            {
                btn = Instantiate(Resources.Load($"Prefabs/UI/Seeds/{pair.Key} Button") as GameObject, content).GetComponent<Button>();

                btn.GetComponentInChildren<Text>().enabled = false;

                btn.onClick.AddListener(delegate { parent_PlantScript.ArtificialImpollination_OnClick(pair.Key); });

                buttons_cached.Add(btn);
            }
        }
    }

    private void OnDisable()        //distrugge tutti i bottoni tranne quello di default di uscita
    {
        foreach(var element in buttons_cached)
        {
            if (element.gameObject.name == "Exit Button") continue;

            Destroy(element.gameObject);
        }

        buttons_cached.Clear();
    }
}
