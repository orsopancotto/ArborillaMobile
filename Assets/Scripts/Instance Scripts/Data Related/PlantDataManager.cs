using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlantDataManager : MonoBehaviour, IDataPersistance
{
    public static PlantDataManager Instance { get; private set; }
    [HideInInspector] internal Dictionary<string, (PlantGenetics.AllelesCouple Chromosomes, PlantScript.LifeStage LifeStage, PlantGenetics.AllelesCouple SonChromes, short Timer, string DateTime)> _plants_saved = new();
    internal event EventHandler OnSaveDataCall;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadData()        //procedura di caricamento dati del dizionario salvato delle piante
    {
        SpawnManagerScript spawn_manager = SpawnManagerScript.Instance;

        /* chiamo il metodo spawn di spawn manager per ogni pianta salvata, specificandone i parametri necessari: 
         - cromosomi
         - fase del ciclo vitale della pianta
         - cromosomi del figlio
         - tempo rimasto alla scadenza della fase del ciclo vitale (al momento di chiusura app)
         - data e ora al momento di chiusura app
        */
        foreach (KeyValuePair<string, (PlantGenetics.AllelesCouple Chromosomes, PlantScript.LifeStage LifeStage, PlantGenetics.AllelesCouple SonChromes, short Timer, string DateTime)> pairs in GameData.currentSessionData.oasisPlants)      
        {
            try
            {
                spawn_manager.SpawnPlant(GameObject.Find(pairs.Key).GetComponent<Transform>(), pairs.Value.Chromosomes, pairs.Value.LifeStage, pairs.Value.SonChromes,
                    CalculateStartingTimer(pairs.Value.Timer, (int)(DateTime.Now - DateTime.Parse(pairs.Value.DateTime)).TotalSeconds));
            }
            catch(Exception e)
            {
                Debug.LogError($"{pairs.Key}; {pairs.Value}. \n\nError: {e}");
            }
        }
    }

    public void SaveData()
    {
        GameData.currentSessionData.oasisPlants.Clear();     //formatto il dizionario, così da non sovrapporre i dati nuovi

        OnSaveDataCall?.Invoke(this, EventArgs.Empty);      //lancio evento di chiusura app; a cui cono iscritte tutte le piante presenti in scena

        GameData.currentSessionData.oasisPlants = _plants_saved;
    }

    private short CalculateStartingTimer(int x, int y)      //calcolo del progresso del timer, tenendo in considerazione dei limiti costanti del tipo short
    {
        if (x - y <= short.MinValue) return 0;

        else return (short)(x - y);
    }
}
