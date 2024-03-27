using System.IO;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Common.AppConfig
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSecretConfig
    {
        private static readonly string Audience_Secret = Appsettings.App(new string[] { "Audience", "Secret" });
        private static readonly string Audience_Secret_File = Appsettings.App(new string[] { "Audience", "SecretFile" });


        /// <summary>
        /// JWT密钥，在配置文件appsettings.json中配置Secret或SecretFile（高优先级）
        /// {"Audience"{ "Secret":"","SecretFile":""}}
        /// </summary>
        public static string Audience_Secret_String => InitAudience_Secret();


        private static string InitAudience_Secret()
        {
            var securityString = DifDBConnOfSecurity(Audience_Secret_File);
            if (!string.IsNullOrEmpty(Audience_Secret_File)&& !string.IsNullOrEmpty(securityString))
            {
                return securityString;
            }
            else
            {
                return Audience_Secret;
            }

        }

        private static string DifDBConnOfSecurity(params string[] conn)
        {
            foreach (var item in conn)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        return File.ReadAllText(item).Trim();
                    }
                }
                catch (System.Exception) { }
            }

            return "";
        }

    }

}
