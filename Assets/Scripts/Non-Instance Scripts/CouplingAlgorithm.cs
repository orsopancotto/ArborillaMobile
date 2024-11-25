using System;

public static class CouplingAlgorithm
{
    internal static PlantGenetics.AllelesCouple CalculateHybrid(PlantGenetics plant, PlantGenetics pollen)
    {

        if (plant.chromosomes == pollen.chromosomes) return plant.chromosomes;

        else if (plant.isBase && pollen.isBase) return SimpleHybridation(plant.alleles, pollen.alleles);

        else return GeneralHybridation(plant.alleles, pollen.alleles);
    }

    private static PlantGenetics.AllelesCouple SimpleHybridation(char[] a, char[] b)      //BASE + BASE
    {
        string combination = new string(a, 0, 1) + new string(b, 0, 1);  //string composta dal primo allele di entrambe le coppie a[0] + b[0]

        /* TryParse ritorna l'enum default (none in questo caso) se combination (to enum) non corrisponde a nessun enum,
         * questo vuol dire che la stringa formata è al contrario (es: BA al posto di AB).
         * Quindi combination viene sovrascritta invertendone i caratteri
         */
        if (Enum.TryParse(combination, false, out PlantGenetics.AllelesCouple _ /* boh l'IDE mi ha consigliato di metterci _ */))
        {
            return Enum.Parse<PlantGenetics.AllelesCouple>(combination);
        }

        else
        {
            combination = new string(b, 0, 1) + new string(a, 0, 1);

            return Enum.Parse<PlantGenetics.AllelesCouple>(combination);
        }
    }
    #region OLD (COMPRENDE CASISTICA "SHINY")
    //private static PlantGenetics.AllelesCouple CanGenerateShiny(PlantGenetics _plant, PlantGenetics _pollen)       //BASE(XX) + IBRIDO(XY)
    //{
    //    if (UnityEngine.Random.Range(1, 101) >= (100 - shiny_probability) && CanPlantsGenerateHybrid(_plant, _pollen))
    //    {
    //        return PlantGenetics.AllelesCouple.RARE;
    //    }

    //    else
    //    {
    //        return GeneralHybridation(_plant.alleles, _pollen.alleles);
    //    }
    //}

    //private static bool CanPlantsGenerateHybrid(PlantGenetics _plant, PlantGenetics _pollen)      //controllo che base e ibrido condividano uno stesso allele (condizione necessaria e sufficiente)
    //{
    //    char to_compare;

    //    /*a seconda di quale delle due piante è la base, ne prendo un allele e lo comparo a entrambi gli alleli dell'ibrido.
    //     * se incontro una corrispondenza allora la condizione è verificata
    //     */
    //    if (_plant.isBase)
    //    {
    //        to_compare = _plant.alleles[0];

    //        for (int i = 0; i < 2; i++)
    //        {
    //            if (_pollen.alleles[i] == to_compare)
    //                return true;
    //        }

    //        return false;
    //    }

    //    else
    //    {
    //        to_compare = _pollen.alleles[0];

    //        for (int i = 0; i < 2; i++)
    //        {
    //            if (_plant.alleles[i] == to_compare)
    //                return true;
    //        }

    //        return false;

    //    }
    //}
    #endregion

    private static PlantGenetics.AllelesCouple GeneralHybridation(char[] a, char[] b)     //IBRIDO + IBRIDO
    {
        int[] alleles_extracted = new int[2] { UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2) };

        string s = new string(a, alleles_extracted[0], 1) + new string(b, alleles_extracted[1], 1);       //estraggo casualmente uno dei due alleli da entrambe le piante

        if (Enum.TryParse(s, false, out PlantGenetics.AllelesCouple _))
        {
            return Enum.Parse<PlantGenetics.AllelesCouple>(s);
        }

        else
        {
            s = new string(b, alleles_extracted[1], 1) + new string(a, alleles_extracted[0], 1);

            return Enum.Parse<PlantGenetics.AllelesCouple>(s);
        }
    }
}
