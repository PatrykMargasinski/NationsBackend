using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NationsAPI.Models
{
    public abstract class Model
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public String IdToString() {
            return "[id=" + this.Id + "]";
        }
    }
}
