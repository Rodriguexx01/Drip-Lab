using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class DripLabWebBuilder
{
    [MenuItem("DripLab/Gerar jogo Web %#w")]
    public static void Build()
    {
        const string scene = "Assets/Scenes/Atelie.unity";
        if (!File.Exists(scene))
            throw new FileNotFoundException("Cena principal não encontrada", scene);

        ConfigureWebTextures();
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;

        string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../DripLabWebOtimizado"));
        Directory.CreateDirectory(output);

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { scene },
            locationPathName = output,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build Web falhou: " + report.summary.result +
                                " (erros: " + report.summary.totalErrors + ")");

        Debug.Log("DRIPLAB_WEB_BUILD_OK: " + output);
    }

    private static void ConfigureWebTextures()
    {
        string[] folders = { "Assets/Resources/UI", "Assets/Resources/Story", "Assets/Resources/Bench" };
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", folders))
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
            int quality = 50;
            var web = importer.GetPlatformTextureSettings("WebGL");
            if (web.overridden && web.maxTextureSize == maxSize &&
                web.textureCompression == TextureImporterCompression.Compressed &&
                web.crunchedCompression && web.compressionQuality == quality)
                continue;

            web.name = "WebGL";
            web.overridden = true;
            web.maxTextureSize = maxSize;
            web.format = TextureImporterFormat.Automatic;
            web.textureCompression = TextureImporterCompression.Compressed;
            web.crunchedCompression = true;
            web.compressionQuality = quality;
            importer.SetPlatformTextureSettings(web);
            importer.SaveAndReimport();
        }
    }
}
