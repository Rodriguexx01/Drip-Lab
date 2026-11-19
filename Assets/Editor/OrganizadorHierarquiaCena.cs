using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using DripLab;

[Serializable] public class ManifestoHierarquiaInterface {public EntradaHierarquiaInterface[] entradas;}
[Serializable] public class EntradaHierarquiaInterface {public string tela,vinculo,nome,texto;public Rect area;}
public static class OrganizadorHierarquiaCena
{
    [MenuItem("DripLab/Organizar interface no Hierarchy")]
    public static void GerarAtual()
    {
        if(EditorApplication.isPlaying){Debug.LogWarning("Pare o Play antes de organizar a cena.");return;}
        Preencher(SceneManager.GetActiveScene());EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
    public static void Preencher(Scene cena)
    {
        var recurso=Resources.Load<TextAsset>("UiHierarchyManifest");if(recurso==null)return;
        var manifesto=JsonUtility.FromJson<ManifestoHierarquiaInterface>(recurso.text);
        GameObject raiz=null;foreach(var item in cena.GetRootGameObjects())if(item.name=="DRIP LAB - Interface")raiz=item;
        if(raiz==null){raiz=new GameObject("DRIP LAB - Interface");SceneManager.MoveGameObjectToScene(raiz,cena);}
        var existentes=new Dictionary<string,ElementoInterfaceCena>();
        foreach(var item in raiz.GetComponentsInChildren<ElementoInterfaceCena>(true))if(!string.IsNullOrEmpty(item.vinculo))existentes[item.vinculo]=item;
        foreach(var entrada in manifesto.entradas)
        {
            ElementoInterfaceCena elemento;if(existentes.TryGetValue(entrada.vinculo,out elemento))continue;
            Transform pai=raiz.transform;
            foreach(string parte in entrada.tela.Split('/'))
            {var filho=pai.Find(parte);if(filho==null){filho=new GameObject(parte).transform;filho.SetParent(pai,false);}pai=filho;}
            var objeto=new GameObject(entrada.nome);objeto.transform.SetParent(pai,false);
            elemento=objeto.AddComponent<ElementoInterfaceCena>();elemento.vinculo=entrada.vinculo;elemento.areaOriginal=entrada.area;elemento.disposicao=entrada.area;elemento.textoOriginal=entrada.texto;
        }
    }
}
