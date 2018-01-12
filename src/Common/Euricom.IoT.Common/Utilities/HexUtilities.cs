using System;

namespace Euricom.IoT.Common.Utilities
{
    public static class HexUtilities
    {
        public static string ToHexString(int hex)
        {
            int value = Convert.ToInt32(hex);
            string stringValue = Char.ConvertFromUtf32(value);
            return stringValue;
        }

        public static string ToHexString(byte[] commands)
        {
            string hex = BitConverter.ToString(commands);
            hex = hex.Replace("-", "");
            return hex;
        }
    }
}
