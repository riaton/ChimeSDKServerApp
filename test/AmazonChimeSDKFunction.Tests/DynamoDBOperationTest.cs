using Xunit;
using Amazon.ChimeSDKMeetings.Model;
using Moq;
using ChimeApp.Infrastructure;
using ChimeSDKServerApp.Domain.DomainHelper;
using Amazon.DynamoDBv2;
using ChimeApp.Domain;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace AmazonChimeSDKFunction.Tests;

public class DynamoDBOperationTest
{
    private Mock<IAmazonDynamoDB> _mock;
    private Mock<DomainHelper> _domainHelperMock;
    private DynamoDBOperation _dynamoDBOperation;
    public DynamoDBOperationTest()
    {
        _domainHelperMock = new Mock<DomainHelper>();

        _mock = new Mock<IAmazonDynamoDB>();
        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task DynamoDB‚Ö‚ÌMeeting“o˜^()
    {
        var meeting = new Meeting();
        meeting.ExternalMeetingId = "aaa";
        meeting.MeetingId = "bbb";
        var ex = await Assert.ThrowsAsync<EnvironmentVariableException>(() => 
            _dynamoDBOperation.RegisterMeetingInfo(meeting));

        ex.Message.Is("A Environment variable of meeting table is null");

        _domainHelperMock.Setup(x => x.GetMeetingTableName()).Returns("MeetingTable");

        _mock.Setup(x => x.PutItemAsync(It.IsAny<PutItemRequest>(), default))
            .Callback<PutItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("MeetingTable");
                request.Item["ExternalMeetingId"].S.Is(meeting.ExternalMeetingId);
                request.Item["MeetingId"].S.Is(meeting.MeetingId);
                request.Item["MeetingInfo"].S.Is(JsonConvert.SerializeObject(meeting));
            });

        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
        await _dynamoDBOperation.RegisterMeetingInfo(meeting);

        _mock.Verify(x => x.PutItemAsync(It.IsAny<PutItemRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task DynamoDB‚Ö‚ÌAttendee“o˜^()
    {
        var attendee = new Attendee();
        attendee.AttendeeId = "ccc";
        attendee.ExternalUserId = "ddd";

        var ex = await Assert.ThrowsAsync<EnvironmentVariableException>(() =>
            _dynamoDBOperation.RegisterAttendeeInfo(attendee));

        ex.Message.Is("A Environment variable of attendee table is null");

        _domainHelperMock.Setup(x => x.GetAttendeeTableName()).Returns("AttendeeTable");

        _mock.Setup(x => x.PutItemAsync(It.IsAny<PutItemRequest>(), default))
            .Callback<PutItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("AttendeeTable");
                request.Item["AttendeeId"].S.Is(attendee.AttendeeId);
                request.Item["ExternalAttendeeId"].S.Is(attendee.ExternalUserId);
            });

        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
        await _dynamoDBOperation.RegisterAttendeeInfo(attendee);

        _mock.Verify(x => x.PutItemAsync(It.IsAny<PutItemRequest>(), default), Times.Once);
    }
}
