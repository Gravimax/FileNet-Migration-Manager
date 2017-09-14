using System;

namespace FileNetMigrationManager
{
    public delegate void CancelOperationEventHandler(object sender, EventArgs e);
    public delegate void GetPasswordEventHandler(object sender, PasswordEventArgs e);
    public delegate void SetPasswordEventHandler(object sender, PasswordEventArgs e);
    public delegate void ExitApplicationEventHandler(object sender, EventArgs e);
    public delegate void SetMessageEventHandler(object sender, SetMessageEventArgs e);
    public delegate void DeleteItemEventHandler(object sender, AddDeleteItemEventArgs e);
    public delegate void AddItemToViewEventHandler(object sender, AddDeleteItemEventArgs e);
}
