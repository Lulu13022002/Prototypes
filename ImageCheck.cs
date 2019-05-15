using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CheckURI.Tools
{
    public static class ImageCheck
    {
        public static async Task<bool> IsValidMedia(this Uri uri, string media, string contentType = null)
        {
            string filter = media + "/";
            if (contentType != null) return contentType.StartsWith(filter);
            if (uri.Scheme == "data") return uri.ToString().Split(':')[1].ToLower().StartsWith(filter);

            using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Head, uri))
            using (var response = await Downloader.client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (HttpContent content = response.Content)
                    {
                        string mediaType = content.Headers.ContentType.MediaType;
                        if (!String.IsNullOrEmpty(mediaType)) return mediaType.StartsWith(filter);
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
