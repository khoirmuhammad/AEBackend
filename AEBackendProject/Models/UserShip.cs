using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AEBackendProject.Models
{
    public class UserShip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("userId")]
        public Guid UserId { get; set; }

        [Column("shipId")]
        public Guid ShipId { get; set; }

        public User User { get; set; }
        public Ship Ship { get; set; }
    }
}
