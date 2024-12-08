using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plants Dictionary", menuName = "Scriptable Objects/Plants Dictionary")]
public class PlantsDictionaryScriptableObject : ScriptableObject
{
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

    [SerializeField] private GameObject apple_fruit, cherry_fruit, fico_fruit/*, AB_fruit, AC_fruit, BC_fruit*/;

    internal Dictionary<PlantGenetics.AllelesCouple, List<GameObject>> chromes_models;
    //internal Dictionary<PlantGenetics.AllelesCouple, GameObject> chromes_flowers;
    internal Dictionary<PlantGenetics.AllelesCouple, GameObject> chromes_fruit;

    [Header("Valori Output Frutti")]
    internal static byte BaseAverageFruitsOutput = 4;
    internal static byte HybridAverageFruitsOutput = 3;

    [Header("Valori Tempi Di Crescita (impostare multipli di 3 per convenzione)")]
    internal static byte BaseDefaultTimeToGrow = 3;
    internal static byte HybridDefaultTimeToGrow = 6;

    [Header("Valori Tempi Di Fioritura")]
    internal static byte BaseDefaultTimeToBloom = 3;  
    internal static byte HybridDefaultTimeToBloom = 5;

    [Header("Valori Tempi Di Crescita Frutti")]
    internal static byte BaseDefaultTimeToBearFruits = 3;
    internal static byte HybridDefaultTimeToBearFruits = 5;

    [Header("Valori Biodiversità default")]
    internal static byte BaseDefaultBiodivValue = 10;
    internal static byte HybridDefaultBiodivValue = 14;

    private void OnEnable()
    {
        chromes_models = new()
        {
            { PlantGenetics.AllelesCouple.AA, AA_models },
            { PlantGenetics.AllelesCouple.BB, BB_models },
            { PlantGenetics.AllelesCouple.CC, CC_models },
            { PlantGenetics.AllelesCouple.AB, AB_models },
            { PlantGenetics.AllelesCouple.AC, AC_models },
            { PlantGenetics.AllelesCouple.BC, BC_models },

        };

        chromes_fruit = new()
        {
            { PlantGenetics.AllelesCouple.AA, apple_fruit },
            { PlantGenetics.AllelesCouple.BB, cherry_fruit },
            { PlantGenetics.AllelesCouple.CC, fico_fruit }
        };
    }
}
