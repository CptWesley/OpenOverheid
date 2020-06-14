using System.Net.Http;

namespace OpenOverheid.Rdw
{
    /// <summary>
    /// The API for getting vehicle information.
    /// </summary>
    /// <seealso cref="OverheidApiBase" />
    public class VehicleApi : OverheidApiBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApi"/> class.
        /// </summary>
        public VehicleApi()
            : base()
        {
            Inspections = new VehicleInspectionApi(Client);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApi"/> class.
        /// </summary>
        /// <param name="client">The client used for webrequests.</param>
        public VehicleApi(HttpClient client)
            : base(client)
        {
            Inspections = new VehicleInspectionApi(Client);
        }

        /// <summary>
        /// Gets the inspections API.
        /// </summary>
        public VehicleInspectionApi Inspections { get; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Inspections.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
