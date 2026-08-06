using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static string Path => Application.persistentDataPath + "/save.json";
    public static bool NeedLoad = false;

    public static bool SaveExists() 
    {
        return File.Exists(Path);
    }
    public static void Save()
    {
        SaveData data = new SaveData();

        

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);
        Debug.Log("Game Saved to " + Path);
    }

    public static void Load()
    {
        if (!SaveExists())
        {
            Debug.LogWarning("Save file not found");
            return;
        }
        string json = File.ReadAllText(Path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        NeedLoad = false;
    }
    public static void DeleteSave()
    {
        if (File.Exists(Path))
        {
            File.Delete(Path);
            Debug.Log("Save deleted");
        }
        else Debug.LogWarning("No save file to delete");
    }
}