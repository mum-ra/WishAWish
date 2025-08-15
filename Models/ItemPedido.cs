namespace WishAWish.Models
{
    public class ItemPedido
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal => ValorUnitario * Quantidade;
    }
}
