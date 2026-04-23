using CloudSpritzers1.Src.Model.Employee;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1Tests.Src.MockClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass()]
    public class EmployeeRepositoryTests
    {
        private IEmployeeRepository? employeeRepository;

        [TestInitialize]
        public void Setup()
        {
            employeeRepository = new InMemoryEmployeeRepository();
        }

        [TestMethod()]
        public void Add_ValidEmployee_ReturnsCorrectId()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);

            int id = employeeRepository!.CreateNewEntity(employee);

            Assert.AreEqual(1, id);
        }

        [TestMethod()]
        public void GetById_ExistingEmployee_ReturnsCorrectEmployee()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);
            employeeRepository!.CreateNewEntity(employee);

            var result = employeeRepository.GetById(1);

            Assert.AreEqual(employee.RetrieveConfiguredDisplayFullNameForBot(), result.RetrieveConfiguredDisplayFullNameForBot());
        }

        [TestMethod()]
        public void GetById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() =>
                employeeRepository!.GetById(999));
        }

        [TestMethod()]
        public void Add_NullEmployee_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                employeeRepository!.CreateNewEntity(null!));
        }

        [TestMethod()]
        public void DeleteById_ExistingId_Succeeds()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);
            employeeRepository!.CreateNewEntity(employee);

            employeeRepository.DeleteById(1);

            Assert.AreEqual(0, employeeRepository.GetAll().Count());
        }

        [TestMethod()]
        public void UpdateById_ExistingId_UpdatesDataCorrectly()
        {
            var employee = new Employee(1, "Old Name", "old@test.com", EmployeeDepartment.HR);
            employeeRepository!.CreateNewEntity(employee);

            var updatedEmployee = new Employee(1, "New Name", "new@test.com", EmployeeDepartment.ADMIN);

            employeeRepository.UpdateById(1, updatedEmployee);
            var result = employeeRepository.GetById(1);

            Assert.AreEqual("New Name", result.RetrieveConfiguredDisplayFullNameForBot());

            Assert.AreEqual(EmployeeDepartment.ADMIN.ToString(), result.GetDepartmentName());
        }

        [TestMethod()]
        public void UpdateById_NullEmployee_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
                employeeRepository!.UpdateById(1, null!));
        }

        [TestMethod()]
        public void UpdateById_NonExistingId_ThrowsKeyNotFoundException()
        {
            var updatedEmployee = new Employee(999, "No Body", "none@test.com", EmployeeDepartment.ADMIN);

            Assert.ThrowsExactly<KeyNotFoundException>(() =>
                employeeRepository!.UpdateById(999, updatedEmployee));
        }

        [TestMethod()]
        public void DeleteById_NonExistingId_ThrowsKeyNotFoundException()
        {
            int nonExistingId = 999;

            var exception = Assert.ThrowsExactly<KeyNotFoundException>(() =>
                employeeRepository!.DeleteById(nonExistingId));

            Assert.AreEqual($"Employee with id {nonExistingId} not found.", exception.Message);
        }

        [TestMethod]
        public void GetById_ReturnsCorrectEmployeeId()
        {
            var employee = new Employee(1, "John Doe", "john@test.com", EmployeeDepartment.ADMIN);

            employeeRepository!.CreateNewEntity(employee);

            var result = employeeRepository.GetById(1);

            Assert.AreEqual(
                employee.RetrieveUniqueDatabaseIdentifierForBot(),
                result.RetrieveUniqueDatabaseIdentifierForBot()
            );
        }
    }
}