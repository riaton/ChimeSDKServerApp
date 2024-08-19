using Xunit;
using ChimeApp.Domain;

namespace AmazonChimeSDKFunction.Tests;

public class CommonResultTest
{
    [Fact]
    [Trait("Category", "Domain")]
    public void �����l�̊m�F()
    {
        Assert.Equal(200, CommonResult.OK);
        Assert.Equal(400, CommonResult.ValidateError);
        Assert.Equal(500, CommonResult.InternalServerError);
        var header = new Dictionary<string, string>(){
            {"Access-Control-Allow-Headers", "Content-Type"},
            {"Access-Control-Allow-Origin", "*"},
            {"Access-Control-Allow-Methods", "OPTIONS,POST,GET"}
        };
        Assert.Equal(header, CommonResult.ResponseHeader);
    }

    [Fact]
    [Trait("Category", "Domain")]
    public void �X�e�[�^�X�R�[�h����K�؂ȃX�e�[�^�X���b�Z�[�W���擾�ł���()
    {
        var message = CommonResult.CreateStatusMessage(CommonResult.OK);
        Assert.Equal("OK", message);

        message = CommonResult.CreateStatusMessage(CommonResult.ValidateError);
        Assert.Equal("Validation Error", message);

        message = CommonResult.CreateStatusMessage(CommonResult.InternalServerError);
        Assert.Equal("Internal Server Error", message);

        message = CommonResult.CreateStatusMessage(999);
        Assert.Equal("What is this error?", message);
    }
}
