using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[DisallowMultipleComponent]
public class BiodiversityManager : MonoBehaviour, IDataPersistance
{
    public static BiodiversityManager Instance { get; private set; }
    private short _max_lvl_progress;
    private short _biodiv_lvl_progress;
    private byte _biodiv_lvl;
    private Dictionary<PlantGenetics.AllelesCouple, byte> plants_in_scene = new();
    internal Action<byte, float, float> OnBiodivLevelUp;
    internal Action<float, float> OnBiodivProgressUpdated;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameObject.Find("Progress Bar").GetComponent<BiodiversityBarScript>().InitializeUI(_biodiv_lvl, _biodiv_lvl_progress, _max_lvl_progress);
    }

    internal void LoadPlantsInScene(PlantGenetics.AllelesCouple chromes)        //carica le piante presenti nel file di salvataggio nel dizionario; eseguito solo dalle piante caricate
    {
        AddPlant(chromes);
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

            _biodiv_lvl_progress -= CalculateBiodivIncrement(new_plant.chromosomes, new_plant.defaultBiodiversityValue);

            if (_biodiv_lvl_progress < 0) _biodiv_lvl_progress = 0;     //non permette la retrocessione di livello (per ora)

            OnBiodivProgressUpdated?.Invoke(_biodiv_lvl_progress, _max_lvl_progress);       //chiamata di aggiornamento UI
        }
    }

    private void CheckForLevelUp(PlantGenetics.AllelesCouple chromes, byte default_amount)      //gestisce i due casi: aumento biodiv, aumento biodiv e di livello
    {
        _biodiv_lvl_progress += CalculateBiodivIncrement(chromes, default_amount);

        if (_biodiv_lvl_progress >= _max_lvl_progress) BiodivLevelUp();     //avvio processo di level up

        else OnBiodivProgressUpdated?.Invoke(_biodiv_lvl_progress, _max_lvl_progress);      //chiamata di aggiornamento UI
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
        _biodiv_lvl_progress -= _max_lvl_progress;      //ricavo la quantità di biodiv sforata dal livello attuale, ottenendo così il valore di progresso relativo al nuovo livello

        _max_lvl_progress = (short)(_max_lvl_progress * 1.3f);      //aumento la soglia di biodiv da superare per salire nuovamente di livello

        _biodiv_lvl += 1;

        OnBiodivLevelUp?.Invoke(_biodiv_lvl, _biodiv_lvl_progress, _max_lvl_progress);      //chiamata di aggiornamento UI
    }

    private void AddPlant(PlantGenetics.AllelesCouple chromes)      //aggiungo la nuova pianta al dizionario di piante presenti in scena
    {
        if (!plants_in_scene.ContainsKey(chromes))
        {
            plants_in_scene.Add(chromes, 1);

            return;
        }

        else
        {
            plants_in_scene[chromes] += 1;
        }
    }

    private void RemovePlant(PlantGenetics.AllelesCouple chromes)       //rimuovoi la pianta dal dizionario di piante presenti in scena
    {
        plants_in_scene[chromes] -= 1;

        if(plants_in_scene[chromes] <= 0)
        {
            plants_in_scene.Remove(chromes);
        }
    }

    public void LoadData()
    {
        _biodiv_lvl = GameData.currentSessionData.biodivLvl;
        _biodiv_lvl_progress = GameData.currentSessionData.biodivLvlProgress;
        _max_lvl_progress = GameData.currentSessionData.maxLvlProgress;
    }

    public void SaveData()
    {
        GameData.currentSessionData.biodivLvl = _biodiv_lvl;
        GameData.currentSessionData.biodivLvlProgress = _biodiv_lvl_progress;
        GameData.currentSessionData.maxLvlProgress = _max_lvl_progress;
    }
}
