using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public byte biodivLvl;
    public short biodivLvlProgress, maxLvlProgress;

    /// <summary>
    /// Inventario Frutti: tipo pianta -> q.tà
    /// </summary>
    public Dictionary<PlantGenetics.AllelesCouple, short> fruitsCollection;

    /// <summary>
    /// Inventario polline: tipo pianta -> q.tà
    /// </summary>
    public Dictionary<PlantGenetics.AllelesCouple, short> pollenCollection;

    /// <summary>
    /// Dizionario delle piante salvate nell'oasi: spot in cui è posizionata la pianta -> dati identificativi della pianta e della fase di vita
    /// </summary>
    public Dictionary<string/*nome spot in cui la pianta è posizionata*/, (PlantGenetics.AllelesCouple Chromosomes, PlantScript.LifeStage LifeStage, PlantGenetics.AllelesCouple SonChromes, short Timer, string DateTime)> oasisPlants;

    /// <summary>
    /// Dizionario delle piante della serra: spot in cui è piantata -> tipo di pianta e stato dei frutti
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
}
