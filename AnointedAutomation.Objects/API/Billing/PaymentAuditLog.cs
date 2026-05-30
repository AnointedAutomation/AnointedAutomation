// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-01-16 19:19:01
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
//Stewarded by Alexander Fields

using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Objects.Billing
{
    /// <summary>
    /// Represents a PCI-DSS compliant audit log entry for payment processor communications.
    /// Captures complete request/response data with sensitive information masked.
    /// Designed for 7-year retention as per PCI-DSS requirements.
    /// </summary>
    [System.Serializable]
    public class PaymentAuditLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAuditLog"/> class.
        /// </summary>
        public PaymentAuditLog()
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.CreatedAt = System.DateTime.UtcNow;
            this.CorrelationId = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAuditLog"/> class with specified parameters.
        /// </summary>
        /// <param name="provider">The payment provider.</param>
        /// <param name="operation">The operation being performed.</param>
        /// <param name="transactionId">The transaction ID if applicable.</param>
        public PaymentAuditLog(PaymentProvider provider, PaymentOperation operation, string transactionId)
        {
            this.Id = System.Guid.NewGuid().ToString();
            this.Provider = provider;
            this.Operation = operation;
            this.TransactionId = transactionId;
            this.CreatedAt = System.DateTime.UtcNow;
            this.CorrelationId = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the unique identifier for this audit log entry.
        /// </summary>
        [DataMember]
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the correlation ID for tracking related operations.
        /// </summary>
        [DataMember]
        public string CorrelationId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the transaction ID from the payment processor.
        /// </summary>
        [DataMember]
        public string TransactionId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the payment provider.
        /// </summary>
        [DataMember]
        public PaymentProvider Provider
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the operation being performed.
        /// </summary>
        [DataMember]
        public PaymentOperation Operation
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the HTTP method used (GET, POST, PUT, DELETE, etc.).
        /// </summary>
        [DataMember]
        public string RequestMethod
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the API endpoint called.
        /// </summary>
        [DataMember]
        public string RequestEndpoint
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the request headers (with sensitive values masked).
        /// </summary>
        [DataMember]
        public string RequestHeaders
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the request body (with sensitive data masked).
        /// </summary>
        [DataMember]
        public string RequestBody
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the timestamp when the request was sent.
        /// </summary>
        [DataMember]
        public System.DateTime RequestTimestamp
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the HTTP status code received.
        /// </summary>
        [DataMember]
        public int ResponseStatusCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        [DataMember]
        public string ResponseHeaders
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the response body (with sensitive data masked).
        /// </summary>
        [DataMember]
        public string ResponseBody
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the timestamp when the response was received.
        /// </summary>
        [DataMember]
        public System.DateTime ResponseTimestamp
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the raw, unmodified response from the payment processor.
        /// Stored for PCI-DSS compliance. Contains masked sensitive data.
        /// </summary>
        [DataMember]
        public string RawResponse
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        [DataMember]
        public bool Success
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the error message if the operation failed.
        /// </summary>
        [DataMember]
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the error code from the payment processor.
        /// </summary>
        [DataMember]
        public string ErrorCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the duration of the operation in milliseconds.
        /// </summary>
        [DataMember]
        public long DurationMs
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the user ID who initiated the operation.
        /// </summary>
        [DataMember]
        public string UserId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the customer ID in the payment system.
        /// </summary>
        [DataMember]
        public string CustomerId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the IP address of the request originator.
        /// </summary>
        [DataMember]
        public string IpAddress
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the user agent string.
        /// </summary>
        [DataMember]
        public string UserAgent
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets additional metadata as JSON string.
        /// </summary>
        [DataMember]
        public string Metadata
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        [DataMember]
        public System.DateTime CreatedAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a live mode transaction.
        /// </summary>
        [DataMember]
        public bool IsLiveMode
        {
            get; set;
        }

        /// <summary>
        /// Masks sensitive data in the provided string.
        /// Masks card numbers, CVV, SSN, routing numbers, and account numbers.
        /// </summary>
        /// <param name="data">The data to mask.</param>
        /// <returns>The data with sensitive information masked.</returns>
        public static string MaskSensitiveData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            string result = data;

            // Mask card numbers (13-19 digits, show first 4 and last 4)
            result = Regex.Replace(result, @"\b(\d{4})\d{5,11}(\d{4})\b", "$1********$2");

            // Mask CVV/CVC (3-4 digits after "cvv", "cvc", "security_code")
            result = Regex.Replace(result, @"(cvv|cvc|security_code)([""':]\s*)(\d{3,4})", "$1$2***", RegexOptions.IgnoreCase);

            // Mask SSN (XXX-XX-XXXX format)
            result = Regex.Replace(result, @"\b(\d{3})-(\d{2})-(\d{4})\b", "***-**-$3");

            // Mask routing numbers (9 digits after "routing")
            result = Regex.Replace(result, @"(routing[_\s]?number[""':]\s*)(\d{9})", "$1*********", RegexOptions.IgnoreCase);

            // Mask account numbers (show last 4)
            result = Regex.Replace(result, @"(account[_\s]?number[""':]\s*)(\d+)(\d{4})", "$1****$3", RegexOptions.IgnoreCase);

            // Mask API keys and tokens
            result = Regex.Replace(result, @"(api[_\s]?key|secret[_\s]?key|access[_\s]?token|bearer)[""':]\s*[""']?([a-zA-Z0-9_\-]{8})[a-zA-Z0-9_\-]+", "$1: $2********", RegexOptions.IgnoreCase);

            return result;
        }

        /// <summary>
        /// Sets the request body with automatic sensitive data masking.
        /// </summary>
        /// <param name="body">The request body to set.</param>
        public void SetRequestBody(string body)
        {
            this.RequestBody = MaskSensitiveData(body);
        }

        /// <summary>
        /// Sets the response body with automatic sensitive data masking.
        /// </summary>
        /// <param name="body">The response body to set.</param>
        public void SetResponseBody(string body)
        {
            this.ResponseBody = MaskSensitiveData(body);
            this.RawResponse = MaskSensitiveData(body);
        }

        /// <summary>
        /// Calculates and sets the duration based on request and response timestamps.
        /// </summary>
        public void CalculateDuration()
        {
            if (ResponseTimestamp > RequestTimestamp)
            {
                this.DurationMs = (long)(ResponseTimestamp - RequestTimestamp).TotalMilliseconds;
            }
        }
    }
}
