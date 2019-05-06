using Newtonsoft.Json;

namespace Moen.U.Api.Models
{
    public sealed class ShowerDetails
    {
        public int active_preset { get; set; }
        public string api_server { get; set; }
        public object battery_level { get; set; }
        public string channel { get; set; }
        public string current_firmware_version { get; set; }
        public int current_temperature { get; set; }
        public int display_brightness { get; set; }
        public int language { get; set; }
        public int max_temp { get; set; }
        public string mode { get; set; }
        public string name { get; set; }
        public bool off_on_idle { get; set; }
        public string preset_greeting { get; set; }
        public string preset_title { get; set; }
        public bool ready_pauses_water { get; set; }
        public bool ready_pushes_notification { get; set; }
        public bool ready_sounds_alert { get; set; }
        public string serial_number { get; set; }
        public bool single_outlet_mode { get; set; }
        public string target_firmware_version { get; set; }
        public int target_temperature { get; set; }
        public int temperature_units { get; set; }
        public int time_remaining { get; set; }
        public bool timer_enabled { get; set; }
        public bool timer_ends_shower { get; set; }
        public int timer_length { get; set; }
        public bool timer_sounds_alert { get; set; }
        public int timezone_offset { get; set; }
        public string token { get; set; }
        public Capability[] capabilities { get; set; }
        public OutletIcon[] outlets { get; set; }
        public Preset[] presets { get; set; }
    }

    public sealed class Capability
    {
        public string name { get; set; }
    }

    public sealed class Preset
    {
        public string greeting { get; set; }
        public int position { get; set; }
        public bool ready_pauses_water { get; set; }
        public bool ready_pushes_notification { get; set; }
        public bool ready_sounds_alert { get; set; }
        public int target_temperature { get; set; }
        public bool timer_enabled { get; set; }
        public bool timer_ends_shower { get; set; }
        public int timer_length { get; set; }
        public bool timer_sounds_alert { get; set; }
        public string title { get; set; }
        public Outlet[] outlets { get; set; }
    }

    public class Outlet
    {
        public int position { get; set; }
        public bool active { get; set; }
    }

    public sealed class OutletIcon: Outlet
    {
        public int icon_index { get; set; }
    }
}