using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using DripLab;

[InitializeOnLoad]
public static class PreparadorProjeto
{
    static PreparadorProjeto()
    {
        if(!Application.isBatchMode)
            EditorApplication.delayCall += () =>
            {
                if(EditorApplication.isCompiling)return;
                if(EditorBuildSettings.scenes.Length==0)Preparar();
                else if(PlayerSettings.companyName!="DG Studio" || PlayerSettings.productName!="Drip Lab")
                {
                    Configurar();
                    AssetDatabase.SaveAssets();
                    if(string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))EditorSceneManager.OpenScene("Assets/Scenes/Atelie.unity");
                }
            };
    }
    [MenuItem("DripLab/Preparar cena e testar regras")]
    public static void Preparar()
    {
        VerificarEconomia();
        var cena=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var cameraObjeto=new GameObject("Main Camera");var camera=cameraObjeto.AddComponent<Camera>();camera.orthographic=true;camera.backgroundColor=new Color(.95f,.94f,.91f);camera.clearFlags=CameraClearFlags.SolidColor;cameraObjeto.tag="MainCamera";cameraObjeto.transform.position=new Vector3(0,0,-10);
        cameraObjeto.AddComponent<AudioListener>();
        new GameObject("DripLab · Game Manager").AddComponent<JogoDripLab>();
        OrganizadorHierarquiaCena.Preencher(cena);
        EditorSceneManager.SaveScene(cena,"Assets/Scenes/Atelie.unity");
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Atelie.unity",true)};
        Configurar();
        AssetDatabase.SaveAssets();
        Debug.Log("DRIPLAB: scene ready and economy checks passed.");
    }
    static void Configurar()
    {
        PlayerSettings.companyName="DG Studio";PlayerSettings.productName="Drip Lab";
        PlayerSettings.defaultScreenWidth=1440;PlayerSettings.defaultScreenHeight=900;
        PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.resizableWindow=true;
        PlayerSettings.runInBackground=true;PlayerSettings.colorSpace=ColorSpace.Gamma;
        PlayerSettings.bundleVersion="0.2.0";
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
    }
    [MenuItem("DripLab/Gerar jogo Windows")]
    public static void Gerar()
    {
        Preparar();
        string caminho=Path.GetFullPath(Path.Combine(Application.dataPath,"../../DripLab-Novo-Windows/Drip Lab.exe"));
        Directory.CreateDirectory(Path.GetDirectoryName(caminho));
        var resumo=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/Atelie.unity"},locationPathName=caminho,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
        if(resumo.summary.result!=BuildResult.Succeeded)throw new Exception("DripLab build failed: "+resumo.summary.result);
        Debug.Log("DRIPLAB: BUILD SUCCEEDED · "+caminho);
    }
    static void Verificar(bool ok,string nome) {if(!ok)throw new Exception("ECONOMY TEST FAILED: "+nome);Debug.Log("PASS: "+nome);}
    public static void VerificarEconomia()
    {
        var economia=new EconomiaJogo();
        economia.ComprarTecido(10);Verificar(economia.estado.dinheiro==2320&&economia.estado.tecido==10,"Fabric costs and inventory");
        economia.Fabricar(0,10);Verificar(economia.estado.dinheiro==2100&&economia.estado.tecido==9&&economia.estado.producao.Count==1,"Production charges both inputs");
        economia.ProximoDia();Verificar(economia.estado.dia==2&&economia.estado.estoque[0]==10,"Next-day production delivery");
        economia.Lancar(0,10,89,true);Verificar(economia.estado.estoque[0]==0&&economia.estado.drops[0].restante==10,"Drop stock reserved once");
        int limite=0;while(economia.estado.pedidos.Count==0&&limite++<3)economia.ProximoDia();
        Verificar(economia.estado.pedidos.Count>0,"Demand creates real orders");
        var o=economia.estado.pedidos[0];int dinheiro=economia.estado.dinheiro;int quantidade=economia.estado.pedidos.Count;int taxa=o.caixa==0?5:7;
        economia.Entregar(o.id,o.caixa,o.adesivo);Verificar(economia.estado.dinheiro==dinheiro+89-taxa&&economia.estado.fama==3&&economia.estado.pedidos.Count==quantidade-1,"Packaging, net payment, fame and removal");
        economia.Entregar(o.id,0,0);Verificar(economia.estado.dinheiro==dinheiro+89-taxa,"Duplicate shipping blocked");
        int saldo=economia.estado.dinheiro;economia.Fabricar(1,100);Verificar(economia.estado.dinheiro==saldo,"Invalid production cannot charge money");
        var vencimento=new EconomiaJogo();vencimento.estado.estoque[0]=9;vencimento.Lancar(0,9,300,true);
        for(int i=0;i<3;i++)vencimento.ProximoDia();Verificar(vencimento.estado.drops.Count==0,"Limited drops expire after three sales days");
        int armazenado=vencimento.estado.estoque[0];foreach(var pedido in vencimento.estado.pedidos)armazenado++;Verificar(armazenado==9,"Expiration preserves all unsold and pending units");
        for(int i=0;i<5;i++)vencimento.ProximoDia();Verificar(vencimento.estado.estoque[0]==9&&vencimento.estado.pedidos.Count==0,"Canceled orders return stock");
        var financeiro=new EconomiaJogo();for(int i=0;i<14;i++)financeiro.ProximoDia();Verificar(financeiro.estado.contas.Count==5&&financeiro.estado.contas[0].valor==150,"Despesas iniciais únicas, sem renovação ou acréscimo");
        financeiro.estado.dinheiro=1000;financeiro.PagarConta(financeiro.estado.contas[0].id);Verificar(financeiro.estado.dinheiro==850&&financeiro.estado.contas[0].pago,"Bill payment");
        int naoDividaDinheiro=financeiro.estado.dinheiro;financeiro.PagarDivida(250);Verificar(financeiro.estado.divida==0&&financeiro.estado.dinheiro==naoDividaDinheiro,"Initial help is never charged");
        financeiro.estado.fama=100;financeiro.estado.divida=2500;financeiro.VerificarVitoria();Verificar(financeiro.estado.final=="win","Fame 100 wins regardless of old debt");
        var anterior=new EstadoJogo{dia=5,dinheiro=731,divida=2500,fama=99};var migrado=new EconomiaJogo(anterior);
        Verificar(migrado.estado.divida==0&&migrado.estado.dinheiro==731&&migrado.estado.dia==5&&migrado.estado.final=="","Old save loses debt without losing money or progress");
        var famosa=new EconomiaJogo(new EstadoJogo{dinheiro=500,divida=2500,fama=100});Verificar(famosa.estado.final=="win"&&famosa.estado.dinheiro==500,"Existing fame 100 save wins without payment");
        var risco=new EconomiaJogo();risco.ComprarMoeda(100);Verificar(risco.estado.dinheiro==2400&&Math.Abs(risco.estado.moedas-1)<.001,"Crypto purchase");
        risco.estado.precoMoeda=120;risco.VenderMoeda();Verificar(risco.estado.dinheiro==2520&&risco.estado.moedas==0,"Crypto liquidation");
        int numero;risco.Roleta(50,true,out numero);Verificar(numero>=0&&numero<=36&&(risco.estado.dinheiro==2470||risco.estado.dinheiro==2570),"Roulette bounds and payout");
        risco.estado.dinheiro=0;risco.AbrirFalencia();Verificar(risco.estado.final=="bankrupt","Bankruptcy at zero balance");
        var campanha=new EconomiaJogo();campanha.ContratarPublicidade(2);campanha.ProximoDia();Verificar(campanha.Publicidade()==2,"Outdoors remains active after first day");for(int i=0;i<4;i++)campanha.ProximoDia();Verificar(campanha.Publicidade()==0,"Campaign expires after advertised duration");
        string json=JsonUtility.ToJson(economia.estado);var carregado=JsonUtility.FromJson<EstadoJogo>(json);Verificar(carregado.dia==economia.estado.dia&&carregado.dinheiro==economia.estado.dinheiro&&carregado.pedidos.Count==economia.estado.pedidos.Count,"Save round trip");
        // A deterministic viable strategy must be able to reach the actual end state.
        var tocar=new EconomiaJogo();tocar.estado.introducaoVista=true;
        for(int dia=0;dia<100 && tocar.estado.final=="";dia++)
        {
            foreach(var pedido in tocar.estado.pedidos.ToArray())tocar.Entregar(pedido.id,pedido.caixa,pedido.adesivo);
            foreach(var conta in tocar.estado.contas.ToArray())if(!conta.pago&&tocar.estado.dia>=conta.vencimentoDia&&tocar.estado.dinheiro>=conta.valor+700)tocar.PagarConta(conta.id);
            if(tocar.estado.estoque[0]>0)tocar.LancarProdutoPronto(0);
            if(tocar.estado.dinheiro>=1000&&tocar.estado.fama==0&&!tocar.PublicidadeAtivo(0))tocar.ContratarPublicidade(0);
            if(tocar.estado.dinheiro>=500&&!tocar.PublicidadeAtivo(2))tocar.ContratarPublicidade(2);
            int restante=0;foreach(var colecao in tocar.estado.drops)restante+=colecao.restante;
            if(tocar.estado.dinheiro>=540 && tocar.estado.producao.Count==0 && restante<25){tocar.ComprarTecidoCatalogo(2,2);tocar.Fabricar(0,20);}
            tocar.VerificarVitoria();
            if(tocar.estado.final=="")tocar.ProximoDia();
        }
        Verificar(tocar.estado.final=="win","Complete production-sales-shipping loop can win without gambling");
        Debug.Log("Complete playthrough: victory on day "+tocar.estado.dia+", cash R$ "+tocar.estado.dinheiro);
    }
}
