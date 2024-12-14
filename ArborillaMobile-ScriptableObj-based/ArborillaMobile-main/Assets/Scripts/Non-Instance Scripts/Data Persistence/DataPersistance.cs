using System.Collections.Generic;
using UnityEngine;

public class DataPersistance
{
    public IEnumerable<IDataPersistance> dataPersistanceObjects { get; private set; }

    private readonly string file_name;
    private readonly FIleDataHandler data_handler;

    public DataPersistance(string file_name, IEnumerable<IDataPersistance> data_persistance_objects)
    {
        this.file_name = file_name;
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
            Debug.Log("No data found, loading new game");

            NewGame();
        }

        foreach(var obj in dataPersistanceObjects)
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
