using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using System.IO;
using Newtonsoft.Json;

namespace UnfollowTheseGuys
{
    class TwitterAuth
    {
        public AuthInfo AuthInfo { get; set; }

        public TwitterAuth()
        {
            GetCredentials();
            Console.WriteLine(AuthInfo.ToString());
            TwitterCredentials.ApplicationCredentials = TwitterCredentials.CreateCredentials(AuthInfo.Token, AuthInfo.TokenSecret, AuthInfo.ApiKey, AuthInfo.ApiSecret);
        }

        private void GetCredentials()
        {
            // Check for saved credentials JSON file
            string fileName = "credentials.json";

            if (File.Exists(fileName))
            {
                try
                {
                    Console.WriteLine("Retrieving saved credentials...");
                    StreamReader reader = new StreamReader(fileName);
                    string json = reader.ReadToEnd();
                    reader.Close();
                    AuthInfo = JsonConvert.DeserializeObject<AuthInfo>(json);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error deserializing credentials.");
                }
            }
            else
            {
                var credentials = CredentialsCreator.GenerateApplicationCredentials(Keys.API_KEY, Keys.API_SECRET);
                string url = CredentialsCreator.GetAuthorizationURL(credentials);
                System.Diagnostics.Process.Start(url); // Open web browser

                Console.WriteLine("Enter PIN:");
                string pin = Console.ReadLine();
                var authorization = CredentialsCreator.GetCredentialsFromVerifierCode(pin, credentials);
                AuthInfo = new AuthInfo(Keys.API_KEY, Keys.API_SECRET, authorization.AccessToken, authorization.AccessTokenSecret);

                Console.WriteLine("Would you like to save these credentials so that you don't have to authenticate next time?\nY/N");
                var reply = Console.ReadKey();
                if (reply.Key.ToString() == "Y")
                {
                    string json = JsonConvert.SerializeObject(AuthInfo);
                    StreamWriter writer = new StreamWriter(fileName);
                    writer.WriteLine(json);
                    writer.Close();
                }
            }
        }
    }

    class AuthInfo
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }

        public AuthInfo(string ApiKey, string ApiSecret, string Token, string TokenSecret)
        {
            this.ApiKey = ApiKey;
            this.ApiSecret = ApiSecret;
            this.Token = Token;
            this.TokenSecret = TokenSecret;
        }

        public override string ToString()
        {
            string output =
                "API Key: " + ApiKey +
                "\nAPI Secret: " + ApiSecret +
                "\nToken: " + Token +
                "\nToken Secret: " + TokenSecret;

            return output;
        }
    }
}
