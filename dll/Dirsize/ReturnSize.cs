using System;

namespace Dirsize
{
    public class ReturnSize
    {
        private const double BYTES_IN_KILOBYTE = 1024.0;
        private static readonly string[] asLibs = { "octets", "Ko", "Mo", "Go", "To", "Po", "Eo" };

        /// <summary>
        /// Converts a size in bytes into a string formatted with the appropriate unit..
        /// </summary>
        /// <param name="lValue">Size in bytes</param>
        /// <returns>Formatted string (ex: "1.5 Mo")</returns>
        public string GetSizeName(long lValue)
        {
            if (lValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lValue), "Size cannot be negative.");
            }

            if (lValue == 0)
                return "0 octet";

            if (lValue == 1)
                return "1 octet";

            double r = Convert.ToDouble(lValue);
            int d = 0;

            while (r >= BYTES_IN_KILOBYTE && d < asLibs.Length - 1)
            {
                r /= BYTES_IN_KILOBYTE;
                d++;
            }

            string sRet;
            if (r == Math.Floor(r))
            {
                sRet = $"{r:F0} {asLibs[d]}";
            }
            else
            {
                sRet = $"{Math.Round(r, 2)} {asLibs[d]}";
            }

            return sRet;
        }

        /// <summary>
        /// Overload allowing you to specify the number of decimal places.
        /// </summary>
        /// <param name="lValue">Size in bytes</param>
        /// <param name="decimals">Number of decimal places (0-3)</param>
        /// <returns>String formatted with the specified number of decimal places</returns>
        public string GetSizeName(long lValue, int decimals)
        {
            if (decimals < 0 || decimals > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(decimals), "The number of decimal places must be between 0 and 3.");
            }

            if (lValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lValue), "Size cannot be negative.");
            }

            if (lValue == 0)
                return "0 octet";

            if (lValue == 1)
                return "1 octet";

            double r = Convert.ToDouble(lValue);
            int d = 0;

            while (r >= BYTES_IN_KILOBYTE && d < asLibs.Length - 1)
            {
                r /= BYTES_IN_KILOBYTE;
                d++;
            }

            string sRet = $"{Math.Round(r, decimals)} {asLibs[d]}";
            return sRet;
        }
    }
}