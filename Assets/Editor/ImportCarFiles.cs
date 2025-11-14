using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ImportCarFiles
{
    [MenuItem("Assets/CMS21/Build Material")]
    static void BuildMaterial()
    {
        // _ALPHATEST_ON _DISABLE_SSR_TRANSPARENT _DOUBLESIDED_ON _ENABLE_FOG_ON_TRANSPARENT _NORMALMAP_TANGENT_SPACE _SURFACE_TYPE_TRANSPARENT
        // _ALPHATEST_ON _DISABLE_SSR_TRANSPARENT _DOUBLESIDED_ON _ENABLE_FOG_ON_TRANSPARENT _NORMALMAP_TANGENT_SPACE _SURFACE_TYPE_TRANSPARENT
        Material newMaterial = new Material(Shader.Find("Shader Graphs/testgen_shader"));
        newMaterial.name = "Test Generated Material";
        Texture2D matTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Cars/gmvtest/Textures/alpha_stamp0.dds");
        //newMaterial.SetTexture("_MainTex", matTexture);
        newMaterial.mainTexture = matTexture;
        //newMaterial.SetTexture("_MainTex", matTexture);
        foreach (string texname in newMaterial.GetTexturePropertyNames())
        {
            Debug.Log(texname);
        }
        //JsonUtility.FromJson<Dictionary<string, object>>("");
        //newMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        //newMaterial.SetOverrideTag("RenderType", "Transparent");
        //newMaterial.SetFloat("_DoubleSidedEnable", 1.0f);
        //newMaterial.SetFloat("_TransmissionEnable", 1.0f);
        //newMaterial.SetFloat("_SurfaceType", 1.0f);
        //newMaterial.SetFloat("_AlphaCutoffEnable", 1.0f);
        //newMaterial.doubleSidedGI = false;
        //newMaterial.EnableKeyword("_ZWrite");
        //newMaterial.SetFloat("_ZWrite", 1.0f);
        //newMaterial.SetFloat("_EnableDepthWrite", 1.0f);
        EditorUtility.SetDirty(newMaterial);
        
        //newMaterial.SetFloat("_DepthWrite", 1.0f);

        Debug.Log(newMaterial.GetFloat("_ZWrite").ToString());
        newMaterial.SetFloat("_AllowRepaint", 1.0f);
        Debug.Log(newMaterial.GetFloat("_AllowRepaint").ToString());

        AssetDatabase.DeleteAsset("Assets/testgen.mat");
        AssetDatabase.CreateAsset(newMaterial, "Assets/testgen.mat");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //newMaterial.SetFloat("_ZWrite", 1.0f);
        Debug.Log(newMaterial.GetFloat("_ZWrite").ToString());
    }
}
