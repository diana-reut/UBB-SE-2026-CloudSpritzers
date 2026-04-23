using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.service.interfaces;

namespace CloudSpritzers1.src.service
{
    public class EmployeeService: IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public Employee GetEmployeeById(int identificationNumber)
        {
            return _employeeRepository.GetById(identificationNumber);
        }

        public int AddEmployee(Employee employeeEntity)
        {
            return _employeeRepository.CreateNewEntity(employeeEntity);
        }

        public void UpdateEmployeeById(int identificationNumber, Employee employeeEntity)
        {
            _employeeRepository.UpdateById(identificationNumber, employeeEntity);
        }

        public void DeleteEmployeeById(int identificationNumber)
        {
            _employeeRepository.DeleteById(identificationNumber);
        }

        public List<Employee> GetAllEmployees()
        {
            return _employeeRepository.GetAll().ToList();
        }

        public void CreateNewEmployee(int identificationNumber, string fullName, string emailAddress, string departmentName)
        {
            EmployeeDepartment departmentEnum = (EmployeeDepartment)Enum.Parse(typeof(EmployeeDepartment), departmentName);
            Employee newEmployee = new Employee(identificationNumber, fullName, emailAddress, departmentEnum);
            ValidateEmployeeIntegrity(newEmployee);
            AddEmployee(newEmployee);
        }

        public void ValidateEmployeeIntegrity(Employee employeeEntity)
        {
            ArgumentNullException.ThrowIfNull(employeeEntity);

            if (this.GetAllEmployees().Contains(employeeEntity))
            {
                throw new ArgumentException("Employee already exists");

            }
            if (string.IsNullOrEmpty(employeeEntity.RetrieveConfiguredDisplayFullNameForBot()))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(employeeEntity.RetrieveConfiguredEmailAddressForBotContact()))
            {

                throw new ArgumentException("Email cannot be null or empty");
            }
            if (string.IsNullOrEmpty(employeeEntity.GetDepartmentName()))
            {
                throw new ArgumentException("Group cannot be null or empty");
            }
            if (!Enum.IsDefined(typeof(EmployeeDepartment), employeeEntity.GetDepartmentName()))
            {
                throw new ArgumentException("Invalid group");
            }
        }
    }
}
