using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;
using Amazon.ChimeSDKMeetings;
using System.Text.Json;
using Amazon.DynamoDBv2;


namespace ChimeApp.LambdaFunctions;

public class JoinMeetingFunction
{
    private readonly IMeetingOperation _meetingRepository;
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public JoinMeetingFunction()
    {
        _meetingRepository = new MeetingOperation(new AmazonChimeSDKMeetingsClient());
        _dynamoDBRepository = new DynamoDBOperation(new AmazonDynamoDBClient());
    }

    /// <summary>
    /// Amazon Chime SDK��p���ă~�[�f�B���O�ɎQ���҂�ǉ�����
    /// ���̌�Q���ҏ���DynamoDB�ɕۑ�����
    /// �Ō�ɎQ���҂̏����N���C�A���g�ɕԋp����
    /// </summary>
    /// <param name="input">API Gateway����̗v��</param>
    /// <param name="context">Lambda���̃��O�L�^�ƋL�q�̂��߂̃��\�b�h</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> JoinMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //�f�V���A���C�Y�ƃo���f�[�V����
            JoinMeetingRequest message = JoinMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //�~�[�e�B���O�ւ̎Q��
            var joinInfo = await _meetingRepository.JoinMeeting(message.MeetingId, message.ExternalAttendeeId);
            //DynamoDB�ɕۑ�
            await _dynamoDBRepository.RegisterAttendeeInfo(joinInfo);

            JoinMeetingResponse response = new();
            response.AttendeeInfo = JsonSerializer.Serialize(joinInfo);

            //�N���C�A���g�ɕԋp
            return ResponseFactory.CreateResponse(CommonResult.OK, response);
        }
        catch (EnvironmentVariableException ex)
        {
            context.Logger.LogLine(ex.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
        catch (Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
