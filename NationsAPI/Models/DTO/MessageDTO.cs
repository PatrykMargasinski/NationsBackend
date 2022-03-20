using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Models
{
    public class MessageDTO
    {
        public long Id { get; set; }
        public string FromPlayer { get; set; }
        public string ToPlayer { get; set; }
        public string Subject { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool Seen { get; set; }
    }
}
