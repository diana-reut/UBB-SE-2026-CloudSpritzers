using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1Tests.src.mockClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.src.repository
{
    [TestClass()]
    public class EmployeeRepositoryTests
    {
        private IEmployeeRepository? _employeeRepository;

        [TestInitialize]
        public void Setup()
        {
            _employeeRepository = new InMemoryEmployeeRepository();
        }

        [TestMethod()]
        public void Add_ValidEmployee_ReturnsCorrectId()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);

            int id = _employeeRepository!.CreateNewEntity(employee);

            Assert.AreEqual(1, id);
        }

        [TestMethod()]
        public void GetById_ExistingEmployee_ReturnsCorrectEmployee()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository!.CreateNewEntity(employee);

            var result = _employeeRepository.GetById(1);

            Assert.AreEqual(employee.RetrieveConfiguredDisplayFullNameForBot(), result.RetrieveConfiguredDisplayFullNameForBot());
        }

        [TestMethod()]
        public void GetById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() =>
                _employeeRepository!.GetById(999));
        }

        [TestMethod()]
        public void Add_NullEmployee_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                _employeeRepository!.CreateNewEntity(null!));
        }

        [TestMethod()]
        public void DeleteById_ExistingId_Succeeds()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);
            _employeeRepository!.CreateNewEntity(employee);

            _employeeRepository.DeleteById(1);

            Assert.AreEqual(0, _employeeRepository.GetAll().Count());
        }

        [TestMethod()]
        public void UpdateById_ExistingId_UpdatesDataCorrectly()
        {
            var employee = new Employee(1, "Old Name", "old@test.com", EmployeeDepartment.HR);
            _employeeRepository!.CreateNewEntity(employee);

            var updatedEmployee = new Employee(1, "New Name", "new@test.com", EmployeeDepartment.ADMIN);

            _employeeRepository.UpdateById(1, updatedEmployee);
            var result = _employeeRepository.GetById(1);

            Assert.AreEqual("New Name", result.RetrieveConfiguredDisplayFullNameForBot());

            Assert.AreEqual(EmployeeDepartment.ADMIN.ToString(), result.GetDepartmentName());
        }

        [TestMethod()]
        public void UpdateById_NullEmployee_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                _employeeRepository!.UpdateById(1, null!));
        }

        [TestMethod()]
        public void UpdateById_NonExistingId_ThrowsKeyNotFoundException()
        {
            var updatedEmployee = new Employee(999, "No Body", "none@test.com", EmployeeDepartment.ADMIN);

            Assert.ThrowsExactly<KeyNotFoundException>(() =>
                _employeeRepository!.UpdateById(999, updatedEmployee));
        }

        [TestMethod()]
        public void DeleteById_NonExistingId_ThrowsKeyNotFoundException()
        {
            int nonExistingId = 999;

            var exception = Assert.ThrowsExactly<KeyNotFoundException>(() =>
                _employeeRepository!.DeleteById(nonExistingId));

            Assert.AreEqual($"Employee with id {nonExistingId} not found.", exception.Message);
        }
    }
}