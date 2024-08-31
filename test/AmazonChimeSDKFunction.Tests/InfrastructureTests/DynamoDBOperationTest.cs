using Xunit;
using Amazon.ChimeSDKMeetings.Model;
using Moq;
using ChimeApp.Infrastructure;
using ChimeSDKServerApp.Domain.DomainHelper;
using Amazon.DynamoDBv2;
using ChimeApp.Domain;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace ChimeSDKServerApp.Tests.InfrastructureTests;

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
    public async Task DynamoDBÇ÷ÇÃMeetingìoò^()
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
    public async Task DynamoDBÇ÷ÇÃAttendeeìoò^()
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

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task DynamoDBÇ©ÇÁÇÃMeetingçÌèú()
    {
        var externalMeetingId = "externalMeetingId";
        var ex = await Assert.ThrowsAsync<EnvironmentVariableException>(() =>
            _dynamoDBOperation.DeleteMeetingInfo(externalMeetingId));

        ex.Message.Is("A Environment variable of meeting table is null");

        _domainHelperMock.Setup(x => x.GetMeetingTableName()).Returns("MeetingTable");

        _mock.Setup(x => x.DeleteItemAsync(It.IsAny<DeleteItemRequest>(), default))
            .Callback<DeleteItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("MeetingTable");
                request.Key["ExternalMeetingId"].S.Is(externalMeetingId);
            });

        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
        await _dynamoDBOperation.DeleteMeetingInfo(externalMeetingId);

        _mock.Verify(x => x.DeleteItemAsync(It.IsAny<DeleteItemRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task DynamoDBÇ©ÇÁÇÃAttendeeçÌèú()
    {
        var attendeeId = "AttendeeId";

        var ex = await Assert.ThrowsAsync<EnvironmentVariableException>(() =>
            _dynamoDBOperation.DeleteAttendeeInfo(attendeeId));

        ex.Message.Is("A Environment variable of attendee table is null");

        _domainHelperMock.Setup(x => x.GetAttendeeTableName()).Returns("AttendeeTable");

        _mock.Setup(x => x.DeleteItemAsync(It.IsAny<DeleteItemRequest>(), default))
            .Callback<DeleteItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("AttendeeTable");
                request.Key["AttendeeId"].S.Is(attendeeId);
            });

        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
        await _dynamoDBOperation.DeleteAttendeeInfo(attendeeId);

        _mock.Verify(x => x.DeleteItemAsync(It.IsAny<DeleteItemRequest>(), default), Times.Once);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task DynamoDBÇ©ÇÁÇÃMeetingéÊìæ()
    {
        var externalMeetingId = "externalMeetingId";
        var ex = await Assert.ThrowsAsync<EnvironmentVariableException>(() =>
            _dynamoDBOperation.GetMeetingInfo(externalMeetingId));

        ex.Message.Is("A Environment variable of meeting table is null");

        _domainHelperMock.Setup(x => x.GetMeetingTableName()).Returns("MeetingTable");

        _mock.Setup(x => x.GetItemAsync(It.IsAny<GetItemRequest>(), default))
            .Callback<GetItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("MeetingTable");
                request.Key["ExternalMeetingId"].S.Is(externalMeetingId);
            })
            .ReturnsAsync(new GetItemResponse()
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    {"MeetingInfo", new AttributeValue{ S = "aaa" }}
                }
            });

        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
        var result = await _dynamoDBOperation.GetMeetingInfo(externalMeetingId);

        result.Is("aaa");
        _mock.Verify(x => x.GetItemAsync(It.IsAny<GetItemRequest>(), default), Times.Once);

        _mock.Setup(x => x.GetItemAsync(It.IsAny<GetItemRequest>(), default))
            .Callback<GetItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("MeetingTable");
                request.Key["ExternalMeetingId"].S.Is(externalMeetingId);
            })
            .ReturnsAsync(new GetItemResponse()
            {
                Item = new Dictionary<string, AttributeValue>()
            });

        result = await _dynamoDBOperation.GetMeetingInfo(externalMeetingId);

        result.Is(string.Empty);
    }

    [Fact]
    [Trait("Category", "Infrastructure")]
    public async Task DynamoDBÇ©ÇÁÇÃAttendeeéÊìæ()
    {
        var attendeeId = "attendeeId";
        var ex = await Assert.ThrowsAsync<EnvironmentVariableException>(() =>
            _dynamoDBOperation.GetAttendeeInfo(attendeeId));

        ex.Message.Is("A Environment variable of attendee table is null");

        _domainHelperMock.Setup(x => x.GetAttendeeTableName()).Returns("AttendeeTable");

        _mock.Setup(x => x.GetItemAsync(It.IsAny<GetItemRequest>(), default))
            .Callback<GetItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("AttendeeTable");
                request.Key["AttendeeId"].S.Is(attendeeId);
            })
            .ReturnsAsync(new GetItemResponse()
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    {"ExternalAttendeeId", new AttributeValue{ S = "bbb" }}
                }
            });

        _dynamoDBOperation = new DynamoDBOperation(_mock.Object, _domainHelperMock.Object);
        var result = await _dynamoDBOperation.GetAttendeeInfo(attendeeId);

        result.Is("bbb");
        _mock.Verify(x => x.GetItemAsync(It.IsAny<GetItemRequest>(), default), Times.Once);

        _mock.Setup(x => x.GetItemAsync(It.IsAny<GetItemRequest>(), default))
            .Callback<GetItemRequest, CancellationToken>((request, token) =>
            {
                request.TableName.Is("AttendeeTable");
                request.Key["AttendeeId"].S.Is(attendeeId);
            })
            .ReturnsAsync(new GetItemResponse()
            {
                Item = new Dictionary<string, AttributeValue>()
            });
        result = await _dynamoDBOperation.GetAttendeeInfo(attendeeId);

        result.Is(string.Empty);
    }
}
