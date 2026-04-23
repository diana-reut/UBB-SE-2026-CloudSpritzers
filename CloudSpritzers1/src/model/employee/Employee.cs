using CloudSpritzers1.Src.Model.Message;

namespace CloudSpritzers1.Src.Model.Employee
{
    public class Employee : ISender
    {
        private int employeeId;
        private string fullName;
        private string emailAddress;
        private EmployeeDepartment assignedDepartment;

        public Employee(int employeeIdentificationNumber, string fullName, string emailAddress, EmployeeDepartment assignedDepartment)
        {
            employeeId = employeeIdentificationNumber;
            this.fullName = fullName;
            this.emailAddress = emailAddress;
            this.assignedDepartment = assignedDepartment;
        }

        public int EmployeeId => employeeId;
        public string GetDepartmentName() => assignedDepartment.ToString();

        public string RetrieveConfiguredDisplayFullNameForBot() => fullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => emailAddress;
        public int RetrieveUniqueDatabaseIdentifierForBot() => employeeId;
    }
}
