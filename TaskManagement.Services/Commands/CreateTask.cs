using System.ComponentModel.DataAnnotations;
using TaskManagement.Services.DataContext.Entities;

namespace TaskManagement.Services.Commands
{
    public class CreateTask
    {
        public int TaskId { get; set; }

        [Required]
        public string TaskName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Status Status { get; set; }

        public string? AssignedTo { get; set; }
    }
}
