using AEBackendProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AEBackendProject.DTO.Ship;

namespace AEBackendProject.DTO.User
{
    public class UserDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
