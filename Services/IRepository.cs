using System.Collections.Generic;
using System.Threading.Tasks;

namespace WishAWish.Services
{
    public interface IRepository<T>
    {
        Task<IList<T>> LoadAsync();
        Task SaveAsync(IList<T> items);
    }
}
