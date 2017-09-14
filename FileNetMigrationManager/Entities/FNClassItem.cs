using FileNet.Api.Admin;

namespace FileNetMigrationManager.Entities
{
    public class FNClassItem
    {
        public IClassDefinition ClassDef { get; set; }

        public string DisplayName { get; set; }

        public string SymbolicName { get; set; }

        public int Level { get; set; }
    }
}
