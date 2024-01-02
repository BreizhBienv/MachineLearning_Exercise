using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializeData
{
    static public void SaveGame(NeuralNetwork ToSave, int Gen, int HighS)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        if (!Directory.Exists(Directory.GetCurrentDirectory() + "/SavedGameData/"))
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/SavedGameData/");


        if (File.Exists(Directory.GetCurrentDirectory() + "/SavedGameData/AI.dat"))
            file = File.Open(Directory.GetCurrentDirectory() + "/SavedGameData/AI.dat", FileMode.Open);
        else
            file = File.Create(Directory.GetCurrentDirectory() + "/SavedGameData/AI.dat");

        SaveData saveData = new SaveData(ToSave, Gen, HighS);
        bf.Serialize(file, saveData);
        file.Close();

        Debug.Log("AI Saved");
    }

    static public void LoadData(out BirdIndividual ToLoad, out int Gen, out int HighS)
    {
        ToLoad = null;
        Gen = -1;
        HighS = 0;

        if (File.Exists(Directory.GetCurrentDirectory() + "/SavedGameData/AI.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Directory.GetCurrentDirectory() + "/SavedGameData/AI.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);

            ToLoad = new BirdIndividual(
                data.TopHeightW, data.BotHeightW,
                data.LastDistW, data.NextDistW);

            Gen = data.Generation;

            HighS = data.HighScore;

            file.Close();

            Debug.Log("AI Loaded");
            return;
        }

        Debug.Log("AI couldn't be loaded");
    }
}

[Serializable]
public class SaveData
{
    public float TopHeightW;
    public float BotHeightW;

    public float LastDistW;
    public float NextDistW;

    public int Generation;

    public int HighScore;

    public SaveData(NeuralNetwork ToSave, int Gen, int HighS)
    {
        //TopHeightW = ToSave.TopHeightW;
        //BotHeightW = ToSave.BotHeightW;
        //LastDistW = ToSave.LastDistW;
        //NextDistW = ToSave.NextDistW;

        Generation = Gen;

        HighScore = HighS;
    }
}
