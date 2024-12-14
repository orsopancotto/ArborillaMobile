using System.Collections.Generic;
using UnityEngine;

public readonly struct PlantGenetics
{
    internal readonly AllelesCouple chromosomes;       //determina la coppia di alleli della pianta, si basa tutto su questa variabile

    internal readonly List<GameObject> Models;     //lista dei modelli relativi allo specifico albero (riferimento alle liste dello scriptable obj)

    internal readonly bool isBase;     //determina se la pianta in questione sia una base o meno

    internal readonly char[] alleles;

    internal readonly short defaultTimeToGrow, defaultTimeToBloom, defaultTimeToBearFruits;       //parametri vari (da calibrare)

    internal readonly byte defaultBiodiversityValue, avrgFruitsOutput /*, cashValue*/;

    internal PlantGenetics(AllelesCouple _chromosomes)     //costruttore
    {
        chromosomes = _chromosomes;

        //Models = Resources.Load<PlantsDictionaryScriptableObject>("Plants Dictionary").chromes_models[chromosomes];
        Models = PlantsDictionaryScriptableObject.Singleton.chromes_models[chromosomes];

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

    public override string ToString()       //fine al debugging
    {
        return $"alleli: {chromosomes}, base: {isBase},\n" +
            $"crescita pianta: {defaultTimeToGrow}, fioritura {defaultTimeToBloom},\n" +
            $" crescita frutti {defaultTimeToBearFruits}, biodiv {defaultBiodiversityValue}";
    }

}