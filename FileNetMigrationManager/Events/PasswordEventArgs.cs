namespace FileNetMigrationManager
{
    public class PasswordEventArgs
    {
        public PasswordEventArgs(string password)
        {
            Password = password;
        }

        public readonly string Password;
    }
}
