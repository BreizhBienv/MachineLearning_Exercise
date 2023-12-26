using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializeData
{
    static public void SaveGame(BirdIndividual ToSave, int Gen, int HighS)
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
                data.TopPipeWheight, data.BottomPipeWheight,
                new Vector2(data.RayWeightX, data.RayWeightY),
                data.Bias);

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
    public float TopPipeWheight;
    public float BottomPipeWheight;
    
    public float RayWeightX;
    public float RayWeightY;

    public float Bias;

    public int Generation;

    public int HighScore;

    public SaveData(BirdIndividual ToSave, int Gen, int HighS)
    {
        TopPipeWheight = ToSave.TopPipeWheight;
        BottomPipeWheight = ToSave.BottomPipeWheight;

        RayWeightX = ToSave.RayWeight.x;
        RayWeightY = ToSave.RayWeight.y;
        Bias = ToSave.Bias;

        Generation = Gen;

        HighScore = HighS;
    }
}
