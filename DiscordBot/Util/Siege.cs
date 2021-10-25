using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Util
{
    public class Siege
    {
        private string _location;
        private DateTime _time;

        public Siege(string location,int hour, int minute)
        {
            _location = location;
            _time = DateTime.Today + new TimeSpan(hour,minute,0);
        }

        public string Location { get => _location; set => _location = value; }
        public DateTime Time { get => _time; set => _time = value; }
    }
}
