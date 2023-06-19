using System.IO;
using UnityEditor;
using UnityEngine;

public static class PathUtility
{
    [MenuItem("Tools/Path/Show Persistent")]
    public static void ShowPersistentPath()
    {
        string path = Path.Combine(Application.persistentDataPath);
        System.Diagnostics.Process.Start(path);
    }
}
