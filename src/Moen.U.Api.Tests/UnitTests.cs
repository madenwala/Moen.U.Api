using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace Moen.U.Api.Tests
{
    public class UnitTests
    {
        [Fact]
        public async void Test1()
        {

            try
            {
                CancellationToken ct = new CancellationToken();
                MoenClient client = new MoenClient();
                var authResponse = await client.AuthenticateAsync("user@outlook.com", "password", ct);
                var showers = await client.GetShowersAsync(ct);
                var details = await client.GetShowerDetailsAsync(showers.First().serial_number, ct);

                var update = client.GetShowerDetailsUpdate(details);
                await client.UpdateShowerDetailsAsync(update, ct);

                // TODO fix socket details
                // var pusherAuth = await client.PusherAuth(details, "2958.2071480", ct);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
    }
}