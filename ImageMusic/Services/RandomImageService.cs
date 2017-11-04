using System.Threading.Tasks;
using System.Net;
using AppKit;

namespace ImageMusic
{
    public class RandomImageService
    {
        const string RandomImageEndpoint = "https://source.unsplash.com/random/150x150";

        public async Task<NSImage> GetRandomImage ()
        {
            try
            {
                var request = WebRequest.Create(RandomImageEndpoint);
                request.Timeout = 3000;
                var response = await request.GetResponseAsync();

                var stream = response.GetResponseStream();

                return NSImage.FromStream(stream);
            }
            catch//this should be smarter
            {
                return null;
            }
        }
    }
}