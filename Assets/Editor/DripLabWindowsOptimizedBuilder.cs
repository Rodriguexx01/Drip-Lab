using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class DripLabWindowsOptimizedBuilder
{
    [MenuItem("DripLab/Gerar Windows otimizado %#e")]
    public static void Build()
    {
        const string scene = "Assets/Scenes/Atelie.unity";
        if (!File.Exists(scene))
            throw new FileNotFoundException("Cena principal não encontrada", scene);

        ConfigureWindowsTextures();

        string output = Path.GetFullPath(Path.Combine(
            Application.dataPath, "../../DripLabWindowsOtimizado/Drip Lab.exe"));
        Directory.CreateDirectory(Path.GetDirectoryName(output));

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { scene },
            locationPathName = output,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build Windows falhou: " + report.summary.result +
                                " (erros: " + report.summary.totalErrors + ")");

        Debug.Log("DRIPLAB_WINDOWS_OPTIMIZED_BUILD_OK: " + output);
    }

    private static void ConfigureWindowsTextures()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            bool isBenchDetail = path.StartsWith("Assets/Resources/Bench/") &&
                                 !path.EndsWith("/Workshop.png");
            bool isUiDetail = path.StartsWith("Assets/Resources/UI/") &&
                              !path.EndsWith("Catalog.png") &&
                              !path.EndsWith("Panel.png") &&
                              !path.EndsWith("Home.png") &&
                              !path.EndsWith("Background.png") &&
                              !path.EndsWith("MenuPrincipal3D.png") &&
                              !path.EndsWith("Victory.png") &&
                              !path.EndsWith("Defeat.png") &&
                              !path.EndsWith("LoadingCrown.png") &&
                              !path.EndsWith("AudioOptions.png") &&
                              !path.EndsWith("DailySummary.png");
            int maxSize = isBenchDetail || isUiDetail ? 1024 : 2048;

            var windows = importer.GetPlatformTextureSettings("Standalone");
            if (windows.overridden && windows.maxTextureSize == maxSize &&
                windows.textureCompression == TextureImporterCompression.Compressed &&
                windows.crunchedCompression && windows.compressionQuality == 50)
                continue;

            windows.name = "Standalone";
            windows.overridden = true;
            windows.maxTextureSize = maxSize;
            windows.format = TextureImporterFormat.Automatic;
            windows.textureCompression = TextureImporterCompression.Compressed;
            windows.crunchedCompression = true;
            windows.compressionQuality = 50;
            importer.SetPlatformTextureSettings(windows);
            importer.SaveAndReimport();
        }
    }
}
