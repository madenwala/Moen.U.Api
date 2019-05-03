using Moen.U.Api.Extensions;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Moen.U.Api
{
    public abstract class ClientBase : IDisposable
    {
        #region Variables

        protected HttpClient Client { get; private set; }

        protected Uri BaseUri { get; private set; }

        #endregion

        #region Constructors

        public ClientBase(string baseUrl = null)
        {
            this.SetBaseUrl(baseUrl);
            this.Client = new HttpClient();
        }

        public void Dispose()
        {
            this.Client.Dispose();
            this.Client = null;
        }

        #endregion

        #region Methods

        protected void SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                this.BaseUri = null;
            else
                this.BaseUri = new Uri(baseUrl);
        }

        protected Uri GetRequestUri(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            return this.BaseUri != null ? new Uri(this.BaseUri, url) : new Uri(url);
        }

        #region Get

        /// <summary>
        /// Gets data from the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>should be deserialized.</param>
        /// <param name="retryCount">Number of retry attempts if a call fails. Default is zero.</param>
        /// <param name="serializerType">Specifies how the data should be deserialized.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        /// <summary>
        protected async Task<T> GetAsync<T>(string url, CancellationToken ct)
        {
            using (var response = await this.GetAsync(url, ct))
            {
                response.EnsureSuccessStatusCode();
                return await response.ContentToAsync<T>();
            }
        }

        private async Task<HttpResponseMessage> GetAsync(string url, CancellationToken ct)
        {
            return await this.Client.GetAsync(this.GetRequestUri(url), ct);
        }

        #endregion

        #region Post

        /// <summary>
        /// Posts data to the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>
        /// <param name="content">Any content that should be passed into the post.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="serializerType">Specifies how the data should be deserialized.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        protected async Task<T> PostAsync<T>(string url, CancellationToken ct, HttpContent content = default(HttpContent))
        {
            using (var response = await this.PostAsync(url, ct, content))
            {
                response.EnsureSuccessStatusCode();
                return await response.ContentToAsync<T>();
            }
        }

        /// <summary>
        /// Post to specified URL.
        /// </summary>
        /// <param name="url">URL to post data to.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="content">Any content that should be passed into the post.</param>
        /// <returns><see cref="HttpResponseMessage"/> returned from post.</returns>
        protected async Task<HttpResponseMessage> PostAsync(string url, CancellationToken ct, HttpContent content = default(HttpContent))
        {
            return await this.Client.PostAsync(this.GetRequestUri(url), content, ct);
        }

        #endregion

        #region Patch

        protected async Task<T> PatchAsync<T>(string url, CancellationToken ct, HttpContent contents = default(HttpContent))
        {
            using (var response = await this.PatchAsync(url, ct, contents))
            {
                response.EnsureSuccessStatusCode();
                return await response.ContentToAsync<T>();
            }
        }

        protected async Task<HttpResponseMessage> PatchAsync(string url, CancellationToken ct, HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), this.GetRequestUri(url))
            {
                Content = content
            };
            return await this.Client.SendAsync(request, ct);
        }

        #endregion Patch

        #endregion
    }
}