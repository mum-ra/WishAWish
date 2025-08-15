using System.Linq;

namespace WishAWish.Models
{
    public static class Cpf
    {
        public static bool Valido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;
            var d = new string(cpf.Where(char.IsDigit).ToArray());
            if (d.Length != 11 || d.Distinct().Count() == 1) return false;

            int Calc(string s)
            {
                int m = s.Length + 1, sum = 0;
                for (int i = 0; i < s.Length; i++) sum += (s[i] - '0') * (m - i);
                int r = sum % 11; return (r < 2) ? 0 : 11 - r;
            }

            var dv1 = Calc(d.Substring(0, 9));
            var dv2 = Calc(d.Substring(0, 9) + dv1);
            return d.EndsWith(dv1.ToString() + dv2);
        }
    }
}
