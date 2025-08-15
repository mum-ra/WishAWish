using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishAWish.Models;

namespace WishAWish.Services
{
    public class OrderService
    {
        private readonly JsonRepository<Pedido> _repo = new JsonRepository<Pedido>(Storage.PathFor("pedidos.json"));

        public Task<IList<Pedido>> LoadAsync() => _repo.LoadAsync();
        public Task SaveAsync(IList<Pedido> items) => _repo.SaveAsync(items);

        public int NextId(IList<Pedido> items) => items.Count == 0 ? 1 : items.Max(p => p.Id) + 1;
    }
}
