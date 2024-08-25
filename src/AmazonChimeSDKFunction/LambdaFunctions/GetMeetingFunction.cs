using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;


namespace ChimeApp.LambdaFunctions;

public class GetMeetingFunction
{
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public GetMeetingFunction()
    {
        _dynamoDBRepository = new DynamoDBOperation();
    }

    /// <summary>
    /// DynamoDB����v����externalMeetingId�Ɉ�v����~�[�f�B���O�����擾����
    /// </summary>
    /// <param name="input">API Gateway����̗v��</param>
    /// <param name="context">Lambda���̃��O�L�^�ƋL�q�̂��߂̃��\�b�h</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> GetMeetingFromDB(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //�o���f�[�V����
            if (string.IsNullOrEmpty(input.PathParameters["externalMeetingId"]))
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //DynamoDB����externalMeetingId�Ɉ�v����~�[�f�B���O�����擾
            string meetingData = await _dynamoDBRepository.GetMeetingInfo(input.PathParameters["externalMeetingId"]);

            GetMeetingResponse response = new();
            response.MeetingInfo = meetingData;

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
