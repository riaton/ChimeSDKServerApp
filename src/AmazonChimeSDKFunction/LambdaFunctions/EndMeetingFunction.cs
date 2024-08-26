using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;


namespace ChimeApp.LambdaFunctions;

public class EndMeetingFunction
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IDynamoDBRepository _dynamoDBRepository;
    public EndMeetingFunction()
    {
        _meetingRepository = new MeetingOperation();
        _dynamoDBRepository = new DynamoDBOperation();
    }

    /// <summary>
    /// Amazon Chime SDK��p���ă~�[�f�B���O���I�����A�Q���҂��폜����
    /// ���̌��c����DynamoDB����폜����
    /// </summary>
    /// <param name="input">API Gateway����̗v��</param>
    /// <param name="context">Lambda���̃��O�L�^�ƋL�q�̂��߂̃��\�b�h</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> EndMeeting(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //�f�V���A���C�Y�ƃo���f�[�V����
            EndMeetingRequest message = EndMeetingRequest.Parser.ParseJson(input.Body);
            var (validateOk, model) = ModelFactory.CreateModel(message);
            if (validateOk == false || model == null)
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //�~�[�e�B���O�̍폜
            await _meetingRepository.EndMeeting(message.MeetingId);
            //DynamoDB�����c�����폜
            await _dynamoDBRepository.DeleteMeetingInfo(message.ExternalMeetingId);

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
