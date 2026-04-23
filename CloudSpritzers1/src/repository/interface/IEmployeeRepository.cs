using System.Collections.Generic;
using CloudSpritzers1.Src.Model.Employee;

namespace CloudSpritzers1.Src.Repository
{
    public interface IEmployeeRepository
    {
        int CreateNewEntity(Employee employeeEntity);
        void DeleteById(int identificationNumber);
        IEnumerable<Employee> GetAll();
        Employee GetById(int identificationNumber);
        void UpdateById(int identificationNumber, Employee employeeEntity);
    }
}