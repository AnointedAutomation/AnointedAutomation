// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using System.Text.Json;
using Xunit;

namespace AnointedAutomation.Objects.API.Tests
{
    /// <summary>
    /// Unit tests for GraphQlEnvelope and GraphQlResult.
    /// Tests cover success, failure, and edge cases per CLAUDE_TESTING.md standards.
    /// </summary>
    public class GraphQlEnvelopeTests
    {
        private const string Mutation = "productUpdate";

        #region Read - malformed input

        [Fact]
        public void Read_NullBody_ReturnsTransportErrorAndNoData()
        {
            GraphQlResult result = GraphQlEnvelope.Read(null);

            Assert.False(result.HasData);
            Assert.Single(result.TransportErrors);
            Assert.Equal("GraphQL response body was empty", result.TransportErrors[0]);
        }

        [Fact]
        public void Read_EmptyBody_ReturnsTransportError()
        {
            GraphQlResult result = GraphQlEnvelope.Read("   ");

            Assert.False(result.HasData);
            Assert.Single(result.TransportErrors);
        }

        [Fact]
        public void Read_NotJsonBody_ReturnsTransportError()
        {
            GraphQlResult result = GraphQlEnvelope.Read("<html>502 Bad Gateway</html>");

            Assert.False(result.HasData);
            Assert.Single(result.TransportErrors);
            Assert.Equal("GraphQL response body was not valid JSON", result.TransportErrors[0]);
            Assert.False(result.Succeeded(Mutation));
            Assert.Equal("GraphQL response body was not valid JSON", result.FirstError(Mutation));
        }

        [Fact]
        public void Read_NonObjectJson_ReturnsTransportError()
        {
            GraphQlResult result = GraphQlEnvelope.Read("[1,2,3]");

            Assert.False(result.HasData);
            Assert.Single(result.TransportErrors);
        }

        #endregion

        #region Read - data and errors

        [Fact]
        public void Read_ValidDataWithUserErrors_ExtractsMessages()
        {
            string body = "{\"data\":{\"productUpdate\":{\"product\":{\"id\":\"gid://1\"},\"userErrors\":[{\"field\":[\"title\"],\"message\":\"Title cannot be blank\"},{\"message\":\"Second problem\"}]}}}";

            GraphQlResult result = GraphQlEnvelope.Read(body);

            Assert.True(result.HasData);
            Assert.Empty(result.TransportErrors);
            Assert.Equal(2, result.UserErrors(Mutation).Count);
            Assert.Equal("Title cannot be blank", result.UserErrors(Mutation)[0]);
            Assert.False(result.Succeeded(Mutation));
            Assert.Equal("Title cannot be blank", result.FirstError(Mutation));
        }

        [Fact]
        public void Read_EmptyUserErrors_Succeeds()
        {
            string body = "{\"data\":{\"productUpdate\":{\"product\":{\"id\":\"gid://1\"},\"userErrors\":[]}}}";

            GraphQlResult result = GraphQlEnvelope.Read(body);

            Assert.True(result.HasData);
            Assert.Empty(result.UserErrors(Mutation));
            Assert.True(result.Succeeded(Mutation));
            Assert.Null(result.FirstError(Mutation));
        }

        [Fact]
        public void Read_TopLevelErrors_ReportsTransportErrors()
        {
            string body = "{\"errors\":[{\"message\":\"Throttled\",\"path\":[\"productUpdate\"],\"extensions\":{\"code\":\"THROTTLED\"}},{\"message\":\"Other\"}]}";

            GraphQlResult result = GraphQlEnvelope.Read(body);

            Assert.False(result.HasData);
            Assert.Equal(2, result.TransportErrors.Count);
            Assert.Equal("Throttled", result.TransportErrors[0]);
            Assert.Equal(2, result.Errors.Count);
            Assert.Equal("[\"productUpdate\"]", result.Errors[0].Path);
            Assert.Contains("THROTTLED", result.Errors[0].Extensions);
            Assert.Null(result.Errors[1].Path);
            Assert.Null(result.Errors[1].Extensions);
            Assert.False(result.Succeeded(Mutation));
            Assert.Equal("Throttled", result.FirstError(Mutation));
        }

        [Fact]
        public void Read_MissingData_HasDataFalseButNoTransportError()
        {
            GraphQlResult result = GraphQlEnvelope.Read("{}");

            Assert.False(result.HasData);
            Assert.Empty(result.TransportErrors);
            Assert.Empty(result.UserErrors(Mutation));
            Assert.True(result.Succeeded(Mutation));
            Assert.Null(result.Payload(Mutation));
        }

        [Fact]
        public void Read_NullData_HasDataFalse()
        {
            GraphQlResult result = GraphQlEnvelope.Read("{\"data\":null}");

            Assert.False(result.HasData);
            Assert.Null(result.Payload(Mutation));
        }

        [Fact]
        public void Read_MissingMutationField_UserErrorsEmptyAndPayloadNull()
        {
            string body = "{\"data\":{\"otherMutation\":{\"userErrors\":[{\"message\":\"x\"}]}}}";

            GraphQlResult result = GraphQlEnvelope.Read(body);

            Assert.True(result.HasData);
            Assert.Empty(result.UserErrors(Mutation));
            Assert.Null(result.Payload(Mutation));
            Assert.True(result.Succeeded(Mutation));
        }

        [Fact]
        public void Read_MissingUserErrorsProperty_ReturnsEmptyList()
        {
            string body = "{\"data\":{\"productUpdate\":{\"product\":{\"id\":\"gid://1\"}}}}";

            GraphQlResult result = GraphQlEnvelope.Read(body);

            Assert.Empty(result.UserErrors(Mutation));
            Assert.True(result.Succeeded(Mutation));
        }

        #endregion

        #region Payload

        [Fact]
        public void Payload_PresentField_ReturnsElement()
        {
            string body = "{\"data\":{\"productUpdate\":{\"product\":{\"id\":\"gid://1\"},\"userErrors\":[]}}}";

            GraphQlResult result = GraphQlEnvelope.Read(body);
            JsonElement? payload = result.Payload(Mutation);

            Assert.True(payload.HasValue);
            Assert.Equal("gid://1", payload.Value.GetProperty("product").GetProperty("id").GetString());
        }

        [Fact]
        public void Payload_NullMutationField_ReturnsNull()
        {
            GraphQlResult result = GraphQlEnvelope.Read("{\"data\":{\"productUpdate\":{}}}");

            Assert.Null(result.Payload(null));
            Assert.Null(result.Payload(string.Empty));
        }

        [Fact]
        public void Payload_NullField_ReturnsNull()
        {
            GraphQlResult result = GraphQlEnvelope.Read("{\"data\":{\"productUpdate\":null}}");

            Assert.Null(result.Payload(Mutation));
        }

        #endregion

        #region FirstError precedence

        [Fact]
        public void FirstError_TransportBeforeUserErrors()
        {
            string body = "{\"data\":{\"productUpdate\":{\"userErrors\":[{\"message\":\"user problem\"}]}},\"errors\":[{\"message\":\"transport problem\"}]}";

            GraphQlResult result = GraphQlEnvelope.Read(body);

            Assert.Equal("transport problem", result.FirstError(Mutation));
            Assert.False(result.Succeeded(Mutation));
        }

        #endregion
    }
}
