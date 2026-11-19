using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class DripLabWindowsOptimizedBuilder
{
    [MenuItem("DripLab/Gerar Windows menu nítido %#e")]
    public static void Build()
    {
        const string scene = "Assets/Scenes/Atelie.unity";
        if (!File.Exists(scene))
            throw new FileNotFoundException("Cena principal não encontrada", scene);

        ConfigureWindowsTextures();

        string output = Path.GetFullPath(Path.Combine(
            Application.dataPath, "../../DripLabWindowsMenuNitido/Drip Lab.exe"));
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

            int maxSize = path == "Assets/Resources/UI/MenuPrincipal3D.png" ? 2048 : 1024;

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
