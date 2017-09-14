using System;

namespace FileNetMigrationManager
{
    public class AddDeleteItemEventArgs : EventArgs
    {
        public AddDeleteItemEventArgs(PropertiesMap item)
        {
            this.Item = item;
        }

        public readonly PropertiesMap Item;
    }
}
