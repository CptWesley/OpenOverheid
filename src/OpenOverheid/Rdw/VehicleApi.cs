using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenOverheid.Rdw
{
    /// <summary>
    /// The API for getting vehicle information.
    /// </summary>
    /// <seealso cref="OverheidApi" />
    public class VehicleApi : OverheidApi
    {
        private const string ExaminationExpirationUrl = "https://opendata.rdw.nl/resource/vkij-7mwc.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApi"/> class.
        /// </summary>
        public VehicleApi()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApi"/> class.
        /// </summary>
        /// <param name="client">The client used for webrequests.</param>
        public VehicleApi(HttpClient client)
            : base(client)
        {
        }

        /// <summary>
        /// Gets the examination expiration asynchronously.
        /// </summary>
        /// <param name="licensePlate">The license plate.</param>
        /// <returns>The date on which the examination expires.</returns>
        public async Task<DateTime> GetExaminationExpirationAsync(string licensePlate)
        {
            licensePlate = NormalizeLicensePlate(licensePlate);
            JsonDocument response = await RequestAsync($"{ExaminationExpirationUrl}?&$where=kenteken='{licensePlate}'");
            return GetExaminationExpiration(response);
        }

        /// <summary>
        /// Gets the examination expiration synchronously.
        /// </summary>
        /// <param name="licensePlate">The license plate.</param>
        /// <returns>The date on which the examination expires.</returns>
        public DateTime GetExaminationExpiration(string licensePlate)
        {
            licensePlate = NormalizeLicensePlate(licensePlate);
            JsonDocument response = Request($"{ExaminationExpirationUrl}?&$where=kenteken='{licensePlate}'");
            return GetExaminationExpiration(response);
        }

        /// <summary>
        /// Gets the examination expirations synchronously.
        /// </summary>
        /// <returns>Get all examination expiration dates of registered vehicles.</returns>
        public Dictionary<string, DateTime> GetExaminationExpirations()
            => GetExaminationExpirations(int.MaxValue, 0);

        /// <summary>
        /// Gets the examination expirations synchronously.
        /// </summary>
        /// <param name="limit">The number of entries requested.</param>
        /// <param name="offset">The offset of entries where to start for pagination.</param>
        /// <returns>Get all examination expiration dates of registered vehicles.</returns>
        public Dictionary<string, DateTime> GetExaminationExpirations(int limit, int offset)
        {
            JsonDocument response = Request($"{ExaminationExpirationUrl}?$limit={limit}&$offset={offset}");
            return GetExaminationExpirations(response);
        }

        /// <summary>
        /// Gets the examination expirations asynchronously.
        /// </summary>
        /// <param name="limit">The number of entries requested.</param>
        /// <param name="offset">The offset of entries where to start for pagination.</param>
        /// <returns>Get all examination expiration dates of registered vehicles.</returns>
        public async Task<Dictionary<string, DateTime>> GetExaminationExpirationsAsync(int limit, int offset)
        {
            JsonDocument response = await RequestAsync($"{ExaminationExpirationUrl}?$limit={limit}&$offset={offset}");
            return GetExaminationExpirations(response);
        }

        /// <summary>
        /// Gets the examination expirations asynchronously.
        /// </summary>
        /// <returns>Get all examination expiration dates of registered vehicles.</returns>
        public Task<Dictionary<string, DateTime>> GetExaminationExpirationsAsync()
            => GetExaminationExpirationsAsync(int.MaxValue, 0);

        private static string NormalizeLicensePlate(string licensePlate)
        {
            if (licensePlate is null)
            {
                throw new ArgumentNullException(licensePlate);
            }

            string normalized = licensePlate.Replace("-", string.Empty).ToUpperInvariant();

            if (normalized.Length != 6 || !normalized.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException("Not a valid license plate.", nameof(licensePlate));
            }

            return normalized;
        }

        private static DateTime ParseDate(string dateString)
        {
            int year = int.Parse(dateString.Substring(0, 4), CultureInfo.InvariantCulture);
            int month = int.Parse(dateString.Substring(4, 2), CultureInfo.InvariantCulture);
            int day = int.Parse(dateString.Substring(6, 2), CultureInfo.InvariantCulture);

            return new DateTime(year, month, day);
        }

        private static DateTime GetExaminationExpiration(JsonDocument document)
        {
            string dateString = document.RootElement.EnumerateArray().First().GetProperty("vervaldatum_keuring").GetString();
            return ParseDate(dateString);
        }

        private static Dictionary<string, DateTime> GetExaminationExpirations(JsonDocument document)
        {
            Dictionary<string, DateTime> result = new Dictionary<string, DateTime>();
            foreach (JsonElement entry in document.RootElement.EnumerateArray())
            {
                if (entry.TryGetProperty("kenteken", out JsonElement licensePlate) && entry.TryGetProperty("vervaldatum_keuring", out JsonElement date))
                {
                    result.Add(licensePlate.GetString(), ParseDate(date.GetString()));
                }
            }

            return result;
        }
    }
}
