using System;
using System.Collections.Generic;
using System.Linq;

namespace WishAWish.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.Now;
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public FormaPagamento FormaPagamento { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal);
    }
}
