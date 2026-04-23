using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.service;
using CloudSpritzers1.src.service.interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.src.service
{
    [TestClass()]
    public class EmployeeServiceTests
    {
        private IEmployeeRepository _employeeRepository;
        private IEmployeeService _employeeService;

        [TestInitialize]
        public void Setup()
        {
            _employeeRepository = Substitute.For<IEmployeeRepository>();
            _employeeService = new EmployeeService(_employeeRepository);

            var employees = new List<Employee>
            {
                new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN),
                new Employee(2, "Elena Radu", "elena@test.com", EmployeeDepartment.HR)
            };
            _employeeRepository.GetAll().Returns(employees);
        }

        [TestMethod()]
        public void GetAllEmployees_ReturnsAllEntities()
        {
            var result = _employeeService.GetAllEmployees();

            
            Assert.AreEqual(2, result.Count); 
             _employeeRepository.Received(1).GetAll();
        }

        [TestMethod()]
        public void CreateNewEmployee_WithValidData_CallsRepository()
        {
            int id = 3;
            string name = "Cristi Dan";
            string email = "cristi@test.com";
            string dept = "SECURITY";

            _employeeService.CreateNewEmployee(id, name, email, dept);

            _employeeRepository.Received(1).CreateNewEntity(Arg.Is<Employee>(e => e.EmployeeId == id)); 
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_NullEmployee_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                _employeeService.ValidateEmployeeIntegrity(null!));
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_EmptyName_ThrowsArgumentException()
        {
            var invalidEmployee = new Employee(4, "", "test@test.com", EmployeeDepartment.ADMIN);

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(invalidEmployee));
            StringAssert.Contains("Name cannot be null or empty", ex.Message); 
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_InvalidDepartment_ThrowsArgumentException()
        {
            
            var employee = new Employee(5, "Name", "email@test.com", (EmployeeDepartment)999);

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(employee)); 
            StringAssert.Contains("Invalid group", ex.Message); 
        }

        [TestMethod()]
        public void DeleteEmployeeById_CallsRepository()
        {
            
            _employeeService.DeleteEmployeeById(1);

            _employeeRepository.Received(1).DeleteById(1); 
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_DuplicateEmployee_ThrowsArgumentException()
        {
            var existingEmployee = new Employee(1, "Andrei Muresan", "andrei@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository.GetAll().Returns(new List<Employee> { existingEmployee }); 

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(existingEmployee));

            StringAssert.Contains("Employee already exists", ex.Message); 
        }

        [TestMethod()]
        public void CreateNewEmployee_InvalidDepartmentString_ThrowsArgumentException()
        {
            int id = 10;
            string name = "Cristi Dan";
            string email = "cristi@test.com";
            string invalidDept = "NON_EXISTENT_DEPT"; 

            Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.CreateNewEmployee(id, name, email, invalidDept));
        }

        [TestMethod()]
        public void GetEmployeeById_ExistingId_ReturnsEmployeeFromRepository()
        {
            var expectedEmployee = new Employee(1, "Test Name", "test@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository.GetById(1).Returns(expectedEmployee);

            var result = _employeeService.GetEmployeeById(1);

            Assert.AreEqual(expectedEmployee, result);
            _employeeRepository.Received(1).GetById(1);
        }

        [TestMethod()]
        public void UpdateEmployeeById_CallsRepositoryWithCorrectData()
        {
            var employeeToUpdate = new Employee(1, "Updated Name", "email@test.com", EmployeeDepartment.HR);

            _employeeService.UpdateEmployeeById(1, employeeToUpdate);

            _employeeRepository.Received(1).UpdateById(1, employeeToUpdate);
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_EmptyEmail_ThrowsArgumentException()
        {
            var employeeWithEmptyEmail = new Employee(1, "Name", "", EmployeeDepartment.ADMIN);

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(employeeWithEmptyEmail));

            StringAssert.Contains("Email cannot be null or empty", ex.Message);
        }

        [TestMethod()]
        public void ValidateEmployeeIntegrity_EmptyDepartmentName_ThrowsArgumentException()
        {
            var employeeWithEmptyDept = new Employee(1, "Name", "test@test.com", (EmployeeDepartment)999);

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _employeeService.ValidateEmployeeIntegrity(employeeWithEmptyDept));

            StringAssert.Contains("Invalid group", ex.Message);
        }

    }
}