using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Manager", menuName = "Scriptable Objects/Inventory Manager")]
public class InventoryManagerSO : ScriptableObject, IDataPersistance
{
    public static InventoryManagerSO Singleton { get; private set; }

    internal Dictionary<PlantGenetics.AllelesCouple, short> pollenCollection = new();
    internal Dictionary<PlantGenetics.AllelesCouple, short> fruitsCollection = new();
    internal Action<PlantGenetics.AllelesCouple, short> OnPollenCollectionUpdated;
    internal Action<PlantGenetics.AllelesCouple, short> OnFruitsCollectionUpdated;

    private void OnEnable()
    {
        Singleton = this;
    }

    public void InitializeObj()
    {
        return;
    }

    internal void UpdatePollenCollection(PlantGenetics.AllelesCouple _chromes, sbyte amount)        //chiamato da PlantScript; procedura di aggiornamento inventario
    {

        if (pollenCollection.ContainsKey(_chromes))       //se possiedo già questo tipo di polline, ne incremento la quantità
        {
            pollenCollection[_chromes] += amount;
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-quantità) a pollen collection e ne incremento la quantità
        {
            pollenCollection.Add(_chromes, 1);
        }

        OnPollenCollectionUpdated?.Invoke(_chromes, pollenCollection[_chromes]);      //lancio evento di aggiornamento UI
    }

    internal void UpdateFruitsCollection(PlantGenetics.AllelesCouple _chromes, sbyte amount)        //chiamato da PlantScript; procedura di aggiornamento inventario
    {

        if (fruitsCollection.ContainsKey(_chromes))       //se possiedo già questo tipo di frutto, ne incremento la quantità
        {
            fruitsCollection[_chromes] += amount;
        }

        else        //altrimenti aggiungo la nuova coppia di valori (alleli-quantità) a fruits collection e ne incremento la quantità
        {
            fruitsCollection.Add(_chromes, 1);
        }

        OnFruitsCollectionUpdated?.Invoke(_chromes, fruitsCollection[_chromes]);      //lancio evento di aggiornamento UI
    }

    public void LoadData()
    {
        pollenCollection = GameData.currentSessionData.pollenCollection;

        fruitsCollection = GameData.currentSessionData.fruitsCollection;
    }

    public void SaveData()
    {
        GameData.currentSessionData.pollenCollection = pollenCollection;

        GameData.currentSessionData.fruitsCollection = fruitsCollection;
    }
}
