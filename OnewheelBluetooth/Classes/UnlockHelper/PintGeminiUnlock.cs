using DataManager.Classes;
using Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnewheelBluetooth.Classes.UnlockHelper
{
    public class PintGeminiUnlock : AbstractOnewheelUnlock
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string SERVICE_BASE_URL = "https://app.onewheel.com/wp-json/fm/v2/activation/";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override async Task CalcAndSendResponseAsync(List<byte> serialReadCache, OnewheelBoard onewheel)
        {
            byte[] challenge = serialReadCache.ToArray();
            byte[] response = await CalcResponseAsync(challenge, onewheel);

            await onewheel.WriteBytesAsync(OnewheelCharacteristicsCache.CHARACTERISTIC_UART_SERIAL_WRITE, response);
            Logger.Info("Sent Gemini unlock response to Onewheel challenge.");
        }

        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// Calculates the response for the given challenge.
        /// Based on: https://github.com/OnewheelCommunityEdition/OWCE_App/blob/4fc1923323543db849deccbf2998646cdf1bae31/OWCE/OWCE/OWBoard.cs#L899-L914
        /// </summary>
        /// <param name="challenge">The challenge send by the Onewheel.</param>
        /// <returns>The response for the given challenge.</returns>
        private async Task<byte[]> CalcResponseAsync(byte[] challenge, OnewheelBoard onewheel)
        {
            List<byte> response = new List<byte>(20);
            response.AddRange(CHALLENGE_FIRST_BYTES);

            // Get bytes 3 through to 19 (start 3, length 16)
            byte[] apiKeyArray = new byte[16];
            Buffer.BlockCopy(challenge, 3, apiKeyArray, 0, 16);

            // Convert to hex string.
            string apiKey = Utils.ByteArrayToHexString(apiKeyArray);

            // Fetch the Token from the server:
            return await FetchTokenAsync(apiKey, onewheel);
        }

        /// <summary>
        /// Based on: https://github.com/OnewheelCommunityEdition/OWCE_App/blob/4fc1923323543db849deccbf2998646cdf1bae31/OWCE/OWCE/OWBoard.cs#L981
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="onewheel"></param>
        /// <returns></returns>
        private async Task<byte[]> FetchTokenAsync(string apiKey, OnewheelBoard onewheel)
        {
            string owType = GetOwTypeName(onewheel);
            string deviceName = GetDeviceName(onewheel);
            OnewheelApiCredentials apiCredentials = new OnewheelApiCredentials(deviceName);
            // Check if we have already an API token cached for this device:
            if (Vault.LoadCredentials(apiCredentials) && string.Equals(apiKey, apiCredentials.apiKey))
            {
                return Utils.HexStringToByteArray(apiCredentials.apiToken);
            }

            // Else continue with requesting a new API token:
            apiCredentials.apiKey = apiKey;
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpClient client = new HttpClient())
                {
                    // Match headers as best as possible:
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-us");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic Og==");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip");

                    // Use the IOS user agent:
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Onewheel/0 CFNetwork/1121.2.2 Darwin/19.2.0");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                    // Request unlock key based on board name, board type, and token:
                    Uri uri = BuildUri(deviceName, owType, apiKey);
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            JArray result = JArray.Parse(responseBody);

                            // TODO parse JSON response and store the API token securely:
                            /*if (string.IsNullOrWhiteSpace(owKey.Key))
                            {
                                throw new Exception("No key found.");
                            }

                            await SecureStorage.SetAsync($"board_{deviceName}_key", apiKey);
                            await SecureStorage.SetAsync($"board_{deviceName}_token", owKey.Key);

                            var tokenArray = owKey.Key.StringToByteArray();
                            return tokenArray;*/
                            return null;
                        }
                        else
                        {
                            throw new Exception($"Unexpected response code ({response.StatusCode})");
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        private string GetDeviceName(OnewheelBoard onewheel)
        {
            // TODO use concrete name:
            return "".ToLowerInvariant().Replace("ow", "");
        }

        private string GetOwTypeName(OnewheelBoard onewheel)
        {
            switch (onewheel.TYPE)
            {
                case OnewheelType.ONEWHEEL_PLUS_XR:
                    return "xr";

                case OnewheelType.ONEWHEEL_PINT:
                    return "pint";

                default:
                    return "";
            }
        }

        private Uri BuildUri(string deviceName, string owType, string apiKey)
        {
            Dictionary<string, string> queryDict = new Dictionary<string, string>
            {
                ["owType"] = owType,
                ["apiKey"] = apiKey
            };

            string query = string.Join("&", queryDict.Keys.Select(key => !string.IsNullOrWhiteSpace(queryDict[key]) ? string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(queryDict[key])) : WebUtility.UrlEncode(key)));
            UriBuilder builder = new UriBuilder(SERVICE_BASE_URL + WebUtility.UrlEncode(deviceName))
            {
                Query = query
            };
            return builder.Uri;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
