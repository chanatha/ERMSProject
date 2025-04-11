// Models/Employee.cs
using System.ComponentModel.DataAnnotations;
namespace ERMS.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

    }
}
