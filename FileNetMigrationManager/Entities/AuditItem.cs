using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileNetMigrationManager.Entities
{
    public class AuditItem
    {
        public string SourceId { get; set; }

        public string SourceVersionId { get; set; }

        public string DestId { get; set; }

        public string DestVersionId { get; set; }
    }
}
