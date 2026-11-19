namespace DripLab
{
    public partial class JogoDripLab
    {
        string TabletHierarquiaTela()
        {
            switch(aba){case 0:return "Tablet/Inicio";case 1:return "Tablet/Confeccao";case 2:return "Tablet/Site";case 3:return "Tablet/Pedidos";case 4:return "Tablet/Publicidade";case 5:return "Tablet/Despesas";case 6:return "Tablet/Investimentos";case 7:return "Tablet/Noticias";case 8:return "Tablet/Gestao";case 9:return "Tablet/Tecidos";case 10:return "Tablet/Collabs";default:return "Tablet";}
        }
        void OnGUI()
        {
            InterfaceCena.IniciarQuadro();
            DesenharInterfaceJogo();
        }
    }
}
