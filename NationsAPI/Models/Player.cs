using System;
using System.Collections.Generic;

#nullable disable

namespace NationsAPI.Models
{
    public partial class Player : Model
    {
        public string Nick { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Message> MessageFromPlayers { get; set; }
        public virtual ICollection<Message> MessageToPlayers { get; set; }
    }
}
