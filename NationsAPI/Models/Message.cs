using System;
using System.Collections.Generic;

#nullable disable

namespace NationsAPI.Models
{
    public partial class Message : Model
    {
        public long ToPlayerId { get; set; }
        public long? FromPlayerId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool Seen { get; set; }
        public virtual Player FromPlayer { get; set; }
        public virtual Player ToPlayer { get; set; }
    }
}
