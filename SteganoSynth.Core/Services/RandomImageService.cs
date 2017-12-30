using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteganoSynth.Core
{
    public class RandomImageService
    {
        const string RandomImageEndpoint = "https://picsum.photos/150/150/?random";

        public async Task<Uri> GetRandomImage()
        {
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) })
                {
                    var request = await client.GetAsync(RandomImageEndpoint);

                    if (request.IsSuccessStatusCode)
                    {
                        return request.RequestMessage.RequestUri;
                    }

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
