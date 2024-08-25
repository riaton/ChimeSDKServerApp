using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;


namespace ChimeApp.LambdaFunctions;

public class LeaveMeetingFunction
{
    private readonly IMeetingOperation _meetingRepository;
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public LeaveMeetingFunction()
    {
        _meetingRepository = new MeetingOperation();
        _dynamoDBRepository = new DynamoDBOperation();
    }

    /// <summary>
    /// Amazon Chime SDK��p���Ďw��~�[�e�B���O�̎w��Q���҂��폜����
    /// </summary>
    /// <param name="input">API Gateway����̗v��</param>
    /// <param name="context">Lambda���̃��O�L�^�ƋL�q�̂��߂̃��\�b�h</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> LeaveMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //�f�V���A���C�Y�ƃo���f�[�V����
            LeaveMeetingRequest message = LeaveMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //�Q���҂̍폜
            await _meetingRepository.LeaveMeeting(message.MeetingId, message.AttendeeId);
            //DynamoDB����Q���ҏ����폜
            await _dynamoDBRepository.DeleteAttendeeInfo(message.AttendeeId);

            //�N���C�A���g�ɕԋp
            return ResponseFactory.CreateResponse(CommonResult.OK);
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
