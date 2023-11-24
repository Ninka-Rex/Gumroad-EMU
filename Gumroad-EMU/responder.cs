using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Gumroad_EMU
{
    internal class responder
    {
        public static string email;
        public static string variant;
        public static string price;
        public static string fee;
        public static string permalink;
        public static string productPermalink;
        public static string licenseKey;
        public static string productName;
        public static string makeResponse(string input)
        {
            // Define the regular expression patterns
            string productIdPattern = @"product_id=([^&]+)";
            string licenseKeyPattern = @"license_key=(.*)";
            string productId = null;
            // Match product ID
            Match productIdMatch = Regex.Match(input, productIdPattern);
            if (productIdMatch.Success)
            {
                productId = productIdMatch.Groups[1].Value;
                // URL decode the product ID
                productId = Uri.UnescapeDataString(productId);

                Logs.Info($"Product ID: {productId}");
            }

            // Match license key
            Match licenseKeyMatch = Regex.Match(input, licenseKeyPattern);
            if (licenseKeyMatch.Success)
            {
                licenseKey = licenseKeyMatch.Groups[1].Value;
                // URL decode the license key
                licenseKey = Uri.UnescapeDataString(licenseKey).Replace("&increment_uses_count=false", "").Replace("&increment_uses_count=true", "").Replace("\r", "");

                Logs.Info($"License Key: {licenseKey}");
            }

            string purchaseJson = $@"{{
""success"": true,
              ""uses"": 0,
              ""purchase"": {{
                ""seller_id"": ""{genID()}"",
                ""product_id"": ""{productId}"",
                ""product_name"": ""{productName}"",
                ""permalink"": ""{permalink}"",
                ""product_permalink"": ""https://sahil.gumroad.com/l/pencil"",
                ""short_product_id"": ""{permalink}"",
                ""email"": ""{email}"",
                ""price"": {price},
                ""gumroad_fee"": {fee},
                ""currency"": ""usd"",
                ""quantity"": 1,
                ""discover_fee_charged"": false,
                ""can_contact"": true,
                ""referrer"": ""direct"",
                ""card"": {{
                  ""visual"": null,
                  ""type"": null,
                  ""bin"": null,
                  ""expiry_month"": null,
                  ""expiry_year"": null
                }},
                ""order_number"": {GenerateRandomNumbers(9)},
                ""sale_id"": ""{genID()}"",
                ""sale_timestamp"": ""{GetCurrentTimeISO8601()}"",
                ""purchaser_id"": ""{GenerateRandomNumbers(13)}"",
                ""variants"": ""{variant}"",
                ""license_key"": ""{licenseKey}"",
                ""ip_country"": ""United States"",
                ""is_gift_receiver_purchase"": false,
                ""refunded"": false,
                ""disputed"": false,
                ""dispute_won"": false,
                ""id"": ""{genID()}"",
                ""created_at"": ""{GetCurrentTimeISO8601()}"",
                ""custom_fields"": [],
                ""chargebacked"": false
              }}
            }}";
            Logs.Info("Rewrote API request!");
            return purchaseJson;
        }
        public static void setParameters(string mail, string ed, string cents, string perm, string productperm, string name)
        {
            email = mail;
            variant = "(" + ed + ")";
            price = cents;
            permalink = perm;
            productPermalink = productperm;
            productName = name.Replace("%", "%");
            fee = Convert.ToString(Convert.ToInt16(cents) * 0.01);
        }
        private static readonly Random random = new Random();

        public static string genID()
        {
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_";
            const int idLength = 20;

            char[] idChars = new char[idLength];

            for (int i = 0; i < idLength; i++)
            {
                idChars[i] = characters[random.Next(0, characters.Length)];
            }

            return new string(idChars) + "==";
        }
        public static string GetCurrentTimeISO8601()
        {
            var utcNow = DateTime.UtcNow;
            return utcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
        public static string GenerateRandomNumbers(int length)
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(random.Next(0, 10));
            }
            return sb.ToString();
        }
    }
}
