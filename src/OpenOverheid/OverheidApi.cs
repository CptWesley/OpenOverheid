using System.Net.Http;

namespace OpenOverheid.Rdw
{
    /// <summary>
    /// The API for getting vehicle information.
    /// </summary>
    /// <seealso cref="OverheidApiBase" />
    public class OverheidApi : OverheidApiBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverheidApi"/> class.
        /// </summary>
        public OverheidApi()
            : base()
        {
            Vehicles = new VehicleApi(Client);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverheidApi"/> class.
        /// </summary>
        /// <param name="client">The client used for webrequests.</param>
        public OverheidApi(HttpClient client)
            : base(client)
        {
            Vehicles = new VehicleApi(Client);
        }

        /// <summary>
        /// Gets the vehicle API.
        /// </summary>
        public VehicleApi Vehicles { get; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Vehicles.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
