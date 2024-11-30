using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImpollinationCanvasScript : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private PlantScript parent_PlantScript;

    private void OnEnable()
    {
        //DisableOthers();

        Button btn;

        foreach(KeyValuePair<PlantGenetics.AllelesCouple, short> pair in InventoryManagerScript.Instance._pollen_collection)
        {
            if (pair.Value > 0)
            {
                btn = Instantiate(Resources.Load($"Prefabs/UI/Seeds/{pair.Key} Button") as GameObject, content).GetComponent<Button>();

                btn.GetComponentInChildren<Text>().enabled = false;

                btn.onClick.AddListener(delegate { parent_PlantScript.ArtificialImpollination_OnClick(pair.Key); });

            }
        }
    }

    private void OnDisable()
    {
        Button[] btns = GetComponentsInChildren<Button>();

        foreach(Button element in btns)
        {
            if(element.gameObject.name != "Exit Button")
            {
                element.onClick.RemoveAllListeners();

                Destroy(element.gameObject);
            }
        }
    }

    //private void DisableOthers()        //pare essere impegnativo a livello computazionale :(
    //{
    //    ImpollinationCanvasScript[] active_canvas = FindObjectsByType<ImpollinationCanvasScript>(sortMode: FindObjectsSortMode.None, findObjectsInactive: FindObjectsInactive.Exclude);

    //    if(active_canvas.Length == 0)
    //    {
    //        return;
    //    }

    //    foreach (ImpollinationCanvasScript canvas in active_canvas)
    //    {
    //        if (canvas != this)
    //        {
    //            canvas.gameObject.SetActive(false);

    //            return;
    //        }
    //    }

    //}
}
