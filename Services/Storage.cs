using System;
using System.IO;

namespace WishAWish.Services
{
    public static class Storage
    {
        public static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        static Storage() { Directory.CreateDirectory(DataDir); }
        public static string PathFor(string file) => Path.Combine(DataDir, file);
    }
}
