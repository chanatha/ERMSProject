using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERMS.Models
{
    public class WorkTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "To Do";

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        [FutureDate(ErrorMessage = "Due date must be in the future")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Project is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        [ValidateNever]
        public Project Project { get; set; }

        [Display(Name = "Assigned Employee")]
        public int? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        [ValidateNever]
        public Employee Employee { get; set; }

    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is DateTime date && date > DateTime.Today;
        }
    }
}