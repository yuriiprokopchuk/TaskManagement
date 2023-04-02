using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Services.DataContext.Entities
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public string TaskName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Status Status { get; set; }

        public string AssignedTo { get; set; }
    }

    public enum Status
    {
        NotStarted,
        InProgress,
        Completed
    }
}
