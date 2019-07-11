using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CheckURI.Tools
{
    public static class ImageCheck
    {
        public static async Task<bool> IsValidMedia(this Uri uri, string media, string contentType = null)
        {
            string filter = media + "/";
            if (contentType != null) return contentType.StartsWith(filter);
            if (uri.Scheme == "data") return uri.ToString().Split(':')[1].ToLower().StartsWith(filter);
                
            try
            {
                using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Head, uri))
                using (HttpResponseMessage response = await Downloader.client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead))
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
            } catch { }
            return false;
        }
    
        public static bool CanRead(string path)
        {
            bool canAccess = false;

            DirectorySecurity acl = new DirectoryInfo(path).GetAccessControl();
            if (acl == null) return false;
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
            if (rules == null) return false;

            WindowsIdentity identify = WindowsIdentity.GetCurrent();
            for (int i = 0, l = rules.Count; i < l; i++)
            {
                AuthorizationRule rule = rules[i];
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
