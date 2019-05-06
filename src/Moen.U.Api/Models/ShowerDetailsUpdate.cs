using Newtonsoft.Json;

namespace Moen.U.Api.Models
{
    internal sealed class ShowerDetailsUpdateRequest
    {
        public ShowerDetailsUpdate shower { get; set; }
    }

    public sealed class ShowerDetailsUpdate
    {
        public int active_preset { get; set; }
        //public string api_server { get; set; }
        //public object battery_level { get; set; }
        //public string channel { get; set; }
        public string current_firmware_version { get; set; }
        public int current_temperature { get; set; }
        //public string current_firmware_version { get; set; }
        public int display_brightness { get; set; }
        public int language { get; set; }
        public int max_temp { get; set; }
        //public string mode { get; set; }
        public string name { get; set; }
        public bool off_on_idle { get; set; }
        //public string preset_greeting { get; set; }
        //public string preset_title { get; set; }
        public bool ready_pauses_water { get; set; }
        public bool ready_pushes_notification { get; set; }
        public bool ready_sounds_alert { get; set; }
        //public string serial_number { get; set; }
        //public bool single_outlet_mode { get; set; }
        //public string target_firmware_version { get; set; }
        public int target_temperature { get; set; }
        public int temperature_units { get; set; }
        public int time_remaining { get; set; }
        public bool timer_enabled { get; set; }
        public bool timer_ends_shower { get; set; }
        public int timer_length { get; set; }
        public bool timer_sounds_alert { get; set; }
        public int timezone_offset { get; set; }
        //public string token { get; set; }
        //public Capability[] capabilities { get; set; }
        public OutletIcon[] outlets { get; set; }
        public Preset[] presets { get; set; }



        public string source { get; set; } = "ios";
        public string current_mode { get; set; } = "off";
        public bool active { get; set; } = true;
        
        [JsonIgnore()]
        public string serial_number { get; internal set; }
    }
}
