// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System.Collections.Generic;
using AnointedAutomation.Objects.Common;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class CommonTypesTests
    {
        [Fact]
        public void OperationResult_Ok_And_Fail_SetExpectedState()
        {
            OperationResult ok = OperationResult.Ok();
            Assert.True(ok.Success);
            Assert.Null(ok.Error);

            OperationResult fail = OperationResult.Fail("nope");
            Assert.False(fail.Success);
            Assert.Equal("nope", fail.Error);
        }

        [Fact]
        public void OperationResult_JsonRoundTrip_PreservesPropertyNames()
        {
            string json = JsonConvert.SerializeObject(OperationResult.Fail("bad"));
            Assert.Contains("\"Success\"", json);
            Assert.Contains("\"Error\"", json);

            OperationResult back = JsonConvert.DeserializeObject<OperationResult>(json);
            Assert.False(back.Success);
            Assert.Equal("bad", back.Error);
        }

        [Fact]
        public void DeletedResponse_Of_DefaultsToDeletedTrueNullId()
        {
            DeletedResponse d = DeletedResponse.Of();
            Assert.True(d.Deleted);
            Assert.Null(d.Id);

            DeletedResponse withId = DeletedResponse.Of(true, "abc123");
            Assert.True(withId.Deleted);
            Assert.Equal("abc123", withId.Id);
        }

        [Fact]
        public void BulkCallCreateRequest_DefaultsToEmptyItems_AndHoldsComment()
        {
            BulkCallCreateRequest<string> req = new BulkCallCreateRequest<string>();
            Assert.NotNull(req.Items);
            Assert.Empty(req.Items);

            req.Items = new List<string> { "a", "b" };
            req.Comment = "batch note";
            string json = JsonConvert.SerializeObject(req);
            Assert.Contains("\"Items\"", json);
            Assert.Contains("\"Comment\"", json);

            BulkCallCreateRequest<string> back = JsonConvert.DeserializeObject<BulkCallCreateRequest<string>>(json);
            Assert.Equal(2, back.Items.Count);
            Assert.Equal("batch note", back.Comment);
        }
    }
}
