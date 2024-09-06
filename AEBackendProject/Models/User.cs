using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEBackendProject.Models
{
    public class User : BaseModel
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("role")]
        public string Role { get; set; } = string.Empty;

        public ICollection<UserShip> UserShips { get; set; } = new List<UserShip>();
    }
}
