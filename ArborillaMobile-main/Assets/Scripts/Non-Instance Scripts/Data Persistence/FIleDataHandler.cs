using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine;

public class FIleDataHandler
{
    private readonly string data_dir_path = " ";
    private readonly string data_file_name = " ";

    public FIleDataHandler(string _data_dir_path, string _data_file_name)
    {
        data_dir_path = _data_dir_path;
        data_file_name = _data_file_name;
    }

    public GameData Load()
    {
        string full_path = Path.Combine(data_dir_path, data_file_name);     //usiamo combine perchè riesce a gestire i diversi separatori di percorso tra i diversi OS

        var data = new GameData();

        if (File.Exists(full_path))
        {
            try
            {
                string data_to_load = "";

                using (FileStream stream = new FileStream(full_path, FileMode.Open))        //scrittura dati sul file (non ho idea di cosa ci sia scritto :) )
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data_to_load = reader.ReadToEnd();
                    }
                }

                data = JsonConvert.DeserializeObject<GameData>(data_to_load);     //deserializzazione dei dati da json a oggetto c#
            }

            catch (Exception e)
            {
                Debug.LogError($"Error when trying to load data to file: {full_path}\n{e}");
            }

        }

        return data;
    }

    public void Save()
    {
        string full_path = Path.Combine(data_dir_path, data_file_name);     //usiamo combine perchè riesce a gestire i diversi separatori di percorso tra i diversi OS

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(full_path));

            string data_to_store = JsonConvert.SerializeObject(GameData.currentSessionData, Formatting.Indented);     //serializza i dati c# in json

            using (FileStream stream = new FileStream(full_path, FileMode.Create))      //scrittura dati sul file (non ho idea di cosa ci sia scritto :) )
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data_to_store);
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError($"Error when trying to save data to file: {full_path}\n{e}");
        }
    }
}
