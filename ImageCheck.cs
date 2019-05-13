using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CheckURI.Tools
{
    public static class ImageCheck
    {
        public static async Task<bool> IsValidImage(this Uri uri, string contentType = null)
        {
            if (contentType != null) return contentType.StartsWith("image/");
            if (uri.Scheme == "data" && !uri.ToString().Split(':')[1].ToLower().StartsWith("image/")) return false;

            using (var response = await Downloader.client.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (HttpContent content = response.Content)
                    {
                        string mediaType = content.Headers.ContentType.MediaType;
                        if (!String.IsNullOrEmpty(mediaType) && !mediaType.StartsWith("image/")) return false;
                        using (var stream = await content.ReadAsStreamAsync())
                        {
                            if (stream == null || stream.Length == 0) return false;
                            using (var br = new BinaryReader(stream))
                            {
                                var soi = br.ReadUInt16(); 
                                var jfif = br.ReadUInt16();
                                return soi == 0xd8ff && jfif == 0xe0ff;
                            }
                        }
                    }
                }
            }
            return false;
        }
    
        public static bool CanRead(string path)
        {
            bool canAccess = false;

            DirectorySecurity acl = new DirectoryInfo(path).GetAccessControl();
            if (acl == null) return false;
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
            if (rules == null) return false;

            var identify = WindowsIdentity.GetCurrent();
            foreach (AuthorizationRule rule in rules)
            {
                if (!(rule is FileSystemAccessRule fsAccessRule)) continue;
                if (identify.Groups.Contains(rule.IdentityReference))
                {
                    if (fsAccessRule.FileSystemRights.HasFlag(FileSystemRights.Read))
                    {
                        if (fsAccessRule.AccessControlType == AccessControlType.Deny) return false;
                        canAccess = true;
                    }
                }
            }
            return canAccess;
        }
    }
}
