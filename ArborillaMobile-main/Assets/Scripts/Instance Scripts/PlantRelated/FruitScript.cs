using System.Collections;
using UnityEngine;
using CandyCoded.HapticFeedback;

public class FruitScript : MonoBehaviour, IUniversalInteractions
{
#nullable enable
    internal readonly struct Params
    {
        internal readonly PlantGenetics.AllelesCouple fruitChromes;
        internal readonly GameObject parentPlant;

        internal Params(PlantGenetics.AllelesCouple _chromes, GameObject parent_plant)
        {
            fruitChromes = _chromes;
            parentPlant = parent_plant;
        }
    }

    internal Params Parameters { private get; set; }

#if UNITY_EDITOR
    [Range(.01f, .2f)]
    [SerializeField] private float shrink_speed = .05f;
#else
    private float shrink_speed = .2f;
#endif

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
        HapticFeedback.LightFeedback();

        InventoryManagerSO.Singleton.UpdateFruitsCollection(Parameters.fruitChromes, 1);

        StartCoroutine(ShrinkAnimation());
    }

    private IEnumerator ShrinkAnimation()
    {
        float target_magnitude = (transform.localScale * .025f).magnitude;

        while (transform.localScale.magnitude > target_magnitude)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrink_speed);

            yield return null;
        }

        if (IsLastOne())
        {
            Camera.main.GetComponent<CameraAnimations>().ResetPosition();

            TriggerEndOfHarvest();
        }

        Destroy(gameObject);
    }

    private void TriggerEndOfHarvest()
    {
        if (Parameters.parentPlant.TryGetComponent(out PlantScript parent)) parent.FruitsHarvestEnded();

        else Parameters.parentPlant.GetComponent<PartialPlantScript>().FruitsHarvesEnded();
    }

    private bool IsLastOne()
    {
        return Parameters.parentPlant.GetComponentsInChildren<FruitScript>().Length <= 1;
    }

}
