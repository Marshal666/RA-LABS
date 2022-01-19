using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GradientToTexture : EditorWindow
{

    static Gradient gradient;
    static int outputWidth = 4, outputHeight = 256;
    static Texture2D outputTexture;
    static string outputFile = "Assets/Textures/gradTex.png";

    [MenuItem("Tools/Gradient To Texture")]
    public static void ShowWindow()
    {
        GetWindow<GradientToTexture>();
    }

    private void OnGUI()
    {
        GUILayout.MinWidth(300);
        GUILayout.MinHeight(150);
        GUILayout.BeginVertical();

        GUILayout.Label("Gradient to texture export:");

        gradient = (Gradient)EditorGUILayout.GradientField(gradient == null ? new Gradient() : gradient);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Width: ");
        outputWidth = EditorGUILayout.IntField(outputWidth);
        if (outputWidth < 1)
            outputWidth = 1;
        GUILayout.Label("Height: ");
        outputHeight = EditorGUILayout.IntField(outputHeight);
        if (outputHeight < 2)
            outputHeight = 2;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //GUILayout.Label("Output texture: ");
        //outputTexture = (Texture2D)EditorGUILayout.ObjectField(outputTexture, typeof(Texture2D), allowSceneObjects: false);
        GUILayout.Label("Output file: ");
        outputFile = EditorGUILayout.TextField(outputFile);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Generate Texture"))
        {
            if (/*outputTexture == null && */(string.IsNullOrWhiteSpace(outputFile)))
            {
                Debug.LogWarning("Output texture is null!");
                goto EndWindows;
            }
            if(outputTexture == null)
            {
                outputTexture = new Texture2D(outputWidth, outputHeight);
                
            }
            outputTexture.Resize(outputWidth, outputHeight);
            //outputTexture.Reinitialize(outputWidth, outputHeight);
            Color[] pixels = new Color[outputWidth * outputHeight];
            for (int i = 0; i < outputWidth; i++)
            {
                for (int j = 0; j < outputHeight; j++)
                {
                    pixels[j * outputWidth + i] = gradient.Evaluate(j / (outputHeight - 1.0f));
                }
            }
            outputTexture.SetPixels(pixels);
            outputTexture.filterMode = FilterMode.Point;
            File.WriteAllBytes(outputFile, outputTexture.EncodeToPNG());
            AssetDatabase.ImportAsset(outputFile);
        }

        if(GUILayout.Button("Reset"))
        {
            outputWidth = 4;
            outputHeight = 256;
            gradient = new Gradient();
            outputTexture = default;
            outputFile = "Assets/Textures/gradTex.png";
        }

    EndWindows:

        GUILayout.EndVertical();
    }

}
