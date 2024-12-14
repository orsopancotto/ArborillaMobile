using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance
{
    /// <summary>
    /// Chiamato una sola volta durante il ciclo dell'app, subito dopo la lettura dei dati di salvataggio (<see cref="DataPersistance.LoadGame()"/>)
    /// </summary>
    void InitializeObj();

    /// <summary>
    /// Chiamato una sola volta durante il ciclo dell'app, subito dopo <see cref="InitializeObj()"/>
    /// </summary>
    void LoadData();

    void SaveData();  
}
