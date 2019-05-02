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

        public ClientBase(string baseURL = null)
        {
            this.SetBaseUrl(baseURL);
            this.Client = new HttpClient();
        }

        public void Dispose()
        {
            this.Client.Dispose();
            this.Client = null;
        }

        #endregion

        #region Methods

        protected void SetBaseUrl(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString))
                this.BaseUri = null;
            else
                this.BaseUri = new Uri(urlString);
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

        protected async Task<HttpResponseMessage> GetAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            return await this.Client.GetAsync(new Uri(this.BaseUri, url), ct);
        }

        #endregion

        #region Post

        /// <summary>
        /// Posts data to the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>
        /// <param name="contents">Any content that should be passed into the post.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="serializerType">Specifies how the data should be deserialized.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        protected async Task<T> PostAsync<T>(string url, CancellationToken ct, HttpContent contents = default(HttpContent))
        {
            using (var response = await this.PostAsync(url, ct, contents))
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
        /// <param name="contents">Any content that should be passed into the post.</param>
        /// <returns><see cref="HttpResponseMessage"/> returned from post.</returns>
        protected async Task<HttpResponseMessage> PostAsync(string url, CancellationToken ct, HttpContent contents = default(HttpContent))
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            return await this.Client.PostAsync(new Uri(this.BaseUri, url), contents, ct);
        }

        #endregion

        #endregion
    }
}