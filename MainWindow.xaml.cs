using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientTestManager.Data;
using PatientTestManager.Models;
using PatientTestManager.ViewModels;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PatientTestManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
        }

        private void ClearInputs()
        {
            NameTextBox.Text = "";
            DobPicker.SelectedDate = null;
            GenderComboBox.SelectedIndex = 0;
        }

        private void PatientGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Tests")
            {
                e.Cancel = true;
            }
        }

        // ---------------- PATIENT CRUD ----------------

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = NameTextBox.Text;
                var dob = DobPicker.SelectedDate;
                var gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("Name is required");

                if (dob == null)
                    throw new Exception("Date of birth is required");

                if (string.IsNullOrWhiteSpace(gender))
                    throw new Exception("Gender is required");

                _vm.AddPatient(name, dob.Value, gender);

                // Clear inputs
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_vm.SelectedPatient == null)
                    throw new Exception("Please select a patient to update");

                // Use user input from textboxes instead of auto values
                var name = NameTextBox.Text;
                var dob = DobPicker.SelectedDate;
                var gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("Name is required");
                if (dob == null)
                    throw new Exception("Date of birth is required");
                if (string.IsNullOrWhiteSpace(gender))
                    throw new Exception("Gender is required");

                // Update the selected patient
                _vm.SelectedPatient.Name = name;
                _vm.SelectedPatient.DateOfBirth = dob.Value;
                _vm.SelectedPatient.Gender = gender;

                _vm.UpdatePatient(_vm.SelectedPatient);

                MessageBox.Show("Patient updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedPatient == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this patient?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _vm.DeletePatient(_vm.SelectedPatient);
                ClearInputs();
            }
        }

        private void PatientGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vm.SelectedPatient != null)
            {
                // Populate the input controls with selected patient data
                NameTextBox.Text = _vm.SelectedPatient.Name;
                DobPicker.SelectedDate = _vm.SelectedPatient.DateOfBirth;

                // Set the gender in ComboBox
                foreach (ComboBoxItem item in GenderComboBox.Items)
                {
                    if (item.Content?.ToString() == _vm.SelectedPatient.Gender)
                    {
                        GenderComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // ---------------- TEST CRUD ----------------

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_vm.SelectedPatient == null)
                    throw new Exception("Please select a patient first");

                var testName = TestNameTextBox.Text;
                var testDate = TestDatePicker.SelectedDate;
                var resultText = TestResultTextBox.Text;
                var isWithinThreshold = IsWithinThresholdCheckBox.IsChecked ?? false;

                if (string.IsNullOrWhiteSpace(testName) || testName == "Test Name")
                    throw new Exception("Test name is required");

                if (testDate == null)
                    throw new Exception("Test date is required");

                if (string.IsNullOrWhiteSpace(resultText) || resultText == "Result")
                    throw new Exception("Test result is required");

                if (!decimal.TryParse(resultText, out decimal result))
                    throw new Exception("Test result must be a valid number");

                _vm.AddTest(_vm.SelectedPatient, testName, testDate.Value, result, isWithinThreshold);

                // Refresh the TestGrid to show the new test
                TestGrid.Items.Refresh();

                // Refresh the PatientGrid to update the test count
                PatientGrid.Items.Refresh();

                // Clear test inputs
                ClearTestInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var test = TestGrid.SelectedItem as Test;
                if (test == null)
                    throw new Exception("Please select a test to update");

                var testName = TestNameTextBox.Text;
                var testDate = TestDatePicker.SelectedDate;
                var resultText = TestResultTextBox.Text;
                var isWithinThreshold = IsWithinThresholdCheckBox.IsChecked ?? false;

                if (string.IsNullOrWhiteSpace(testName) || testName == "Test Name")
                    throw new Exception("Test name is required");

                if (testDate == null)
                    throw new Exception("Test date is required");

                if (string.IsNullOrWhiteSpace(resultText) || resultText == "Result")
                    throw new Exception("Test result is required");

                if (!decimal.TryParse(resultText, out decimal result))
                    throw new Exception("Test result must be a valid number");

                test.TestName = testName;
                test.TestDate = testDate.Value;
                test.Result = result;
                test.IsWithinThreshold = isWithinThreshold;

                _vm.UpdateTest(test);

                // Refresh the TestGrid to show the new test
                TestGrid.Items.Refresh();

                // Refresh the PatientGrid to update the test count
                PatientGrid.Items.Refresh();

                MessageBox.Show("Test updated successfully.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_vm.SelectedPatient == null)
                    throw new Exception("No patient selected");

                var test = TestGrid.SelectedItem as Test;
                if (test == null)
                    throw new Exception("Please select a test to delete");

                var result = MessageBox.Show("Are you sure you want to delete this test?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _vm.DeleteTest(_vm.SelectedPatient, test);

                    // Refresh the TestGrid to show the new test
                    TestGrid.Items.Refresh();

                    // Refresh the PatientGrid to update the test count
                    PatientGrid.Items.Refresh();

                    ClearTestInputs();
                    MessageBox.Show("Test deleted successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TestGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var test = TestGrid.SelectedItem as Test;
            if (test != null)
            {
                // Populate test input controls with selected test data
                TestNameTextBox.Text = test.TestName;
                TestDatePicker.SelectedDate = test.TestDate;
                TestResultTextBox.Text = test.Result.ToString();
                IsWithinThresholdCheckBox.IsChecked = test.IsWithinThreshold;
            }
        }

        private void ClearTestInputs()
        {
            TestNameTextBox.Text = "Test Name";
            TestDatePicker.SelectedDate = null;
            TestResultTextBox.Text = "Result";
            IsWithinThresholdCheckBox.IsChecked = false;
        }

        private void TestGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Patient")
            {
                e.Cancel = true;
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            // Validate date pickers
            if (!ReportFromDatePicker.SelectedDate.HasValue || !ReportToDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both From and To dates.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime fromDate = ReportFromDatePicker.SelectedDate.Value.Date;
            DateTime toDate = ReportToDatePicker.SelectedDate.Value.Date.AddDays(1).AddTicks(-1); // End of day

            if (fromDate > toDate)
            {
                MessageBox.Show("From date cannot be later than To date.", "Invalid Range", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    string connectionString = context.Database.GetConnectionString();

                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string sql = @"
                                SELECT 
                                    p.Id AS PatientId,
                                    p.Name AS PatientName,
                                    COUNT(t.Id) AS TotalTests,
                                    ISNULL(
                                        CAST(
                                            SUM(CASE WHEN t.IsWithinThreshold = 1 THEN 1 ELSE 0 END) * 100.0 
                                            / NULLIF(COUNT(t.Id), 0) 
                                        AS DECIMAL(5,2)
                                        ), 
                                        0
                                    ) AS PercentageWithinThreshold
                                FROM Patients p
                                LEFT JOIN Tests t 
                                    ON p.Id = t.PatientId 
                                    AND t.TestDate >= @FromDate 
                                    AND t.TestDate <= @ToDate
                                GROUP BY p.Id, p.Name
                                ORDER BY p.Name";

                        using (var command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@FromDate", fromDate);
                            command.Parameters.AddWithValue("@ToDate", toDate);

                            var dataTable = new DataTable();
                            using (var adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(dataTable);
                            }

                            // Bind to ReportGrid
                            ReportGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report:\n{ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}