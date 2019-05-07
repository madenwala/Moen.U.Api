using Moen.U.Api.Extensions;
using System;
using System.Diagnostics;
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

        #region Addresses

        protected void SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                this.BaseUri = null;
            else
                this.BaseUri = new Uri(baseUrl);
        }

        private Uri CreateRequestUri(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            return this.BaseUri != null ? new Uri(this.BaseUri, url) : new Uri(url);
        }

        #endregion

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
            var uri = this.CreateRequestUri(url);
            this.Log("GET REQUEST URL: " + uri);
            var response = await this.Client.GetAsync(uri, ct);
            this.Log(response);
            return response;
        }

        #endregion

        #region Put

        /// <summary>
        /// Puts data to the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>
        /// <param name="content">Any content that should be passed into the post.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        protected async Task<T> PutAsync<T>(string url, CancellationToken ct, HttpContent content = null)
        {
            using (var response = await this.PutAsync(url, ct, content))
            {
                response.EnsureSuccessStatusCode();
                return await response.ContentToAsync<T>();
            }
        }

        /// <summary>
        /// Put to specified URL.
        /// </summary>
        /// <param name="url">URL to post data to.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="content">Any content that should be passed into the post.</param>
        /// <returns><see cref="HttpResponseMessage"/> returned from post.</returns>
        protected async Task<HttpResponseMessage> PutAsync(string url, CancellationToken ct, HttpContent content = null)
        {
            var uri = this.CreateRequestUri(url);
            this.Log("PUT REQUEST URL: " + uri);
            var response = await this.Client.PutAsync(uri, content, ct);
            this.Log(response);
            return response;
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
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        protected async Task<T> PostAsync<T>(string url, CancellationToken ct, HttpContent content = null)
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
        protected async Task<HttpResponseMessage> PostAsync(string url, CancellationToken ct, HttpContent content = null)
        {
            var uri = this.CreateRequestUri(url);
            this.Log("POST REQUEST URL: " + uri);
            var response = await this.Client.PostAsync(uri, content, ct);
            this.Log(response);
            return response;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Posts data to the specified URL.
        /// </summary>
        /// <typeparam name="T">Type for the strongly typed class representing data returned from the URL.</typeparam>
        /// <param name="url">URL to retrieve data from.</param>
        /// <param name="content">Any content that should be passed into the post.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Instance of the type specified representing the data returned from the URL.</returns>
        protected async Task<T> DeleteAsync<T>(string url, CancellationToken ct)
        {
            using (var response = await this.DeleteAsync(url, ct))
            {
                response.EnsureSuccessStatusCode();
                return await response.ContentToAsync<T>();
            }
        }

        /// <summary>
        /// Delete to specified URL.
        /// </summary>
        /// <param name="url">URL to post data to.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="content">Any content that should be passed into the post.</param>
        /// <returns><see cref="HttpResponseMessage"/> returned from post.</returns>
        protected async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken ct)
        {
            var uri = this.CreateRequestUri(url);
            this.Log("DELETE REQUEST URL: " + uri);
            var response = await this.Client.DeleteAsync(uri, ct);
            this.Log(response);
            return response;
        }

        #endregion

        #region Patch

        protected async Task<T> PatchAsync<T>(string url, CancellationToken ct, HttpContent contents = null)
        {
            using (var response = await this.PatchAsync(url, ct, contents))
            {
                response.EnsureSuccessStatusCode();
                return await response.ContentToAsync<T>();
            }
        }

        protected async Task<HttpResponseMessage> PatchAsync(string url, CancellationToken ct, HttpContent content = null)
        {
            var uri = this.CreateRequestUri(url);
            this.Log("PATCH REQUEST URL: " + uri);

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            if (content != null)
                request.Content = content;

            var response = await this.Client.SendAsync(request, ct);
            this.Log(response);
            return response;
        }

        #endregion

        #region Logging

        private static Action<string> _log = new Action<string>((message) => Debug.WriteLine(message));

        /// <summary>
        /// Allows you to override where logging happens via an Action method that accepts string messages to log.
        /// </summary>
        /// <param name="loggingMethod">Action method that can except string messages.</param>
        protected static void SetLogging(Action<string> loggingMethod)
        {
            if (loggingMethod != null)
                _log = loggingMethod;
        }

        /// <summary>
        /// Logs a message to the input log method.
        /// </summary>
        /// <param name="message">String to log</param>
        private void Log(string message)
        {
            _log.Invoke($"{DateTime.Now.ToString()}\t{message}");
        }

        /// <summary>
        /// Logs a <see cref="HttpResponseMessage" to the specified log./>
        /// </summary>
        /// <param name="response">Response message to log</param>
        private async void Log(HttpResponseMessage response)
        {
            var data = await response.Content?.ReadAsStringAsync();
            _log.Invoke("********");
            _log.Invoke(DateTime.Now.ToString());
            _log.Invoke($"REQUEST   URL: {response.RequestMessage.RequestUri}");
            if(response.RequestMessage.Content != null)
                _log.Invoke($"REQUEST   Content: {await response.RequestMessage.Content?.ReadAsStringAsync()}");
            _log.Invoke($"RESPONSE  Code: {(int)response.StatusCode} ({response.StatusCode})");
            if(!string.IsNullOrEmpty(data))
                _log.Invoke($"{data}");
            _log.Invoke("********");
        }

        #endregion

        #endregion
    }
}