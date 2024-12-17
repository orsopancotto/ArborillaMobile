using System.Collections.Generic;


[System.Serializable]
public class GameData
{
    public byte biodivLvl;
    public short biodivLvlProgress, maxLvlProgress;

    /// <summary>
    /// Inventario Frutti: tipo pianta -> q.t�
    /// </summary>
    public Dictionary<PlantGenetics.AllelesCouple, short> fruitsCollection;

    /// <summary>
    /// Inventario polline: tipo pianta -> q.t�
    /// </summary>
    public Dictionary<PlantGenetics.AllelesCouple, short> pollenCollection;

    /// <summary>
    /// Lista di pacchetti di dati che identificano la pianta nell'oasi
    /// </summary>
    public List<PlantDataPacket> oasisPlants;

    /// <summary>
    /// Dizionario delle piante della serra: spot in cui � piantata -> tipo di pianta e stato dei frutti
    /// </summary>
    public Dictionary<string, (PlantGenetics.AllelesCouple Chromosomes, bool HasBeenHarvested)> greenhousePlants;

    public static GameData currentSessionData;
    
    public GameData()       //costruttore; i valori assegnati qui dentro sono i valori default in caso di nuova partita
    {
        biodivLvl = 0;

        biodivLvlProgress = 0;

        maxLvlProgress = 40;

        fruitsCollection = new()
        {
            { PlantGenetics.AllelesCouple.AA, 1 },
            { PlantGenetics.AllelesCouple.BB, 1 },
            { PlantGenetics.AllelesCouple.CC, 1 }
        };

        pollenCollection = new();

        oasisPlants = new();

        greenhousePlants = new()
        {
            {"Spot (3)", (PlantGenetics.AllelesCouple.AA, false) },
            {"Spot (4)", (PlantGenetics.AllelesCouple.BB, false) },
            {"Spot (5)", (PlantGenetics.AllelesCouple.CC, false) }
        };
    }

    public override string ToString()
    {
        return $"biodivLvl: {biodivLvl}\n*****" +
            $"\nbiodivLvlProgress: {biodivLvlProgress}\n*****" +
            $"\nmaxLvlProgress: {maxLvlProgress}\n*****" +
            $"\noasisPlants:\n{/*ListToString(oasisPlants)*/oasisPlants.Count}\n*****" +
            $"\npollenCollection:\n{/*DictionaryToString(pollenCollection)*/pollenCollection.Count}\n*****" +
            $"\nfruitsCollection:\n{/*DictionaryToString(fruitsCollection)*/fruitsCollection.Count}\n*****" +
            $"\ngreenhousePlants: {greenhousePlants.Count}*****";
    }

    private string DictionaryToString<TK, TV>(Dictionary<TK, TV> map)
    {
        string s = "";

        foreach(var v in map)
        {
            s += $"{v.Key}; {v.Value}\n";
        }

        return s;
    }

    private string ListToString<T>(List<T> list)
    {
        string s = "";

        foreach(var obj in list)
        {
            s += $"{obj}\n";
        }

        return s;
    }
}
