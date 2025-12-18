using PatientTestManager.Data;
using PatientTestManager.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace PatientTestManager.ViewModels
{
    public class MainViewModel
    {
        private readonly AppDbContext _db = new();

        public ObservableCollection<Patient> Patients { get; set; }
        public Patient? SelectedPatient { get; set; }

        public MainViewModel()
        {
            _db.Database.EnsureCreated();
            Patients = new ObservableCollection<Patient>(_db.Patients.Include(p => p.Tests));
        }

        public void AddPatient(string name, DateTime dob, string gender)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Name is required");

            var patient = new Patient
            {
                Name = name,
                DateOfBirth = dob,
                Gender = gender
            };

            _db.Patients.Add(patient);
            _db.SaveChanges();
            Patients.Add(patient);
        }

        public void UpdatePatient(Patient patient)
        {
            if (patient == null) return;
            if (string.IsNullOrWhiteSpace(patient.Name))
                throw new Exception("Name is required");

            _db.Patients.Update(patient);
            _db.SaveChanges();
        }

        public void DeletePatient(Patient patient)
        {
            if (patient == null) return;

            _db.Patients.Remove(patient);
            _db.SaveChanges();
            Patients.Remove(patient);
        }

        // ---------------- TEST CRUD ----------------
        public void AddTest(Patient patient, string testName, DateTime testDate, decimal result, bool isWithin)
        {
            if (patient == null)
                throw new Exception("Select patient first");

            if (string.IsNullOrWhiteSpace(testName))
                throw new Exception("Test name required");

            var test = new Test
            {
                PatientId = patient.Id,
                TestName = testName,
                TestDate = testDate,
                Result = result,
                IsWithinThreshold = isWithin
            };

            _db.Tests.Add(test);
            _db.SaveChanges();

            // Reload the tests collection from the database to get the updated list
            _db.Entry(patient).Collection(p => p.Tests).Load();
        }
        public void UpdateTest(Test test)
        {
            if (test == null)
                throw new Exception("Test cannot be null");

            if (string.IsNullOrWhiteSpace(test.TestName))
                throw new Exception("Test name required");

            // Attach the test to the context if it's not being tracked
            var entry = _db.Entry(test);
            if (entry.State == EntityState.Detached)
            {
                _db.Tests.Attach(test);
                entry.State = EntityState.Modified;
            }
            else
            {
                _db.Tests.Update(test);
            }

            _db.SaveChanges();
        }
        public void DeleteTest(Patient patient, Test test)
        {
            if (patient == null || test == null)
                throw new Exception("Patient and test cannot be null");

            _db.Tests.Remove(test);
            _db.SaveChanges();

            // Reload the tests collection from the database to get the updated list
            _db.Entry(patient).Collection(p => p.Tests).Load();
        }
    }
}
