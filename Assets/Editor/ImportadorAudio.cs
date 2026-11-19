using UnityEditor;
using UnityEngine;

public class ImportadorAudio : AssetPostprocessor
{
    void OnPreprocessAudio()
    {
        if(!assetPath.StartsWith("Assets/Resources/Audio/"))return;
        var importador=(AudioImporter)assetImporter;
        var configuracao=importador.defaultSampleSettings;
        configuracao.loadType=assetPath.Contains("/Trilha")?AudioClipLoadType.Streaming:AudioClipLoadType.DecompressOnLoad;
        configuracao.compressionFormat=assetPath.Contains("/Trilha")?AudioCompressionFormat.Vorbis:AudioCompressionFormat.PCM;
        configuracao.preloadAudioData=true;configuracao.quality=.85f;configuracao.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;
        importador.defaultSampleSettings=configuracao;importador.forceToMono=false;importador.loadInBackground=assetPath.Contains("/Trilha");
    }
}
