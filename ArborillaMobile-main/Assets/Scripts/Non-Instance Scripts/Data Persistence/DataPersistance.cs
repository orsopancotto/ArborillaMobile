using System.Collections.Generic;
using UnityEngine;

public class DataPersistance
{
    public IEnumerable<IDataPersistance> dataPersistanceObjects { get; private set; }
    private readonly FIleDataHandler data_handler;

    public DataPersistance(string file_name, IEnumerable<IDataPersistance> data_persistance_objects)
    {
        dataPersistanceObjects = data_persistance_objects;
        data_handler = new FIleDataHandler(Application.persistentDataPath, file_name);
    }

    private void NewGame()
    {
        GameData.currentSessionData = new GameData();
    }

    public void LoadGame()
    {
        GameData.currentSessionData = data_handler.Load();

        if (GameData.currentSessionData == null)
        {
            NewGame();
        }

        foreach (var obj in dataPersistanceObjects)
        {
            obj.InitializeObj();
        }
    }

    public void SaveGame()
    {
        foreach (var objects in dataPersistanceObjects)
        {
            objects.SaveData();
        }

        data_handler.Save();
    }
}
