using Moen.U.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Moen.U.Api
{
    public sealed class MoenClient : ClientBase
    {
        #region Variables

        private const string BASE_URL = "https://www.moen-iot.com";

        private UserAuthentication UserAuthentication { get; set; }

        #endregion

        #region Constructors

        public MoenClient() : base(BASE_URL)
        {
        }

        #endregion

        #region Methods

        #region Private

        private void SetHeaders()
        {
            if (this.UserAuthentication == null)
                throw new UnauthorizedAccessException("User is not authenticated!");

            this.Client.DefaultRequestHeaders.Clear();
            this.Client.DefaultRequestHeaders.Add("User-Token", this.UserAuthentication.token);
            this.Client.DefaultRequestHeaders.Accept.Clear();
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Public

        #region Authentication

        public async Task<UserAuthentication> AuthenticateAsync(string email, string password, CancellationToken ct)
        {
            this.Client.DefaultRequestHeaders.Accept.Clear();
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.UserAuthentication = await this.GetAsync<UserAuthentication>($"/v2/authenticate?password={password}&email={email}", ct);
            return this.UserAuthentication;
        }

        #endregion

        #region Shower

        public Task<IList<Shower>> GetShowersAsync(CancellationToken ct)
        {
            this.SetHeaders();
            return this.GetAsync<IList<Shower>>("/v2/showers", ct);
        }

        public Task<ShowerDetails> GetShowerDetailsAsync(string serialNumber, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw new ArgumentNullException(nameof(serialNumber));

            this.SetHeaders();
            return this.GetAsync<ShowerDetails>($"/v5/showers/{serialNumber}", ct);
        }

        #endregion Showers


        public Task<PusherAuth> PusherAuth(ShowerDetails showerDetails, string socket_id, CancellationToken ct)
        {
            if (showerDetails == null)
                throw new ArgumentNullException(nameof(showerDetails));
            if (string.IsNullOrWhiteSpace(showerDetails.channel))
                throw new ArgumentNullException(nameof(showerDetails.channel));
            if (string.IsNullOrWhiteSpace(socket_id))
                throw new ArgumentNullException(nameof(socket_id));

            //socket_id   2958.2071480
            //channel_name    private-7fb4a4cb-cc48-4fd5-bba9-f418084402d1
            var dic = new Dictionary<string, string>();
            dic.Add("socket_id", socket_id);
            dic.Add("channel_name", $"private-{showerDetails.channel}");

            this.SetHeaders();
            return this.PostAsync<PusherAuth>("/v3/pusher-auth", ct, new FormUrlEncodedContent(dic));
        }

        public Task SetCapabilitiesAsync(ShowerDetails showerDetails, CancellationToken ct)
        {
            if (showerDetails == null)
                throw new ArgumentNullException(nameof(showerDetails));
            if (string.IsNullOrWhiteSpace(showerDetails.token))
                throw new ArgumentNullException(nameof(showerDetails.token));

            var capability = showerDetails.capabilities?.FirstOrDefault()?.name ?? "mobile_supports_pusher";

            this.Client.DefaultRequestHeaders.Clear();
            this.Client.DefaultRequestHeaders.Add("Shower-Token", showerDetails.token);
            return this.PostAsync($"/v2/capabilities?name={capability}", ct);
        }

        #endregion Public

        #endregion Methods
    }
}