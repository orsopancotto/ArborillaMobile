using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struttura identificativa della pianta
/// </summary>
public readonly struct PlantGenetics
{
    /// <summary>
    /// Rappresenta la coppia di alleli della pianta
    /// </summary>
    internal readonly AllelesCouple chromosomes;

    /// <summary>
    /// Lista dei modelli di questa specie di pianta (vedi <see cref="PlantsDictionaryScriptableObject.chromes_models"/>)
    /// </summary>
    internal readonly List<GameObject> models;

    /// <summary>
    /// Indica se la pianta è una base o meno (caso XX), utilizzato da <see cref="CouplingAlgorithm"/>
    /// </summary>
    internal readonly bool isBase;

    internal readonly char[] alleles;

    internal readonly short defaultTimeToGrow, defaultTimeToBloom, defaultTimeToBearFruits;       //parametri vari (da calibrare)

    internal readonly byte defaultBiodiversityValue, avrgFruitsOutput;

    internal PlantGenetics(AllelesCouple _chromosomes)     //costruttore
    {
        chromosomes = _chromosomes;

        models = PlantsDictionaryScriptableObject.Singleton.chromesModels[chromosomes];

        alleles = new char[2];
        alleles = chromosomes.ToString().ToCharArray();

        isBase = alleles[0] == alleles[1];

        if (isBase)
        {
            defaultTimeToGrow = PlantsDictionaryScriptableObject.Singleton.BaseDefaultTimeToGrow;
            defaultTimeToBloom = PlantsDictionaryScriptableObject.Singleton.BaseDefaultTimeToBloom;
            defaultTimeToBearFruits = PlantsDictionaryScriptableObject.Singleton.BaseDefaultTimeToBearFruits;
            avrgFruitsOutput = PlantsDictionaryScriptableObject.Singleton.BaseAverageFruitsOutput;
            defaultBiodiversityValue = PlantsDictionaryScriptableObject.Singleton.BaseDefaultBiodivValue;
        }
        else
        {
            defaultTimeToGrow = PlantsDictionaryScriptableObject.Singleton.HybridDefaultTimeToGrow;
            defaultTimeToBloom = PlantsDictionaryScriptableObject.Singleton.HybridDefaultTimeToBloom;
            defaultTimeToBearFruits = PlantsDictionaryScriptableObject.Singleton.HybridDefaultTimeToBearFruits;
            avrgFruitsOutput = PlantsDictionaryScriptableObject.Singleton.HybridAverageFruitsOutput;
            defaultBiodiversityValue = PlantsDictionaryScriptableObject.Singleton.HybridDefaultBiodivValue;
        }
    }

    public enum AllelesCouple     //enumerazione di tutte le possibili combinazioni
    {
        none, AA /*melo*/, BB /*ciliegio*/, CC /*fico*/, AB, AC, BC
    }
}