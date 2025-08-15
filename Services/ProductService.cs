using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishAWish.Models;

namespace WishAWish.Services
{
    public class ProductService
    {
        private readonly JsonRepository<Produto> _repo = new JsonRepository<Produto>(Storage.PathFor("produtos.json"));

        public Task<IList<Produto>> LoadAsync() => _repo.LoadAsync();
        public Task SaveAsync(IList<Produto> items) => _repo.SaveAsync(items);

        public int NextId(IList<Produto> items) => items.Count == 0 ? 1 : items.Max(p => p.Id) + 1;
    }
}
