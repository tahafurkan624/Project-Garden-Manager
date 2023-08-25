using UnityEngine;

namespace HelmetMaster.Main
{
    public static class MoneyFormatter 
    {
        private static int GetDigit(int amount, int count = 0)
        {
            if (count == 0 && amount == 0) return 1;
            return amount == 0 ? count : GetDigit(amount / 10, ++count);
        }

        public static string FormatMoney(this int money, char[] suffixes = null, int min = 3)
        {
            double numStr = money;
            var divisor = Mathf.Pow(10, min);
            if (suffixes == null)
            {
                char[] defaultPrefix = {' ', 'K', 'M', 'B', 'T'};
                suffixes = defaultPrefix;
            }

            var length = GetDigit(money);
            for (var i = 0; i < length; i++)
            {
                if (GetDigit((int) numStr) <= min)
                {
                    return $"${numStr:0.##}{suffixes[Mathf.Clamp(i, 0, suffixes.Length - 1)]}";
                }

                numStr /= divisor;
            }

            return "Error";
        }
    }
}