using Microsoft.Win32;
using System;

namespace FileNetMigrationManager.Registration
{
    internal sealed class ProductKeyManager
    {
        private const string phase = "EB46EFEB-1F49-4111-BA56-67CE7393BC4C";
        private string keyPath = Utilities.GetCurrentDirectory() + "\\activation.key";

        public bool AddNewKey(string key)
        {
            return AddKey(key);
        }

        private bool AddKey(string key)
        {
            if (ValidateKey(key))
            {
                Utilities.WriteTextFile(keyPath, EncryptActivationKey(key));

                return true;
            }
            else
            {
               throw new ProductKeyInvalidException("The activation key entered is invalid");
            }
        }

        public bool CheckKey(out string keyVal)
        {
            if (Utilities.FileExists(keyPath))
            {
                string key = DecryptActivationKey(Utilities.ReadTextFile(keyPath));

                if (ValidateKey(key))
                {
                    keyVal = key;
                    return true;
                }
                else
                {
                    throw new ProductKeyInvalidException("Invlid activation key");
                }
            }
            else
            {
                keyVal = null;
                return false;
            }
        }

        private bool ValidateKey(string key)
        {
            SKGL.SerialKeyConfiguration skgl = new SKGL.SerialKeyConfiguration();
            SKGL.Validate validateKey = new SKGL.Validate(skgl);
            validateKey.secretPhase = phase;
            validateKey.Key = key;

            return validateKey.IsValid;
        }

        private string EncryptActivationKey(string key)
        {
            return StringCipher.Encrypt(key, GetCipyerKeyId());
        }

        private string DecryptActivationKey(string actvKey)
        {
            try
            {
                return StringCipher.Decrypt(actvKey, GetCipyerKeyId());
            }
            catch (Exception ex)
            {
                throw new ProductKeyInvalidException("Invalid activation key", ex);
            }
        }

        private string GetCipyerKeyId()
        {
            RegistryView view = System.Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
            {
                string keyPath = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";
                var prodId = key.OpenSubKey(keyPath).GetValue("ProductId");
                return prodId.ToString();
            }
        }
    }
}
