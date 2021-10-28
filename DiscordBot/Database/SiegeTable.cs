using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiscordBot.Database
{
    public partial class SiegeTable
    {
        [Key]      
        public string LocationID { get; set; }
        public DateTime Time { get; set; }
        public ulong CreationMessage { get; set; }
    }
}
