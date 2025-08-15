using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishAWish.Models;

namespace WishAWish.Services
{
    public class PersonService
    {
        private readonly JsonRepository<Pessoa> _repo = new JsonRepository<Pessoa>(Storage.PathFor("pessoas.json"));

        public Task<IList<Pessoa>> LoadAsync() => _repo.LoadAsync();
        public Task SaveAsync(IList<Pessoa> items) => _repo.SaveAsync(items);

        public int NextId(IList<Pessoa> items) => items.Count == 0 ? 1 : items.Max(p => p.Id) + 1;
    }
}
