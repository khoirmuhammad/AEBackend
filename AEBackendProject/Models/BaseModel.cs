using System.ComponentModel.DataAnnotations.Schema;

namespace AEBackendProject.Models
{
    public class BaseModel
    {
        [Column("createdBy")]
        public string CreatedBy { get; set; } = "Administrator";
        [Column("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Column("modifiedBy")]
        public string? ModifiedBy { get; set; }
        [Column("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
