using System;

namespace Dirsize
{
    public class ReturnSize
    {
        public string GetSizeName(long lValue)
        {
            if ((lValue == 0) | (lValue == 1))
                return lValue.ToString() + " octet";
            else if (lValue < 0)
                return string.Empty;
            string[] asLibs = new[] { "octets", "Ko", "Mo", "Go", "To" };
            double r = Convert.ToDouble(lValue);
            int d = asLibs.GetLowerBound(0);
            while (r > 1024)
            {
                _ = !((Convert.ToInt64(r) & 1023) == 0);
                r /= 1024;
                d += 1;
            }
            string sRet = string.Format("{0} {1}", Math.Round(r, 3), asLibs[d]);
            return sRet;
        }
    }
}
