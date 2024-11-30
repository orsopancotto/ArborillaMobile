using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class InventoryManagerScript : MonoBehaviour, IDataPersistance
{
    public static InventoryManagerScript Instance { get; private set; }

    internal Dictionary<PlantGenetics.AllelesCouple, short> _pollen_collection = new();

    internal Dictionary<PlantGenetics.AllelesCouple, short> _fruits_collection = new();

    internal Action<PlantGenetics.AllelesCouple, short> OnPollenCollectionUpdated;

    internal Action<PlantGenetics.AllelesCouple, short> OnFruitsCollectionUpdated;

    private void Awake()
    {
        Instance = this;
    }

    internal void UpdatePollenCollection(PlantGenetics.AllelesCouple _chromes, sbyte amount)        //chiamato da PlantScript; procedura di aggiornamento inventario
    {

        if (_pollen_collection.ContainsKey(_chromes))       //se possiedo già questo tipo di polline, ne incremento la quantità
        {
            _pollen_collection[_chromes] += amount;
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-quantità) a pollen collection e ne incremento la quantità
        {
            _pollen_collection.Add(_chromes, 1);
        }

        OnPollenCollectionUpdated?.Invoke(_chromes, _pollen_collection[_chromes]);      //lancio evento di aggiornamento UI
    }

    internal void UpdateFruitsCollection(PlantGenetics.AllelesCouple _chromes, sbyte amount)        //chiamato da PlantScript; procedura di aggiornamento inventario
    {

        if (_fruits_collection.ContainsKey(_chromes))       //se possiedo già questo tipo di frutto, ne incremento la quantità
        {
            _fruits_collection[_chromes] += amount;
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-quantità) a fruits collection e ne incremento la quantità
        {
            _fruits_collection.Add(_chromes, 1);
        }

        OnFruitsCollectionUpdated?.Invoke(_chromes, _fruits_collection[_chromes]);      //lancio evento di aggiornamento UI
    }

    public void LoadData()
    {
        _pollen_collection = GameData.currentSessionData.pollenCollection;

        _fruits_collection = GameData.currentSessionData.fruitsCollection;

        DataLoaded();       //brutto ma funzia
    }

    internal async Task DataLoaded()
    {
        await Task.Yield();
    }

    public void SaveData()
    {
        GameData.currentSessionData.pollenCollection = _pollen_collection;

        GameData.currentSessionData.fruitsCollection = _fruits_collection;
    }
}
