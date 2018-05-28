using System.Text.RegularExpressions;

namespace Euricom.IoT.Common
{
    public static class Validation
    {
        private static readonly Regex Regex = new Regex("^[a-zA-Z_0-9]+$");
        public static bool ValidateDeviceId(string deviceId)
        {
            return Regex.IsMatch(deviceId);
        }
    }
}
