using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DiscordBot.Util
{
    public class Siege
    {
        private string _location;
        private DateTime _time;
        private ulong _creationMessage;
        public Siege(string location, DateTime time)
        {
            _location = location;
            _time = time;
            _creationMessage = 0;
        }

        public string Location { get => _location; set => _location = value; }
        public DateTime Time { get => _time; set => _time = value; }
        public ulong CreationMessage { get => _creationMessage; set => _creationMessage = value; }
    }
}
