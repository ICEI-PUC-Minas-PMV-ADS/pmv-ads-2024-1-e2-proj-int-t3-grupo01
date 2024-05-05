﻿namespace GestLab.Models
{
    public class PedidoModel
    {
        public PedidoModel()
        {
            Cliente = new();
            Receita = new();
            Status = "Novo";
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public UsuarioModel? MontadorResponsavel { get; set; }
        public ClienteModel Cliente { get; set; }
        public ReceitaModel Receita { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal ValorPedido { get; set; }
        public string CorLentes { get; set; }
        public bool ArmacaoEntreguePeloCliente { get; set; }
        public string IdentificacaoArmacao { get; set; }
        public bool PossuiLentesEmEstoque { get; set; }
        public ProdutoModel? LenteEsquerda { get; set; }
        public ProdutoModel? LenteDireita { get; set; }
        public ProdutoModel? Armacao { get; set; }
    }
}
