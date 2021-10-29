using System.Configuration;

namespace Tools
{
    class EnvironmentHelper
    {
        public static bool isProduction()
        {
            if (ConfigurationManager.AppSettings["ENVIRONMENT"] == "PRODUCTION")
            {
                return true;
            }

            return false;
        }

        public static bool isDevelopment()
        {
            if (ConfigurationManager.AppSettings["ENVIRONMENT"] == "DEVELOPMENT")
            {
                return true;
            }

            return false;
        }

        public static bool isQA()
        {
            if (ConfigurationManager.AppSettings["ENVIRONMENT"] == "QA")
            {
                return true;
            }

            return false;
        }
    }
}
