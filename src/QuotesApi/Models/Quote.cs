namespace QuotesApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class Quote
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [StringLength(20)]
        public string Author { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
    }
}
