using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientTestManager.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        [Display(AutoGenerateField = false)]
        public ICollection<Test> Tests { get; set; } = new List<Test>();

        [NotMapped]
        public int TestCount => Tests?.Count ?? 0;
    }
}
