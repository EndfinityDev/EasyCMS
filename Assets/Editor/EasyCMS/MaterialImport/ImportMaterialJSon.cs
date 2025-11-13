#if UNITY_EDITOR
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// This file is brought to you by sleep deprivation

public class ImportMaterialJSon
{
    const string GENERAL_SHADER_NAME_UV0 = "Shader Graphs/EasyCMS_Default_UV0";
    const string GENERAL_SHADER_NAME_UV1 = "Shader Graphs/EasyCMS_Default_UV1";

    const string GLASS_SHADER_NAME_UV1 = "Shader Graphs/EasyCMS_Glass_UV1";
    const string GLASS_SHADER_NAME_UV0 = "Shader Graphs/EasyCMS_Glass_UV0";

    const string OPAQUE_SHADER_NAME_UV0 = "Shader Graphs/EasyCMS_Opaque_UV0";
    const string OPAQUE_SHADER_NAME_UV1 = "Shader Graphs/EasyCMS_Opaque_UV1";

    const string PAINT_STAMP_SHADER_NAME_UV0 = "Shader Graphs/EasyCMS_Paint_Stamp_UV0";
    const string PAINT_STAMP_SHADER_NAME_UV1 = "Shader Graphs/EasyCMS_Paint_Stamp_UV1";

    const string PAINT_OPAQUE_SHADER_NAME_UV0 = "Shader Graphs/EasyCMS_Paint_Opaque_UV0";
    const string PAINT_OPAQUE_SHADER_NAME_UV1 = "Shader Graphs/EasyCMS_Paint_Opaque_UV1";

    enum UVMap
    {
        UV0,
        UV1
    }

    static T GetValue<T>(in JObject dict, in string key, T defaultValue)
    {
        JToken token;
        if(dict.TryGetValue(key, System.StringComparison.OrdinalIgnoreCase, out token))
        {
            return token.Value<T>();
        }
        else
        {
            return defaultValue;
        }
    }

    [MenuItem("Assets/EasyCMS/Materials/Import Materials File (HDRP)", priority = 100)]
    static void ImportSelectedMaterialFiles()
    {
        Object[] selectionObjects = Selection.objects;
        if (selectionObjects.Length <= 0 )
        {
            Debug.LogError("[ImportMaterialJSon.ImportSelectedMaterialFiles] | No files were provided!");
        }
        HashSet<Object> ignoreObjects = new HashSet<Object>();
        foreach (Object selectedObject in selectionObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            if (assetPath.EndsWith(".json"))
            {
                ParseMaterialsFile(assetPath);
            }
            else
            {
                Debug.LogError($"[ImportMaterialJSon.ImportSelectedMaterialFiles] | {Path.GetFileName(assetPath)} is not a json file!");
                continue;
            }
        }
    }


    static void ParseMaterial(in KeyValuePair<string, JToken> material, in Dictionary<string, CarPaintSO> paintData, 
        in string directoryName, in string texturesDirectory, in string materialsDirectory)
    {
        JObject materialData = material.Value as JObject;

        if (!materialData.ContainsKey("Stages"))
        {
            Debug.LogWarning($"[ImportMaterialJSon.ParseMaterialsFile] | {material.Key}: no Stages array found");
            return;
        }
        JArray stages = materialData.GetValue("Stages") as JArray;
        JObject stageData = stages[0] as JObject;

        bool isTranslucent = GetValue(materialData, "translucent", false);

        string normalMapName = GetValue(stageData, "normalMap", string.Empty);
        if (normalMapName.StartsWith("camso_col"))
        {
            normalMapName = string.Empty;
        }

        // We will overwrite translucency for materials that should work better without it regardless of how they were exported
        if (material.Key.Contains("fixturebulb_light"))
        {
            isTranslucent = false;
        }

        string opacityMapName = GetValue(stageData, "opacityMap", string.Empty);

        UVMap uvMap = UVMap.UV0;
        int opacityMapUseUV = GetValue(stageData, "opacityMapUseUV", 0);
        if (stageData.ContainsKey("opacityMapUseUV"))
        {
            switch (opacityMapUseUV)
            {
                case 0:
                    uvMap = UVMap.UV0;
                    break;
                case 1:
                    uvMap = UVMap.UV1;
                    break;
                default:
                    Debug.LogWarning($"[ImportMaterialJSon.ParseMaterialsFile] | \"{material.Key}\": UV Map is not 0 or 1, using default (UV0)");
                    break;
            }
        }

        string shaderName = "";
        //string shaderName = isTranslucent ? GLASS_SHADER_NAME : GENERAL_SHADER_NAME;
        if (isTranslucent)
        {
            switch (uvMap)
            {
                case UVMap.UV0:
                    shaderName = GLASS_SHADER_NAME_UV0;
                    break;
                case UVMap.UV1:
                    shaderName = GLASS_SHADER_NAME_UV1;
                    break;
                default:
                    Debug.LogError($"[ImportMaterialJSon.ParseMaterialsFile] | \"{material.Key}\": Unknown UV Map enum, using default (UV0)");
                    shaderName = GLASS_SHADER_NAME_UV0;
                    break;
            }
        }
        else
        {
            bool hasOpacityMap = opacityMapName != string.Empty;
            bool isPaint = material.Key.Contains("_carpaint");
            if (isPaint)
            {
                switch (uvMap)
                {
                    case UVMap.UV0:
                        shaderName = hasOpacityMap ? PAINT_STAMP_SHADER_NAME_UV0 : PAINT_OPAQUE_SHADER_NAME_UV0;
                        break;
                    case UVMap.UV1:
                        shaderName = hasOpacityMap ? PAINT_STAMP_SHADER_NAME_UV1 : PAINT_OPAQUE_SHADER_NAME_UV1;
                        break;
                    default:
                        Debug.LogError($"[ImportMaterialJSon.ParseMaterialsFile] | \"{material.Key}\": Unknown UV Map enum, using default (UV0)");
                        shaderName = hasOpacityMap ? PAINT_STAMP_SHADER_NAME_UV0 : PAINT_OPAQUE_SHADER_NAME_UV0;
                        break;
                }
            }
            else
            {
                switch (uvMap)
                {
                    case UVMap.UV0:
                        shaderName = hasOpacityMap ? GENERAL_SHADER_NAME_UV0 : OPAQUE_SHADER_NAME_UV0;
                        break;
                    case UVMap.UV1:
                        shaderName = hasOpacityMap ? GENERAL_SHADER_NAME_UV1 : OPAQUE_SHADER_NAME_UV1;
                        break;
                    default:
                        Debug.LogError($"[ImportMaterialJSon.ParseMaterialsFile] | \"{material.Key}\": Unknown UV Map enum, using default (UV0)");
                        shaderName = hasOpacityMap ? GENERAL_SHADER_NAME_UV0 : OPAQUE_SHADER_NAME_UV0;
                        break;
                }
            }
        }


        Material newMaterial = new Material(Shader.Find(shaderName));
        newMaterial.name = material.Key;
        EditorUtility.SetDirty(newMaterial);

        if (normalMapName != string.Empty)
        {
            string normalMapPath = $"{texturesDirectory}/{normalMapName}";
            Texture2D normalMapTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalMapPath);
            newMaterial.SetTexture("_NormalMap", normalMapTex);
        }

        //_NormalMapStrength
        float normalMapStrength = GetValue(stageData, "normalMapStrength", 1.0f);
        if (normalMapName == string.Empty)
        {
            normalMapStrength = 0.0f;
        }
        newMaterial.SetFloat("_NormalMapStrength", normalMapStrength);

        List<float> colorChannels = new List<float>();
        if (stageData.ContainsKey("diffuseColor"))
        {
            foreach (float colorChannel in stageData.GetValue("diffuseColor").Values<float>())
            {
                colorChannels.Add(colorChannel);
            }
        }
        else
        {
            colorChannels.Add(1.0f);
            colorChannels.Add(1.0f);
            colorChannels.Add(1.0f);
        }
        Color color = new Color(colorChannels[0], colorChannels[1], colorChannels[2]);

        string colorPaletteMap = GetValue(stageData, "colorPaletteMap", string.Empty);
        bool isBodyPaint = GetValue(stageData, "instanceDiffuse", false) &&
            colorPaletteMap.Length > 0;

        float metallic = GetValue(stageData, "metallicFactor", 0.0f);
        if (isBodyPaint)
        {
            metallic = 0.85f;
        }
        newMaterial.SetFloat("_Metallic", metallic);

        float roughness = GetValue(stageData, "roughnessFactor", 0.5f);
        if (isBodyPaint)
        {
            roughness = 0.35f;
        }

        float clearCoat = 0.0f;

        if (isTranslucent && stageData.ContainsKey("opacityFactor"))
        {
            float opacityFactor = GetValue(stageData, "opacityFactor", 1.0f);
            opacityFactor *= opacityFactor;
            newMaterial.SetFloat("_OpacityFactor", opacityFactor);
            roughness *= 1.5f;
            Mathf.Clamp(roughness, 0.0f, 1.0f);
        }
        else
        {
            clearCoat = GetValue(stageData, "clearCoatFactor", 0.0f);
            newMaterial.SetFloat("_ClearCoat", clearCoat);

            string diffuseMapName = GetValue(stageData, "diffuseMap", string.Empty);
            if (diffuseMapName.Length > 0)
            {
                string diffuseMapPath = $"{texturesDirectory}/{diffuseMapName}";
                Texture2D diffuseMapTex = AssetDatabase.LoadAssetAtPath<Texture2D>(diffuseMapPath);
                newMaterial.SetTexture("_DiffuseTex", diffuseMapTex);
            }
        }
        newMaterial.SetFloat("_Roughness", roughness);

        if (isBodyPaint)
        {
            if (color == Color.white)
            {
                color = new Color(1.0f, 0.2f, 0.2f);
            }
            if (!paintData.ContainsKey(colorPaletteMap))
            {
                CarPaintSO newPaintData = ScriptableObject.CreateInstance<CarPaintSO>();
                newPaintData.name = directoryName + colorPaletteMap;
                newPaintData.CarColor = color;
                newPaintData.Roughness = roughness;
                newPaintData.Metallic = metallic;
                newPaintData.Clearcoat = clearCoat;
                paintData[colorPaletteMap] = newPaintData;
            }
            CarPaintSO carPaintData = paintData[colorPaletteMap];
            carPaintData.TargetMaterials.Add(newMaterial);
        }
        newMaterial.SetColor("_Color", color);

        if (opacityMapName.Length > 0)
        {
            string opacityMapPath = $"{texturesDirectory}/{opacityMapName}";
            Texture2D opacityMapTex = AssetDatabase.LoadAssetAtPath<Texture2D>(opacityMapPath);
            newMaterial.SetTexture("_MainTex", opacityMapTex);
        }

        AssetDatabase.CreateAsset(newMaterial, $"{materialsDirectory}/{material.Key}.mat");
    }

    public static HashSet<Object> ParseMaterialsFile(in string filePath)
    {
        HashSet<Object> ignoreObjects = new HashSet<Object>();
        string[] splitPath = filePath.Split('/');
        string directory = "";
        for (int i = 0; i < splitPath.Length - 1; i++)
        {
            directory += splitPath[i] + '/';
        }
        string directoryName = splitPath[splitPath.Length - 2];
        string texturesDirectory = directory + "Textures";
        string materialsDirectory = directory + "Materials";
        if (!Directory.Exists(materialsDirectory))
        {
            Directory.CreateDirectory(materialsDirectory);
        }

        Dictionary<string, CarPaintSO> paintData = new Dictionary<string, CarPaintSO>();

        TextAsset materialsFile = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);

        JObject materials = JsonConvert.DeserializeObject(materialsFile.text) as JObject;

        foreach (KeyValuePair<string, JToken> material in materials)
        {
            ParseMaterial(material, paintData, directoryName, texturesDirectory, materialsDirectory);
        }

        int paintDataIdx = 0;
        foreach(KeyValuePair<string, CarPaintSO> carPaintData in paintData)
        {
            paintDataIdx++;
            string paintDataName = $"{directory}Paint_{paintDataIdx}.asset";
            AssetDatabase.DeleteAsset(paintDataName);
            AssetDatabase.CreateAsset(carPaintData.Value, paintDataName);
            ignoreObjects.Add(carPaintData.Value);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return ignoreObjects;
    }

    public static void CopyTextures(in string absMaterialFolder, in string matFilePath)
    {
        string absMatFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), matFilePath);
        string absTextureDirectory = Path.Combine(Path.GetDirectoryName(absMatFilePath), "Textures");
        if (!Directory.Exists(absTextureDirectory))
        {
            Directory.CreateDirectory(absTextureDirectory);
        }
        if (Directory.EnumerateFileSystemEntries(absTextureDirectory).Any())
        {
            if (!EditorUtility.DisplayDialog("Potential texture conflict", "The texture folder is not empty. Continuing may potentially replace existing textures, which may lead to existing materials breaking", "Continue", "Abort"))
            {
                return;
            }
        }
        TextAsset materialsFile = AssetDatabase.LoadAssetAtPath<TextAsset>(matFilePath);

        HashSet<string> textureFileNames = new HashSet<string>();

        JObject materials = JsonConvert.DeserializeObject(materialsFile.text) as JObject;
        foreach (KeyValuePair<string, JToken> material in materials)
        {
            JObject materialData = material.Value as JObject;
            if (!materialData.ContainsKey("Stages"))
            {
                Debug.LogWarning($"[ImportMaterialJSon.CopyTextures] | {material.Key}: no Stages array found");
                continue;
            }

            JArray stages = materialData.GetValue("Stages") as JArray;
            JObject stageData = stages[0] as JObject;
            string colorPaletteMap = GetValue(stageData, "colorPaletteMap", string.Empty);
            if(colorPaletteMap != string.Empty) { textureFileNames.Add(colorPaletteMap); }
            string diffuseMapName = GetValue(stageData, "diffuseMap", string.Empty);
            if (diffuseMapName != string.Empty) { textureFileNames.Add(diffuseMapName); }
            string opacityMapName = GetValue(stageData, "opacityMap", string.Empty);
            if (opacityMapName != string.Empty) { textureFileNames.Add(opacityMapName); }
            string normalMapName = GetValue(stageData, "normalMap", string.Empty);
            if (normalMapName != string.Empty) { textureFileNames.Add(normalMapName); }
        }

        foreach(string textureFile in textureFileNames)
        {
            string fullPath = Path.Combine(absMaterialFolder, textureFile);
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[ImportMaterialJSon.Copy] Could not find file {fullPath}");
                continue;
            }

            File.Copy(fullPath, Path.Combine(absTextureDirectory, textureFile), true);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
