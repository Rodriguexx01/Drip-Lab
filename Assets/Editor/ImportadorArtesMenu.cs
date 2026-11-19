using UnityEditor;
using UnityEngine;

// Preserve the user's exact proportions and lettering on texture import.
public class ImportadorArtesMenu : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if(!assetPath.StartsWith("Assets/Resources/UI/") && !assetPath.StartsWith("Assets/Resources/Story/") && !assetPath.StartsWith("Assets/Resources/Bench/"))return;
        var importador=(TextureImporter)assetImporter;
        importador.textureType=TextureImporterType.Default;
        importador.npotScale=TextureImporterNPOTScale.None;
        importador.maxTextureSize=4096;
        importador.mipmapEnabled=false;
        importador.textureCompression=TextureImporterCompression.Uncompressed;
        importador.filterMode=FilterMode.Bilinear;
        importador.wrapMode=TextureWrapMode.Clamp;
        importador.sRGBTexture=true;
        importador.alphaIsTransparency=true;
    }
}
