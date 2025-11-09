using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Toon.Serde
{
    public class ToonHttpClientHandler : DelegatingHandler
    {
        private readonly ToonOptions _options;

        public ToonHttpClientHandler(ToonOptions? options = null, HttpMessageHandler? inner = null)
            : base(inner ?? new HttpClientHandler())
        {
            _options = options ?? new ToonOptions();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null && request.Content.Headers.ContentType?.MediaType == "application/json")
            {
                string json;
#if NET5_0_OR_GREATER
                // Modern overload that accepts CancellationToken
                json = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                // Older runtimes don't expose ReadAsStringAsync(CancellationToken)
                // Fallback: use the parameterless ReadAsStringAsync and honor cancellation afterward.
                json = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
#endif
                try
                {
                    var token = JToken.Parse(json);
                    var toon = ToonConverter.ToToon(token, _options);
                    request.Content = new StringContent(toon, Encoding.UTF8, "text/plain");
                    request.Headers.Remove("X-TOON");
                    request.Headers.Add("X-TOON", "1");
                }
                catch (Newtonsoft.Json.JsonException)
                {
                    // If JSON parsing fails, fall back to original content (do nothing)
                }
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
