using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class InventoryManagerScript : MonoBehaviour, IDataPersistance
{
    public static InventoryManagerScript Singleton { get; private set; }

    internal Dictionary<PlantGenetics.AllelesCouple, short> pollenCollection = new();

    internal Dictionary<PlantGenetics.AllelesCouple, short> fruitsCollection = new();

    internal Action<PlantGenetics.AllelesCouple, short> OnPollenCollectionUpdated;

    internal Action<PlantGenetics.AllelesCouple, short> OnFruitsCollectionUpdated;

    private void Awake()
    {
        Singleton = this;
    }

    internal void UpdatePollenCollection(PlantGenetics.AllelesCouple _chromes, sbyte amount)        //chiamato da PlantScript; procedura di aggiornamento inventario
    {

        if (pollenCollection.ContainsKey(_chromes))       //se possiedo gi� questo tipo di polline, ne incremento la quantit�
        {
            pollenCollection[_chromes] += amount;
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-quantit�) a pollen collection e ne incremento la quantit�
        {
            pollenCollection.Add(_chromes, 1);
        }

        OnPollenCollectionUpdated?.Invoke(_chromes, pollenCollection[_chromes]);      //lancio evento di aggiornamento UI
    }

    internal void UpdateFruitsCollection(PlantGenetics.AllelesCouple _chromes, sbyte amount)        //chiamato da PlantScript; procedura di aggiornamento inventario
    {

        if (fruitsCollection.ContainsKey(_chromes))       //se possiedo gi� questo tipo di frutto, ne incremento la quantit�
        {
            fruitsCollection[_chromes] += amount;
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-quantit�) a fruits collection e ne incremento la quantit�
        {
            fruitsCollection.Add(_chromes, 1);
        }

        OnFruitsCollectionUpdated?.Invoke(_chromes, fruitsCollection[_chromes]);      //lancio evento di aggiornamento UI
    }

    public void LoadData()
    {
        pollenCollection = GameData.currentSessionData.pollenCollection;

        fruitsCollection = GameData.currentSessionData.fruitsCollection;

        DataLoaded();       //brutto ma funzia
    }

    internal async Task DataLoaded()
    {
        await Task.Yield();
    }

    public void SaveData()
    {
        GameData.currentSessionData.pollenCollection = pollenCollection;

        GameData.currentSessionData.fruitsCollection = fruitsCollection;
    }
}
