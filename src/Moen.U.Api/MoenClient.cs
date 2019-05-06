using Moen.U.Api.Models;
using Newtonsoft.Json;
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
            try
            {
                this.Client.DefaultRequestHeaders.Accept.Clear();
                this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                this.UserAuthentication = await this.GetAsync<UserAuthentication>($"/v2/authenticate?password={password}&email={email}", ct);

                return this.UserAuthentication;
            }
            finally
            {
                this.Client.DefaultRequestHeaders.Accept.Clear();
            }
        }

        public async Task ForgotPasswordAsync(string email, CancellationToken ct)
        {
            using (var response = await this.PostAsync($"/v2/reset_tokens?language=0&email={email}", ct))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task ForgotPasswordAsync(string token, string newPassword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            using (var response = await this.DeleteAsync($"/v2/reset_tokens/{token}?password={newPassword}", ct))
            {
                response.EnsureSuccessStatusCode();
            }            
        }

        public async Task ChangeEmailAsync(UserAuthentication userAuthentication, string newEmail, string currentPassword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userAuthentication?.token))
                throw new ArgumentNullException(nameof(userAuthentication.token));
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentNullException(nameof(newEmail));
            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ArgumentNullException(nameof(currentPassword));

            var url = $"/v3/users/{userAuthentication.token}?user%5Bemail%5D={newEmail}&user%5Bcurrent_password%5D={currentPassword}";
            using (var response = await this.PatchAsync(url, ct))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task ChangePasswordAsync(UserAuthentication userAuthentication, string currentPassword, string newPassword, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userAuthentication?.token))
                throw new ArgumentNullException(nameof(userAuthentication.token));
            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ArgumentNullException(nameof(currentPassword));
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException(nameof(newPassword));

            var url = $"/v3/users/{userAuthentication.token}?user%5Bcurrent_password%5D={currentPassword}&user%5Bpassword%5D={newPassword}";
            using (var response = await this.PatchAsync(url, ct))
            {
                response.EnsureSuccessStatusCode();
            }
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

        public ShowerDetailsUpdate GetShowerDetailsUpdate(ShowerDetails showerDetails)
        {
            if (showerDetails == null)
                throw new ArgumentNullException(nameof(showerDetails));
            if (string.IsNullOrWhiteSpace(showerDetails.serial_number))
                throw new ArgumentNullException(nameof(showerDetails.serial_number));

            var data = JsonConvert.DeserializeObject<ShowerDetailsUpdate>(JsonConvert.SerializeObject(showerDetails));
            data.serial_number = showerDetails.serial_number;
            return data;
        }

        public async Task UpdateShowerDetailsAsync(ShowerDetailsUpdate showerDetailsUpdate, CancellationToken ct)
        {
            if (showerDetailsUpdate == null)
                throw new ArgumentNullException(nameof(showerDetailsUpdate));
            if (string.IsNullOrWhiteSpace(showerDetailsUpdate.serial_number))
                throw new ArgumentNullException(nameof(showerDetailsUpdate.serial_number));

            var url = $"/v4/showers/{showerDetailsUpdate.serial_number}";
            var requestData = new ShowerDetailsUpdateRequest() { shower = showerDetailsUpdate };
            var content = new StringContent(JsonConvert.SerializeObject(requestData));

            this.SetHeaders();
            using (var response = await this.PatchAsync(url, ct, content))
            {
                var c = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
        }

        #endregion

        #region Configuration

        public Task<PusherAuth> PusherAuthAsync(ShowerDetails showerDetails, string socket_id, CancellationToken ct)
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

        #endregion

        #endregion

        #endregion Methods
    }
}