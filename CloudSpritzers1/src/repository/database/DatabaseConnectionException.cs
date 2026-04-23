using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.repository.database
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException(string message) : base(message)
        { }
    }
}
