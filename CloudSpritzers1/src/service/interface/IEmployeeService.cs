using System.Collections.Generic;
using CloudSpritzers1.Src.Model.Employee;

namespace CloudSpritzers1.Src.Service
{
    public interface IEmployeeService
    {
        Employee GetEmployeeById(int identificationNumber);
        int AddEmployee(Employee employeeEntity);
        void UpdateEmployeeById(int identificationNumber, Employee employeeEntity);
        void DeleteEmployeeById(int identificationNumber);
        List<Employee> GetAllEmployees();
        void CreateNewEmployee(int identificationNumber, string fullName, string emailAddress, string departmentName);
        void ValidateEmployeeIntegrity(Employee employeeEntity);
    }
}