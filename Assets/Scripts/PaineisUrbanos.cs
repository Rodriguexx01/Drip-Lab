using GUI = DripLab.InterfaceCena;
using UnityEngine;

namespace DripLab
{
    public partial class JogoDripLab
    {
        GUIStyle urbanoRotulo;
        Font urbanoFonte;
        static readonly Color UrbanoAmarelo=Hex("FFB600"),UrbanoAzul=Hex("123DFF"),UrbanoPreto=Hex("090B12");
        void ConfigurarUrbanoFonte()
        {
            if(urbanoFonte==null)urbanoFonte=CarregarFonteProjeto(true);
        }
        void UrbanoArredondado(Rect r,float raio,Color cor,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using var elemento=InterfaceCena.Elemento("Painel",r,"",null,arquivo,linha);if(!elemento.Visivel)return;r=elemento.Area;
            ConfigurarControleArte();
            raio=Mathf.Min(raio,Mathf.Min(r.width,r.height)/2);
            Retangulo(r.x+raio,r.y,r.width-raio*2,r.height,cor);
            Retangulo(r.x,r.y+raio,r.width,r.height-raio*2,cor);
            Disco(r.x+raio,r.y+raio,raio*2,cor);Disco(r.xMax-raio,r.y+raio,raio*2,cor);
            Disco(r.x+raio,r.yMax-raio,raio*2,cor);Disco(r.xMax-raio,r.yMax-raio,raio*2,cor);
        }
        void UrbanoTexto(string texto,Rect area,int tamanho,Color cor,TextAnchor alinhamento=TextAnchor.UpperLeft,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using var elemento=InterfaceCena.Elemento("Texto",area,texto,null,arquivo,linha);if(!elemento.Visivel)return;area=elemento.Area;texto=elemento.Texto;
            if(urbanoRotulo==null)
            {
                ConfigurarUrbanoFonte();
                urbanoRotulo=new GUIStyle(InterfaceCena.tema.label){font=urbanoFonte!=null?urbanoFonte:fonte,wordWrap=true,padding=new RectOffset(0,0,0,0)};
            }
            urbanoRotulo.fontSize=tamanho;urbanoRotulo.alignment=alinhamento;
            urbanoRotulo.normal.textColor=cor;urbanoRotulo.hover.textColor=cor;
            urbanoRotulo.active.textColor=cor;urbanoRotulo.focused.textColor=cor;
            DesenharRotuloUrbano(area,texto,urbanoRotulo,cor,tamanho>=18);
        }
        void UrbanoPainel(Rect r,float tituloAltura,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using var elemento=InterfaceCena.Elemento("Painel",r,"",null,arquivo,linha);if(!elemento.Visivel)return;r=elemento.Area;
            UrbanoArredondado(new Rect(r.x+7,r.y+8,r.width,r.height),22,Color.black);
            UrbanoArredondado(r,22,UrbanoAmarelo);
            UrbanoArredondado(new Rect(r.x+5,r.y+5,r.width-10,r.height-10),18,UrbanoPreto);
            UrbanoArredondado(new Rect(r.x+10,r.y+10,r.width-20,tituloAltura),14,UrbanoAzul);
            // Repeatable ink flecks stay on the frame, away from readable content.
            for(int i=0;i<35;i++)
            {
                float x=r.x+16+(i*71)%(int)(r.width-32);
                float y=i%2==0?r.y+12+(i*13)%9:r.yMax-16+(i*7)%9;
                Disco(x,y,2+i%4,i%3==0?UrbanoAzul:UrbanoPreto);
            }
            DesenharLinha(new Vector2(r.x+12,r.yMax+2),new Vector2(r.x+12,r.yMax+13),UrbanoAmarelo,4);
            DesenharLinha(new Vector2(r.xMax-26,r.yMax+1),new Vector2(r.xMax-26,r.yMax+9),UrbanoAzul,5);DesenharAcabamentoPainel(r,tituloAltura);
        }
        bool UrbanoBotao(string rotulo,Rect area,bool principal,bool habilitado=true,int tamanho=22,Color? destaque=null,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            using var elemento=InterfaceCena.Elemento("Botao",area,rotulo,null,arquivo,linha);if(!elemento.Visivel)return false;area=elemento.Area;rotulo=elemento.Texto;
            bool disponivel=InterfaceCena.habilitado&&habilitado; Matrix4x4 matrizAnterior=InterfaceCena.matriz; if(disponivel&&!(rotulo.StartsWith("VOLTAR")&&!tutorialAtivo)&&rotulo!="MENU"&&area.Contains(Event.current.mousePosition))InterfaceCena.matriz=matrizAnterior*Matrix4x4.TRS(new Vector3(area.center.x,area.center.y,0),Quaternion.identity,new Vector3(1.04f,1.04f,1))*Matrix4x4.Translate(new Vector3(-area.center.x,-area.center.y,0));
            Color preenchimento=destaque??(principal?UrbanoAmarelo:UrbanoAzul);
            if(!disponivel)preenchimento=Color.Lerp(preenchimento,UrbanoPreto,.65f);
            else if(area.Contains(Event.current.mousePosition))preenchimento=Color.Lerp(preenchimento,Color.white,.12f);
            if(Event.current.type==EventType.Repaint)
            {
                UrbanoArredondado(new Rect(area.x+3,area.y+4,area.width,area.height),12,Color.black);
                UrbanoArredondado(area,12,principal?UrbanoAzul:UrbanoAmarelo);
                UrbanoArredondado(new Rect(area.x+3,area.y+3,area.width-6,area.height-6),9,preenchimento);DesenharAcabamentoBotao(area,preenchimento,disponivel);
                UrbanoTexto(rotulo,area,tamanho,disponivel?Creme:SemSom,TextAnchor.MiddleCenter);
            }
            bool anterior=InterfaceCena.habilitado;InterfaceCena.habilitado=disponivel;
            bool pressionado=InterfaceCena.Botao(area,GUIContent.none,GUIStyle.none);InterfaceCena.habilitado=anterior;
            InterfaceCena.matriz=matrizAnterior;if(pressionado)Toque();return pressionado;
        }
        static Font fonteTextoProjeto,fonteDestaqueProjeto;
        static Font CarregarFonteProjeto(bool destaque=false)
        {
            if(fonteTextoProjeto==null)fonteTextoProjeto=Resources.Load<Font>("Fonts/Texto");
            if(destaque&&fonteDestaqueProjeto==null)fonteDestaqueProjeto=Resources.Load<Font>("Fonts/Destaque");
            Font escolhida=destaque?fonteDestaqueProjeto:fonteTextoProjeto;
            if(escolhida!=null)return escolhida;
            if(fonteTextoProjeto!=null)return fonteTextoProjeto;
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        GUIContent medidaTextoUrbano;
        void CorRotuloUrbano(GUIStyle estilo,Color tinta)
        {
            estilo.normal.textColor=tinta;estilo.hover.textColor=tinta;
            estilo.active.textColor=tinta;estilo.focused.textColor=tinta;
        }
        void DesenharRotuloUrbano(Rect area,string texto,GUIStyle estilo,Color tinta,bool contorno,[System.Runtime.CompilerServices.CallerFilePath]string arquivo="",[System.Runtime.CompilerServices.CallerLineNumber]int linha=0)
        {
            if(Event.current.type!=EventType.Repaint)return;
            using var elemento=InterfaceCena.Elemento("Texto",area,texto,null,arquivo,linha);if(!elemento.Visivel)return;area=elemento.Area;texto=elemento.Texto;
            int tamanho=estilo.fontSize;bool quebraAnterior=estilo.wordWrap;
            AjustarRotuloAoCampo(area,texto,estilo,contorno);
            if(contorno&&tinta.grayscale>.18f)
            {
                float borda=estilo.fontSize>=30?1.6f:1;
                CorRotuloUrbano(estilo,Color.black);
                InterfaceCena.Rotulo(new Rect(area.x-borda,area.y,area.width,area.height),texto,estilo);
                InterfaceCena.Rotulo(new Rect(area.x+borda,area.y,area.width,area.height),texto,estilo);
                InterfaceCena.Rotulo(new Rect(area.x,area.y-borda,area.width,area.height),texto,estilo);
                InterfaceCena.Rotulo(new Rect(area.x,area.y+borda,area.width,area.height),texto,estilo);
            }
            CorRotuloUrbano(estilo,tinta);InterfaceCena.Rotulo(area,texto,estilo);estilo.fontSize=tamanho;estilo.wordWrap=quebraAnterior;
        }
        void AjustarRotuloAoCampo(Rect area,string texto,GUIStyle estilo,bool contorno)
        {
            if(medidaTextoUrbano==null)medidaTextoUrbano=new GUIContent();medidaTextoUrbano.text=texto;
            bool centroVertical=estilo.alignment==TextAnchor.MiddleLeft||estilo.alignment==TextAnchor.MiddleCenter||estilo.alignment==TextAnchor.MiddleRight;
            bool linhaUnica=!texto.Contains("\n")&&(!estilo.wordWrap||((contorno||centroVertical)&&area.height<=estilo.fontSize*2.2f));
            estilo.wordWrap=!linhaUnica;estilo.clipping=TextClipping.Clip;
            float largura=Mathf.Max(1,area.width-(contorno?6:2)),altura=Mathf.Max(1,area.height-(contorno?4:2));
            int minimo=Mathf.Min(10,estilo.fontSize);
            while(estilo.fontSize>minimo)
            {
                Vector2 tamanho=estilo.CalcSize(medidaTextoUrbano);
                float alturaTexto=linhaUnica?tamanho.y:estilo.CalcHeight(medidaTextoUrbano,largura);
                if((!linhaUnica||tamanho.x<=largura)&&alturaTexto<=altura)break;
                estilo.fontSize--;
            }
        }
        void DesenharAcabamentoPainel(Rect area,float alturaTitulo)
        {
            if(Event.current.type!=EventType.Repaint)return;
            Retangulo(area.x+20,area.y+14,Mathf.Max(1,area.width-40),2,new Color(.55f,.7f,1,.35f));
            Retangulo(area.x+16,area.y+alturaTitulo+11,Mathf.Max(1,area.width-32),2,UrbanoAmarelo);
            for(int i=0;i<5;i++)
            {
                float x=area.xMax-18-i*8;
                DesenharLinha(new Vector2(x,area.yMax-8),new Vector2(x-5,area.yMax-15),UrbanoAzul,3);
            }
        }
        void DesenharAcabamentoBotao(Rect area,Color baseCor,bool habilitado)
        {
            if(Event.current.type!=EventType.Repaint)return;
            float margem=Mathf.Min(12,area.width*.1f);
            Retangulo(area.x+margem,area.y+5,Mathf.Max(1,area.width-2*margem),2,Color.Lerp(baseCor,Color.white,.45f));
            Retangulo(area.x+margem,area.yMax-6,Mathf.Max(1,area.width-2*margem),2,Color.Lerp(baseCor,Color.black,.45f));
            if(!habilitado)return;
            for(int i=0;i<8;i++)
            {
                float x=area.x+8+(i*37)%Mathf.Max(1,(int)(area.width-16));
                float y=i%2==0?area.y+7:area.yMax-8;
                Disco(x,y,1.5f+i%2,new Color(0,0,0,.65f));
            }
        }
        void DesenharPrecoUrbano(Rect area,bool pago)
        {
            if(Event.current.type!=EventType.Repaint)return;
            Capsula(new Rect(area.x+3,area.y+4,area.width,area.height),Color.black);
            Capsula(area,UrbanoAmarelo);Capsula(new Rect(area.x+3,area.y+3,area.width-6,area.height-6),Color.black);
            Color cor=pago?Hex("D9232B"):Hex("00B936");
            Capsula(new Rect(area.x+6,area.y+6,area.width-12,area.height-12),cor);
            DesenharAcabamentoBotao(area,cor,!pago);
        }
    }
}
