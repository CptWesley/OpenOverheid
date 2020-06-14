using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenOverheid
{
    /// <summary>
    /// Abstract class for APIs.
    /// </summary>
    public abstract class OverheidApiBase : IOverheidApiBase
    {
        private readonly bool ownClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverheidApiBase"/> class.
        /// </summary>
        public OverheidApiBase()
        {
            Client = new HttpClient();
            ownClient = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverheidApiBase"/> class.
        /// </summary>
        /// <param name="client">The client used for webrequests.</param>
        public OverheidApiBase(HttpClient client)
        {
            Client = client;
            ownClient = false;
        }

        /// <summary>
        /// Gets the HttpClient used for requests.
        /// </summary>
        protected HttpClient Client { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ownClient)
                {
                    Client.Dispose();
                }
            }
        }

        /// <summary>
        /// Makes an asynchronous request.
        /// </summary>
        /// <param name="path">The URI.</param>
        /// <returns>The content of the request.</returns>
        protected async Task<JsonDocument> RequestAsync(string path)
        {
            HttpResponseMessage response = await Client.GetAsync(path);
            response.EnsureSuccessStatusCode();
            Stream stream = await response.Content.ReadAsStreamAsync();
            JsonDocument document = await JsonDocument.ParseAsync(stream);
            return document;
        }

        /// <summary>
        /// Makes a synchronous request.
        /// </summary>
        /// <param name="path">The URI.</param>
        /// <returns>The content of the request.</returns>
        protected JsonDocument Request(string path)
        {
            HttpResponseMessage response = Client.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();
            Stream stream = response.Content.ReadAsStreamAsync().Result;
            JsonDocument document = JsonDocument.Parse(stream);
            return document;
        }
    }
}
