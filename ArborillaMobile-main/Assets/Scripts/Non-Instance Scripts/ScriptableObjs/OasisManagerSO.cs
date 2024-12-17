using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Oasis Manager", menuName = "Scriptable Objects/Oasis Manager")]
public class OasisManagerSO : ScriptableObject, IDataPersistance
{
    public static OasisManagerSO Singleton { get; private set; }

    internal List<PlantDataPacket> oasis_plants;
    internal event EventHandler OnSaveDataCall;

    private void OnEnable()
    {
        Singleton = this;
    }

    public void InitializeObj()
    {
        GameManager.Singleton.OnSceneLoaded += GeneratePlants;
    }

    public void LoadData()
    {
        oasis_plants = GameData.currentSessionData.oasisPlants;
    }

    public void SaveData()
    {
        OnSaveDataCall?.Invoke(this, EventArgs.Empty);      //lancio evento di chiusura app; a cui cono iscritte tutte le piante presenti in scena

        GameData.currentSessionData.oasisPlants = oasis_plants;
    }

    private void GeneratePlants(int scene_index)
    {
        if (scene_index != 1) return;

        /* chiamo il metodo spawn di spawn manager per ogni pianta salvata, specificandone i parametri necessari: 
         - cromosomi
         - fase del ciclo vitale della pianta
         - cromosomi del figlio
         - tempo rimasto alla scadenza della fase del ciclo vitale al momento di salvataggio della pianta
         - data e ora al momento di salvataggio della pianta
        */

        foreach (var plant in oasis_plants)
        {
            try
            {
                SpawnManagerScript.Singleton.SpawnPlant(
                    GameObject.Find(plant.spotName).GetComponent<Transform>(),
                    plant.chromosomes,
                    plant.lifeStage,
                    plant.sonChromosomes,
                    CalculateStartingTimer(plant.stageTime, (int)(DateTime.Now - DateTime.Parse(plant.time)).TotalSeconds)
                    );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
            }
        }
    }

    private short CalculateStartingTimer(int x, int y)      //calcolo del progresso del timer, tenendo in considerazione dei limiti costanti del tipo short
    {
        if (x - y <= short.MinValue) return 0;

        else return (short)(x - y);
    }

    internal void UpdatePlantData(string spot_name, short stage_time)
    {
        PlantDataPacket data = oasis_plants.Where(p => p.spotName == spot_name).FirstOrDefault();

        data.stageTime = stage_time;
    }

    internal void UpdatePlantData(string spot_name, PlantGenetics.AllelesCouple son_chromes)
    {
        PlantDataPacket data = oasis_plants.Where(p => p.spotName == spot_name).FirstOrDefault();

        data.sonChromosomes = son_chromes;
    }

    internal void UpdatePlantData(string spot_name, PlantScript.LifeStage life_stage)
    {
        PlantDataPacket data = oasis_plants.Where(p => p.spotName == spot_name).FirstOrDefault();

        data.lifeStage = life_stage;
    }

    internal void EreasePlantData(string spot_name)
    {
        oasis_plants.RemoveAll(p => p.spotName == spot_name);
    }

}
