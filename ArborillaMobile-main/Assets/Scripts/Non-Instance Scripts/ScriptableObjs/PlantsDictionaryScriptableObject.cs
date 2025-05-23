using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plants Dictionary", menuName = "Scriptable Objects/Plants Dictionary")]
public class PlantsDictionaryScriptableObject : ScriptableObject
{
    public static PlantsDictionaryScriptableObject Singleton { get; private set; }

    [Header("Apple Models")]
    [SerializeField] private List<GameObject> AA_models = new List<GameObject>(5);

    [Header("Cherry Models")]
    [SerializeField] private List<GameObject> BB_models = new List<GameObject>(5);

    [Header("Fico Models")]
    [SerializeField] private List<GameObject> CC_models = new List<GameObject>(5);

    [Header("AB Models")]
    [SerializeField] private List<GameObject> AB_models = new List<GameObject>(5);

    [Header("AC Models")]
    [SerializeField] private List<GameObject> AC_models = new List<GameObject>(5);

    [Header("BC Models")]
    [SerializeField] private List<GameObject> BC_models = new List<GameObject>(5);

    [Header("Fruits Models")]
    [SerializeField] private GameObject apple_fruit;
    [SerializeField] private GameObject cherry_fruit;
    [SerializeField] private GameObject fico_fruit;

    internal Dictionary<PlantGenetics.AllelesCouple, List<GameObject>> chromesModels;
    //internal Dictionary<PlantGenetics.AllelesCouple, GameObject> chromesFlowers;
    internal Dictionary<PlantGenetics.AllelesCouple, GameObject> chromesFruit;

    [Header("Valori Output Frutti")]
    internal byte BaseAverageFruitsOutput = 4;
    internal byte HybridAverageFruitsOutput = 3;

    [Header("Valori Tempi Di Crescita (impostare multipli di 3 per convenzione)")]
    internal byte BaseDefaultTimeToGrow = 3;
    internal byte HybridDefaultTimeToGrow = 6;

    [Header("Valori Tempi Di Fioritura")]
    internal byte BaseDefaultTimeToBloom = 3;  
    internal byte HybridDefaultTimeToBloom = 5;

    [Header("Valori Tempi Di Crescita Frutti")]
    internal byte BaseDefaultTimeToBearFruits = 3;
    internal byte HybridDefaultTimeToBearFruits = 5;

    [Header("Valori BiodiversitÓ default")]
    internal byte BaseDefaultBiodivValue = 10;
    internal byte HybridDefaultBiodivValue = 14;

    private void OnEnable()
    {
        Singleton = this;

        Singleton.chromesModels = new()
        {
            { PlantGenetics.AllelesCouple.AA, AA_models },
            { PlantGenetics.AllelesCouple.BB, BB_models },
            { PlantGenetics.AllelesCouple.CC, CC_models },
            { PlantGenetics.AllelesCouple.AB, AB_models },
            { PlantGenetics.AllelesCouple.AC, AC_models },
            { PlantGenetics.AllelesCouple.BC, BC_models },

        };

        Singleton.chromesFruit = new()
        {
            { PlantGenetics.AllelesCouple.AA, apple_fruit },
            { PlantGenetics.AllelesCouple.BB, cherry_fruit },
            { PlantGenetics.AllelesCouple.CC, fico_fruit }
        };
    }
}
