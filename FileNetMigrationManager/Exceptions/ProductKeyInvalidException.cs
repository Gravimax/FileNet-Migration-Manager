using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNetMigrationManager
{
    public class ProductKeyInvalidException : Exception
    {
        public ProductKeyInvalidException()
        {

        }

        public ProductKeyInvalidException(string message) : base(message)
        {

        }

        public ProductKeyInvalidException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
