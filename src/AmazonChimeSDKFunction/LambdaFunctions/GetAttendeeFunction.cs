using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ChimeApp.Domain;
using ChimeApp.Factories;
using ChimeApp.Models;
using ChimeApp.Infrastructure;
using Amazon.DynamoDBv2;

//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ChimeApp.LambdaFunctions;

public class GetAttendeeFunction
{
    private readonly IDynamoDBOperation _dynamoDBRepository;
    public GetAttendeeFunction()
    {
        _dynamoDBRepository = new DynamoDBOperation(new AmazonDynamoDBClient());
    }

    /// <summary>
    /// DynamoDB����v����attendeeId�Ɉ�v����Q���ҏ����擾����
    /// </summary>
    /// <param name="input">API Gateway����̗v��</param>
    /// <param name="context">Lambda���̃��O�L�^�ƋL�q�̂��߂̃��\�b�h</param>
    /// <returns>APIGatewayProxyResponse</returns>
    public async Task<APIGatewayProxyResponse> GetAttendeeFromDB(APIGatewayProxyRequest input, ILambdaContext context)
    {
        try
        {
            //�o���f�[�V����
            if (string.IsNullOrEmpty(input.PathParameters["attendeeId"]))
            {
                return ResponseFactory.CreateResponse(CommonResult.ValidateError);
            }
            //DynamoDB����AttendeeId�Ɉ�v����externalAttendeeId���擾
            string externalAttendeeId = await _dynamoDBRepository.GetAttendeeInfo(input.PathParameters["attendeeId"]);

            GetAttendeeResponse response = new();
            response.ExternalAttendeeId = externalAttendeeId;

            //�N���C�A���g�ɕԋp
            return ResponseFactory.CreateResponse(CommonResult.OK, response);
        }
        catch(Exception e)
        {
            context.Logger.LogLine(e.Message);
            return ResponseFactory.CreateResponse(CommonResult.InternalServerError);
        }
    }
}
