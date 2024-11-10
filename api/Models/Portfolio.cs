using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Portfolio")]
    // public class AppUserStock  //what does it mean if you call the table AppUserStock,the Entity framework will take care of the whole work. But the toturial said its probably not a good idea.
    public class Portfolio
    {
        public string AppUserId { get; set; }
        public int StockId { get; set; }
        public AppUser AppUser { get; set; } // why do we need navigation properties? give me an example how to set a navigation property. The tutorial saied having navigation property here is optional. Its ok to only have AppUserId and STcokId and use the onModelCreating to actually do everything. whats the difference for this approach? He said current approach is the conventional.
        public Stock Stock { get; set; }    
    }
}