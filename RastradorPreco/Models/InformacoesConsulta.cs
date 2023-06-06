namespace RastradorPreco.Models
{
    public class InformacoesConsulta
    {
        public int InformacoesConsultaId { get; set; }

        public int ProdutoId { get; set; }

        public double Valor { get; set; }

        public int Dia { get; set; }

        public int Mes { get; set; }

        public int Ano { get; set; }
    }
}
