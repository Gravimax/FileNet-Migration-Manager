using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNetMigrationManager
{
    public class DocumentSearchItem
    {
        public DateTime DateCreated { get; set; }

        public string DocumentTitle { get; set; }

        public string FileSize { get; set; }

        public string Id { get; set; }

        public bool IsSelected { get; set; }

        public string Title { get; set; }

        public string Version { get; set; }
    }
}
