using CloudSpritzers1.src.model.message;


namespace CloudSpritzers1.src.model.employee
{
    public class Employee : ISender
    {
        private int _employeeId;
        private string _fullName;
        private string _emailAddress;
        private EmployeeDepartment _assignedDepartment;

        public Employee(int employeeIdentificationNumber, string fullName, string emailAddress, EmployeeDepartment assignedDepartment)
        {
            _employeeId = employeeIdentificationNumber;
            _fullName = fullName;
            _emailAddress = emailAddress;
            _assignedDepartment = assignedDepartment;
        }

        public int EmployeeId => _employeeId;
        public string GetDepartmentName() => _assignedDepartment.ToString();

        public string RetrieveConfiguredDisplayFullNameForBot() => _fullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => _emailAddress;


        public int RetrieveUniqueDatabaseIdentifierForBot() => _employeeId;
    }
}
