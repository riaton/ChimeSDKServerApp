using Xunit;
using ChimeApp.Domain;

namespace AmazonChimeSDKFunction.Tests;

public class CommonResultTest
{
    [Fact]
    [Trait("Category", "Domain")]
    public void �����l�̊m�F()
    {
        CommonResult.OK.Is(200);
        CommonResult.ValidateError.Is(400);
        CommonResult.InternalServerError.Is(500);
        var header = new Dictionary<string, string>(){
            {"Access-Control-Allow-Headers", "Content-Type"},
            {"Access-Control-Allow-Origin", "*"},
            {"Access-Control-Allow-Methods", "OPTIONS,POST,GET"}
        };
        CommonResult.ResponseHeader.Is(header);
    }

    [Fact]
    [Trait("Category", "Domain")]
    public void �X�e�[�^�X�R�[�h����K�؂ȃX�e�[�^�X���b�Z�[�W���擾�ł���()
    {
        var message = CommonResult.CreateStatusMessage(CommonResult.OK);
        message.Is("OK");

        message = CommonResult.CreateStatusMessage(CommonResult.ValidateError);
        message.Is("Validation Error");

        message = CommonResult.CreateStatusMessage(CommonResult.InternalServerError);
        message.Is("Internal Server Error");

        message = CommonResult.CreateStatusMessage(999);
        message.Is("What is this error?");
    }
}
