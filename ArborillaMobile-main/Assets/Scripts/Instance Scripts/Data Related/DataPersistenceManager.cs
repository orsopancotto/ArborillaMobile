using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string file_name;

    public static DataPersistenceManager Singleton { get; private set; }
    private List<IDataPersistance> data_persistance_objects;
    private FIleDataHandler data_handler;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        data_handler = new FIleDataHandler(Application.persistentDataPath, file_name);

        data_persistance_objects = FindDataPersistanceObjects();

        LoadGame();

        SceneLoaderScript.Singleton.OnSceneLoadingProcedure += SaveGame;
    }

    public void NewGame()
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

        foreach (IDataPersistance objects in data_persistance_objects)       //push dei dati caricati agli script che ne necessitano
        {
            objects.LoadData();
        }

    }

    public void SaveGame()
    {
        foreach (IDataPersistance objects in data_persistance_objects)       //push dei dati salvati agli script, che aggiornano i propri
        {
            objects.SaveData();
        }

        data_handler.Save();
    }

    private void OnDestroy()
    {
        SceneLoaderScript.Singleton.OnSceneLoadingProcedure -= SaveGame;
    }


#if UNITY_EDITOR        //OnApplicationQuit non funziona su android, usiamo OnApplicationPause e OnApplicationFocus (metodo non definitivo)
    private void OnApplicationQuit()
    {
        SaveGame();
    }
#else
    private void OnApplicationPause()
    {
        SaveGame();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGame();
        }
    }
#endif


    private List<IDataPersistance> FindDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> objects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

        return new List<IDataPersistance>(objects);
    }

}
