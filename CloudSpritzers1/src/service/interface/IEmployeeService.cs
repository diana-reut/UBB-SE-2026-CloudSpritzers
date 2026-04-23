using CloudSpritzers1.src.model.employee;
using System.Collections.Generic;

namespace CloudSpritzers1.src.service
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