using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetsPath
{
    private const string AssetsFolderName = "Paths";
    public static string LocalPath
    {
        get
        {
            var path = FullPath;
            return path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));

        }
    }
    public static string FullPath
    {
        get
        {
            string path = "";
            var directories = Directory.GetDirectories(Application.dataPath, AssetsFolderName, SearchOption.AllDirectories);
            if (directories.Length > 0)
                path = directories[0];
            return path;
        }
    }

    public const string AssetName_Components = "Components";
    public const string AssetName_ComponentsObservers = "Components_Observers";
    public const string AssetName_Entries = "Entries";
    public const string AssetName_TemplateEntries = "Template_Entries";
    public const string AssetName_TemplateEntryObserver = "Template_Entry_observers";
    public const string AssetType = ".asset";

    public static void CreateAsset(UnityEngine.Object obj,string assetName)
    {
        if (obj == null)
            throw new ArgumentNullException();
        if (!FileExists(assetName + AssetType))
            AssetDatabase.CreateAsset(obj, string.Format("{0}/{1}{2}", LocalPath, assetName, AssetType));
        else AssetDatabase.AddObjectToAsset(obj, string.Format("{0}/{1}{2}", LocalPath, assetName, AssetType));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static void DestroyAsset(UnityEngine.Object obj)
    {
        UnityEngine.Object.DestroyImmediate(obj, true);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }



    static bool FileExists(string fileName)
    {
        return FileExists(Application.dataPath, fileName);
    }
    static bool FileExists(string path, string fileName)
    {
        var directories = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
        return (directories.Length > 0);
    }


}
