using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChimeApp.LambdaFunctions;

public class CreateMeetingFunction
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IDynamoDBRepository _dynamoDBRepository;
    public CreateMeetingFunction() : this(new MeetingOperation(), new DynamoDBOperation()) { }

    public CreateMeetingFunction(IMeetingRepository meeting, IDynamoDBRepository dynamo)
    {
        _meetingRepository = meeting;
        _dynamoDBRepository = dynamo;
    }

    /// <summary>
    /// Amazon Chime SDK��p���ă~�[�f�B���O���쐬���A�Q���҂�ǉ�����
    /// ���̌�쐬�����~�[�e�B���O���ƎQ���ҏ���DynamoDB�ɕۑ�����
    /// �Ō�Ƀ~�[�e�B���O�̏��ƎQ���҂̏����N���C�A���g�ɕԋp����
    /// </summary>
    /// <param name="input">API Gateway����̗v��</param>
    /// <param name="context">Lambda���̃��O�L�^�ƋL�q�̂��߂̃��\�b�h</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> CreateMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //�f�V���A���C�Y�ƃo���f�[�V����
            CreateMeetingRequest message = CreateMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //�~�[�e�B���O�쐬
            var meetingInfo = await _meetingRepository.CreateMeeting(model);
            //�~�[�e�B���O�ւ̎Q��
            var joinInfo = await _meetingRepository.JoinMeeting(meetingInfo.MeetingId, message.ExternalAttendeeId);
            //DynamoDB�ɕۑ�
            await _dynamoDBRepository.RegisterMeetingInfo(meetingInfo);
            await _dynamoDBRepository.RegisterAttendeeInfo(joinInfo);

            CreateMeetingResponse response = new();
            response.MeetingInfo = JsonSerializer.Serialize(meetingInfo);
            response.AttendeeInfo = JsonSerializer.Serialize(joinInfo);

            context.Logger.LogLine("meeting: " + meetingInfo.ToString());
            context.Logger.LogLine("attendee: " + joinInfo.ToString());
            //�N���C�A���g�ɕԋp
            return ResponseFactory.CreateResponse(CommonResult.OK, response);
        }
        catch(EnvironmentVariableException ex)
        {
            context.Logger.LogLine(ex.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
        catch(Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
