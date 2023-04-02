using System.ComponentModel.DataAnnotations;
using TaskManagement.Services.DataContext.Entities;

namespace TaskManagement.Services.Commands
{
    public class UpdateTask
    {
        public int TaskId { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public string UpdatedBy { get; set; }
    }
}
