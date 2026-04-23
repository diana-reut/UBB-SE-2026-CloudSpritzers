using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.repository.database;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.repository
{
    public class EmployeeRepository : DatabaseRepository<int, Employee>, IRepository<int, Employee>, IEmployeeRepository
    {
        public int CreateNewEntity(Employee employeeEntity)
        {
            if (employeeEntity == null)
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");

            string insertQuery = "INSERT INTO Employee " +
                "(name, email, group) " +
                "OUTPUT INSERTED.employee_id " +
                "VALUES (@name, @email, @group)";

            SqlCommand sqlCommand = new SqlCommand(insertQuery);


            sqlCommand.Parameters.AddWithValue("@name", employeeEntity.RetrieveConfiguredDisplayFullNameForBot());
            sqlCommand.Parameters.AddWithValue("@email", employeeEntity.RetrieveConfiguredEmailAddressForBotContact());
            sqlCommand.Parameters.AddWithValue("@group", employeeEntity.GetDepartmentName());


            int identificationNumber = base.Add(sqlCommand, employeeEntity);
            return identificationNumber;
        }

        public void DeleteById(int identificationNumber)
        {
            string deleteQuery = "DELETE FROM Employee WHERE employee_id = @id";
            SqlCommand sqlCommand = new SqlCommand(deleteQuery);
            sqlCommand.Parameters.AddWithValue("@id", identificationNumber);

            base.DeleteById(identificationNumber, sqlCommand);
        }

        public IEnumerable<Employee> GetAll()
        {
            string selectAllQuery = "SELECT * FROM Employee";
            SqlCommand sqlCommand = new SqlCommand(selectAllQuery);
            return base.GetAll(sqlCommand);
        }

        public Employee GetById(int identificationNumber)
        {
            string selectByIdQuery = "SELECT * FROM Employee WHERE employee_id = @id";
            SqlCommand sqlCommand = new SqlCommand(selectByIdQuery);
            sqlCommand.Parameters.AddWithValue("@id", identificationNumber);

            Employee foundEmployee = base.GetById(identificationNumber, sqlCommand);

            if (foundEmployee == null)
                throw new KeyNotFoundException($"Employee with id {identificationNumber} was not found.");

            return foundEmployee;
        }

        public void UpdateById(int identificationNumber, Employee employeeEntity)
        {
            if (employeeEntity == null)
                throw new ArgumentNullException(nameof(employeeEntity), "Employee cannot be null.");

            string updateQuery = "UPDATE Employee SET " +
                "name = @name, " +
                "email = @email " +
                "group = @group " +
                "WHERE employee_id = @id";

            SqlCommand sqlCommand = new SqlCommand(updateQuery);


            sqlCommand.Parameters.AddWithValue("@id", identificationNumber);
            sqlCommand.Parameters.AddWithValue("@name", employeeEntity.RetrieveConfiguredDisplayFullNameForBot());
            sqlCommand.Parameters.AddWithValue("@email", employeeEntity.RetrieveConfiguredEmailAddressForBotContact());
            sqlCommand.Parameters.AddWithValue("@group", employeeEntity.GetDepartmentName());



            base.UpdateById(identificationNumber, sqlCommand, employeeEntity);
        }

        protected override int GetEntityId(Employee employeeEntity)
        {
            return employeeEntity.EmployeeId;
        }

        protected override Employee MapRowToEntity(SqlDataReader sqlDataReader)
        {
            int employeeIdentificationNumber = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("employee_id"));
            string employeeFullName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("name"));
            string employeeEmailAddress = sqlDataReader.GetString(sqlDataReader.GetOrdinal("email"));
            string departmentName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("group"));

            EmployeeDepartment departmentEnum = (EmployeeDepartment)Enum.Parse(typeof(EmployeeDepartment), departmentName);

            return new Employee(employeeIdentificationNumber, employeeFullName, employeeEmailAddress, departmentEnum);
        }
    }
}
