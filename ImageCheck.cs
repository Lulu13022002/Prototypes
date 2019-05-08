using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CheckURI.Tools
{
    public static class ImageCheck
    {
        public static async Task<bool> IsValidImage(this Uri uri)
        {
            string uriT = uri.ToString();
            if (uri.Scheme == "data" && uriT.Split(',', ';')[0] != "data:" &&
                !uriT.Substring(5).Split(',', ';')[0].ToLower().StartsWith("image/")) return false;

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
    }
}
