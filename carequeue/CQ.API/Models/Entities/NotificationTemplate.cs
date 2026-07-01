using carequeue.CQ.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carequeue.CQ.API.Models.Entities
{
    [Table("NotificationTemplates")]
    public class NotificationTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TemplateId { get; set; }

        [Required]
        public required string Name { get; set; }  

        [Required]
        public TemplateType Type { get; set; }

        [Required]
        [StringLength(250)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty; // Left unbounded for nvarchar(max) templates

        public bool IsActive { get; set; } = true;
    }
}
