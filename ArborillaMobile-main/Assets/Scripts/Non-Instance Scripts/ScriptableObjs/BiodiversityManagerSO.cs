using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Biodiversity Manager", menuName = "Scriptable Objects/Biodiversity Manager")]
public class BiodiversityManagerSO : ScriptableObject, IDataPersistance
{
    public static BiodiversityManagerSO Singleton { get; private set; }

    private short max_lvl_progress;
    private short biodiv_lvl_progress;
    private byte biodiv_lvl;
    private Dictionary<PlantGenetics.AllelesCouple, byte> plants_in_scene = new();

    internal Action<byte, float, float> OnBiodivLevelUp;
    internal Action<float, float> OnBiodivProgressUpdated;

    private void OnEnable()
    {
        Singleton = this;
    }

    public void InitializeObj()
    {
        GameManager.Singleton.OnGameStarted += SyncPlantsMap;
    }

    private void SyncPlantsMap()        //sincronizza il dizionario contatore con le piante che sono già presenti all'avvio
    {
        foreach(var obj in GameData.currentSessionData.oasisPlants)
        {
            if (plants_in_scene.ContainsKey(obj.chromosomes)) plants_in_scene[obj.chromosomes]++;

            else plants_in_scene.Add(obj.chromosomes, 1);
        }
    }

    internal void UpdateBiodivLevelProgress(PlantGenetics new_plant, bool hasIncreased)     //aggiorna il progresso in biodiversità; chiamato dalla pianta in distruzione e istanza
    {
        if (hasIncreased)
        {
            CheckForLevelUp(new_plant.chromosomes, new_plant.defaultBiodiversityValue);

            AddPlant(new_plant.chromosomes);
        }

        else
        {
            RemovePlant(new_plant.chromosomes);

            biodiv_lvl_progress -= CalculateBiodivIncrement(new_plant.chromosomes, new_plant.defaultBiodiversityValue);

            if (biodiv_lvl_progress < 0) biodiv_lvl_progress = 0;     //non permette la retrocessione di livello 

            OnBiodivProgressUpdated?.Invoke(biodiv_lvl_progress, max_lvl_progress);       //chiamata di aggiornamento UI
        }
    }

    private void CheckForLevelUp(PlantGenetics.AllelesCouple chromes, byte default_amount)      //gestisce i due casi: aumento biodiv, aumento biodiv e di livello
    {
        biodiv_lvl_progress += CalculateBiodivIncrement(chromes, default_amount);

        if (biodiv_lvl_progress >= max_lvl_progress) BiodivLevelUp();     //avvio processo di level up

        else OnBiodivProgressUpdated?.Invoke(biodiv_lvl_progress, max_lvl_progress);      //chiamata di aggiornamento UI
    }

    private short CalculateBiodivIncrement(PlantGenetics.AllelesCouple chromes, short default_amount)       //funzione di aumento biodiv; varia in base al numero di piante dello stesso tipo presenti in scena
    {
        if (plants_in_scene.ContainsKey(chromes))
        {
            return (short)(default_amount * MathF.Pow(.4f, plants_in_scene[chromes]));
        }

        else
        {
            return default_amount;
        }
    }

    private void BiodivLevelUp()        //procedure di level up
    {
        biodiv_lvl_progress -= max_lvl_progress;      //ricavo la quantità di biodiv sforata dal livello attuale, ottenendo così il valore di progresso relativo al nuovo livello

        max_lvl_progress = (short)(max_lvl_progress * 1.3f);      //aumento la soglia di biodiv da superare per salire nuovamente di livello

        biodiv_lvl++;

        OnBiodivLevelUp?.Invoke(biodiv_lvl, biodiv_lvl_progress, max_lvl_progress);      //chiamata di aggiornamento UI
    }

    private void AddPlant(PlantGenetics.AllelesCouple chromes)      //aggiungo la nuova pianta al dizionario di piante presenti in scena
    {
        if (!plants_in_scene.ContainsKey(chromes))
        {
            plants_in_scene.Add(chromes, 1);

            return;
        }

        plants_in_scene[chromes]++;
    }

    private void RemovePlant(PlantGenetics.AllelesCouple chromes)       //rimuovo la pianta dal dizionario di piante presenti in scena
    {
        plants_in_scene[chromes]--;

        if (plants_in_scene[chromes] <= 0)
        {
            plants_in_scene.Remove(chromes);
        }
    }

    public void LoadData()
    {
        biodiv_lvl = GameData.currentSessionData.biodivLvl;
        biodiv_lvl_progress = GameData.currentSessionData.biodivLvlProgress;
        max_lvl_progress = GameData.currentSessionData.maxLvlProgress;
    }

    public void SaveData()
    {
        GameData.currentSessionData.biodivLvl = biodiv_lvl;
        GameData.currentSessionData.biodivLvlProgress = biodiv_lvl_progress;
        GameData.currentSessionData.maxLvlProgress = max_lvl_progress;
    }
}
