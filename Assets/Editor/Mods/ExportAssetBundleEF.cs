using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ExportAssetBundleEF
{
    [MenuItem("Assets/CMS21/Mods/Export Asset Bundle")]
    private static void CreateCarBundleExtended()
    {
        try
        {
            var selectionObjects = Selection.objects;
            var pathList = new List<string>();
            for (var i = 0; i < selectionObjects.Length; i++)
            {
                pathList.Clear();

                var file = AssetDatabase.GetAssetPath(selectionObjects[i]);
                var bundleName = file;
                var b = bundleName.Split('/');
                b[b.Length - 1] = b[b.Length - 1].Replace(" ", "_");
                bundleName = b[b.Length - 1];

                // This path is a directory
                pathList.AddRange(ProcessDirectory(file));

                var buildMap = new AssetBundleBuild[1];
                buildMap[0].assetBundleName = $"{bundleName}.assetbundle";
                buildMap[0].assetNames = pathList.ToArray();

                var outputPath = Path.Combine(Application.streamingAssetsPath, $"{bundleName}");

                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);

                if (!Directory.Exists($"{outputPath}/PartThumb"))
                    Directory.CreateDirectory($"{outputPath}/PartThumb");

                try
                {
                    var bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;

                    RemoveOldAssetBundle($"{outputPath}/{bundleName}.assetbundle");
                    BuildPipeline.BuildAssetBundles(outputPath, buildMap, bundleOptions,
                        EditorUserBuildSettings.activeBuildTarget);

                    RemoveOldAssetBundle($"{outputPath}/{bundleName}");
                    RemoveOldAssetBundle($"{outputPath}/{bundleName}.manifest");
                    RemoveOldAssetBundle(
                        $"{outputPath}/{bundleName}.assetbundle.manifest");

                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[ExportAssetBundleEF] -> CreateCarBundleExtended() Failed to create asset bundle. Error: {ex.Message}");
                }
            }

            AssetDatabase.Refresh();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ExportAssetBundleEF] -> CreateCarBundleExtended() {ex.Message}");
        }
    }

    private static void RemoveOldAssetBundle(string assetFilePath)
    {
        if (File.Exists(assetFilePath))
            File.Delete(assetFilePath);
    }

    private static List<string> ProcessDirectory(string targetDirectory)
    {
        try
        {
            var pathList = new List<string>();

            var files = Directory.GetFiles(targetDirectory);
            for (var i = 0; i < files.Length; i++)
                pathList.Add(files[i]);

            var subdirectories = Directory.GetDirectories(targetDirectory);
            for (var i = 0; i < subdirectories.Length; i++)
                pathList.AddRange(ProcessDirectory(subdirectories[i]));

            return pathList;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ExportAssetBundleEF] -> ProcessDirectory() {ex.Message}");
            return null;
        }
    }
}