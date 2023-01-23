using System.Net.Http;
using System.Threading.Tasks;

#nullable enable

namespace PetAdoptUI.Image
{
    public class ImageManager
    {
        public async Task<byte[]> DownloadFileAsync(string uri)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            return await httpClient.GetByteArrayAsync(uri);
        }
    }
}