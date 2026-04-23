using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.src.mockClasses
{
    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees = new List<Employee>();

        public int CreateNewEntity(Employee employeeEntity)
        {
            if (employeeEntity == null)
                 throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null."); 

            _employees.Add(employeeEntity);
            return employeeEntity.EmployeeId;
        }

        public void DeleteById(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with id {id} not found.");

            _employees.Remove(employee);
        }

        public IEnumerable<Employee> GetAll() => _employees;

        public Employee GetById(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with id {id} not found.");

            return employee;
        }

        public void UpdateById(int id, Employee employeeEntity)
        {
            if (employeeEntity == null)
                throw new ArgumentNullException(nameof(employeeEntity));

            var index = _employees.FindIndex(e => e.EmployeeId == id);
            if (index == -1)
                throw new KeyNotFoundException();

            _employees[index] = employeeEntity;
        }
    }
}