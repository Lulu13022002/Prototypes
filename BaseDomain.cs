using System;
using System.Threading.Tasks;

namespace CheckURI.Tools
{
    public static class BaseDomain
    {
        public static async Task<string> GetBaseDomain(this Uri uri)
        {
            string host = uri.Host;
            if (host.Contains("."))
            {
                var split = host.Split('.');
                if (split.Length == 2) return host;
                else
                {
                    int i = 2;
                    string supposed = split[split.Length - 1];
                    while (!(await ("http://" + supposed).IsValidWebRequest()) && i < split.Length)
                    {
                        supposed = $"{split[split.Length - i]}.{supposed}";
                        i++;
                    }
                    return supposed;
                }
            }
            return null;
        }

        //lite version but not sure at 100%
        public static string GetBaseDomainLite(this Uri uri)
        {
            string host = uri.Host;
            if (host.Contains("."))
            {
                var split = host.Split('.');
                if (split.Length == 2) return host;
                else
                {
                    string supposed = $"{split[split.Length - 2]}.{split[split.Length - 1]}";
                    string[] exts = { "co.uk", "gouv.fr", "co.za", "co.in", "co.nz", "co.il", "co.zw" };
                    if (Array.Exists(exts, x => supposed.Contains(x))) supposed = $"{split[split.Length - 3]}.{supposed}";
                    return supposed;
                }
            }
            return null;
        }
    }
}
