using System.Collections.Generic;

namespace DripLab
{
    // Folding belongs to the order, so changing orders cannot reuse another shirt's progress.
    public sealed class DobraPecas
    {
        readonly Dictionary<int,int> etapas=new Dictionary<int,int>();
        readonly Dictionary<int,int> necessariasDobras=new Dictionary<int,int>();
        public int IdAtivo {get;private set;}
        public int Etapa(int id){return etapas.TryGetValue(id,out int etapa)?etapa:0;}
        public bool EstaDobrada(int id){return necessariasDobras.TryGetValue(id,out int dobras)&&Etapa(id)>=dobras;}
        public void Iniciar(int id,int dobras=2){if(id>0&&dobras>=0){IdAtivo=id;if(!necessariasDobras.ContainsKey(id))necessariasDobras[id]=dobras;}}
        public void Clique(){if(IdAtivo>0)etapas[IdAtivo]=System.Math.Min(necessariasDobras[IdAtivo],Etapa(IdAtivo)+1);}
        public void Sair(){IdAtivo=0;}
        public void Reiniciar(){Sair();etapas.Clear();necessariasDobras.Clear();}
    }
}
