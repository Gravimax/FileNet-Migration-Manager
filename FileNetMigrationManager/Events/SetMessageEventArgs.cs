using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNetMigrationManager
{
    public class SetMessageEventArgs : EventArgs
    {
        public SetMessageEventArgs(string message)
        {
            Message = message;
        }

        public readonly string Message;
    }
}
