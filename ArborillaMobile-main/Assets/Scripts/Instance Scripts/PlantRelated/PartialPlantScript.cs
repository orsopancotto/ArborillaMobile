using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PartialPlantScript : MonoBehaviour, IUniversalInteractions
{
    private PlantGenetics genetics;
    private bool has_been_harvested = false;
    private CameraAnimations camera_animations_script;

    private void Start()
    {
        camera_animations_script = Camera.main.GetComponent<CameraAnimations>();
    }

    internal void Initialize(PlantGenetics.AllelesCouple set_chromes, bool set_harvested)
    {
        genetics = new(set_chromes);

        has_been_harvested = set_harvested;

        Instantiate(genetics.Models[3], transform);
    }

    private void StartFruitsHarvest()
    {
        Transform[] anchors = Instantiate(genetics.Models[4], transform).GetComponentsInChildren<Transform>();

        GameObject fruit = PlantsDictionaryScriptableObject.Singleton.chromes_fruit[genetics.chromosomes];

        byte amount = (byte)UnityEngine.Random.Range(1, genetics.avrgFruitsOutput + 1);

        for (byte i = 1; i <= amount; i++)      //parto da 1 perche anchors[0] è il transform del parent
        {
            Instantiate(fruit, anchors[i]).GetComponent<FruitScript>().Parameters = new FruitScript.Params(genetics.chromosomes, gameObject);
        }

        camera_animations_script.MoveToPlant(new Vector2(transform.position.x, transform.position.z));

    }

    internal void FruitsHarvesEnded()
    {

    }

    public void Grabbed()
    {
    }

    public void InteractionEnded()
    {
        Selected();
    }

    public void Move(Vector3 touch_position, float speed)
    {
    }

    public void Selected()
    {
        if (has_been_harvested) return;

        GetComponentInChildren<Collider>().enabled = false;

        GreenhouseManagerSO.Singleton.greenhouse_plants[transform.parent.name] = (genetics.chromosomes, true);

        StartFruitsHarvest();
    }
}
