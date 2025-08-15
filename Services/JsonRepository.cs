using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WishAWish.Services
{
    public class JsonRepository<T> : IRepository<T>
    {
        private readonly string _path;
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();

        public JsonRepository(string path) { _path = path; }

        public async Task<IList<T>> LoadAsync()
        {
            if (!File.Exists(_path)) return new List<T>();
            var json = await Task.Run(() => File.ReadAllText(_path, Encoding.UTF8));
            return _js.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public Task SaveAsync(IList<T> items)
        {
            var json = _js.Serialize(items);
            return Task.Run(() => File.WriteAllText(_path, json, Encoding.UTF8));
        }
    }
}
