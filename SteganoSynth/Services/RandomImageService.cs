using System;
using System.Threading.Tasks;
using System.Net;
using AppKit;
using Foundation;

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

                return new NSImage(NSUrl.FromString(response.ResponseUri.ToString()));
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}