using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientTestManager.Models
{
    public class Test
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public DateTime TestDate { get; set; }
        public decimal Result { get; set; }
        public bool IsWithinThreshold { get; set; }

        [Display(AutoGenerateField = false)]
        public Patient Patient { get; set; } = null!;

        [NotMapped]
        public string PatientName => Patient.Name;
    }
}
