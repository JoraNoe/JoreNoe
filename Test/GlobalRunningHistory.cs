namespace Dogegg.Abstractions.Domains
{
    /// <summary>
    /// Represents a record in the GlobalRunningHistory table.
    /// </summary>
    public class GlobalRunningHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method of the request.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the path of the request.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the start time of the request.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the request.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets the query string of the request.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the body of the request.
        /// </summary>
        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }

        /// <summary>
        /// Gets or sets the full URL of the request.
        /// </summary>
        public string FullPathUrl { get; set; }

        /// <summary>
        /// Gets or sets the headers of the request.
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the request.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the record.
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Ip 
        /// </summary>
        public string IpAddress { get; set; }

        public string UserAgent { get; set; }
    }
}
