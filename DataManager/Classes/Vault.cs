using Logging;
using System;
using System.Collections.Generic;
using Windows.Security.Credentials;

namespace DataManager.Classes
{
    public static class Vault
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly PasswordVault PASSWORD_VAULT = new PasswordVault();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static bool LoadCredentials(OnewheelApiCredentials credentials)
        {
            try
            {
                IReadOnlyList<PasswordCredential> pwCredentials = PASSWORD_VAULT.FindAllByResource(credentials.DEVICE_NAME);
                if (pwCredentials.Count >= 1)
                {
                    pwCredentials[0].RetrievePassword();
                    credentials.apiKey = pwCredentials[0].UserName;
                    credentials.apiToken = pwCredentials[0].Password;
                    return !string.IsNullOrEmpty(credentials.apiKey) && !string.IsNullOrEmpty(credentials.apiToken);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to retrieve credentials for: " + credentials.DEVICE_NAME, e);
            }
            return false;
        }

        public static void StoreCredentials(OnewheelApiCredentials credentials)
        {
            // Delete existing password vaults:
            DeleteAllVaults();

            // Store the new password:
            if (!string.IsNullOrEmpty(credentials.apiToken))
            {
                PASSWORD_VAULT.Add(new PasswordCredential(credentials.DEVICE_NAME, credentials.apiKey, credentials.apiToken));
            }
        }

        /// <summary>
        /// Deletes all vaults.
        /// </summary>
        public static void DeleteAllVaults()
        {
            foreach (PasswordCredential item in PASSWORD_VAULT.RetrieveAll())
            {
                PASSWORD_VAULT.Remove(item);
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
