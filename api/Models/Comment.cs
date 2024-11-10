using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api
{
    [Table("Comment")]
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public int? StockID { get; set; }
        public Stock? Stock { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}