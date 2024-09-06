﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEBackendProject.Models
{
    public class Ship : BaseModel
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } // Unique identifier for the ship

        [Required]
        [StringLength(255)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(-180, 180)]
        [Column("longitude")]
        public double Longitude { get; set; }

        [Required]
        [Range(-90, 90)]
        [Column("latitude")]
        public double Latitude { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        [Column("velocity")]
        public double Velocity { get; set; }

        public ICollection<UserShip> UserShips { get; set; } = new List<UserShip>();
    }
}
