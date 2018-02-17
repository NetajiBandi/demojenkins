using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class Randomizers
    {
        /// <summary>
        /// The RNG crypto service provider
        /// </summary>
        private static RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();

        /// <summary>
        /// Gets the random number.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The random number</returns>
        public static int GetRandomNumber(int minValue, int maxValue)
        {
            var bytes = new byte[4];
            rngCryptoServiceProvider.GetBytes(bytes);
            var seed = BitConverter.ToInt32(bytes, 0);
            var random = new Random(seed);
            return random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Generates the random date.
        /// </summary>
        /// <returns>The random date</returns>
        public static DateTime GenerateRandomDate()
        {
            var year = GetRandomNumber(1901, DateTime.UtcNow.Year);
            var month = GetRandomNumber(1, DateTime.UtcNow.Month);
            var day = GetRandomNumber(1, DateTime.DaysInMonth(year, month));
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Generates the random date.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="startMonth">The start month.</param>
        /// <param name="endMonth">The end month.</param>
        /// <returns>The random date</returns>
        /// <exception cref="Exception">startMonth can not be higher than endMonth</exception>
        public static DateTime GenerateRandomDate(int year, int startMonth, int endMonth)
        {
            if (startMonth > endMonth)
            {
                throw new Exception("startMonth can not be higher than endMonth");
            }

            var month = GetRandomNumber(startMonth, endMonth);
            var day = GetRandomNumber(1, DateTime.DaysInMonth(year, month));
            var randomDate = new DateTime(year, month, day).AddHours(GetRandomNumber(1, 24));

            return randomDate;
        }
    }
}
