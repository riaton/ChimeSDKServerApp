using Xunit;
using ChimeApp.Factories;
using ChimeApp.Domain;
using ChimeApp.Models;
using Newtonsoft.Json;

namespace AmazonChimeSDKFunction.Tests;

public class ResponseFactoryTest
{
    [Fact]
    [Trait("Category", "Domain")]
    public void ˆø”‚ÌBody‚ªNULL()
    {
        var res = ResponseFactory.CreateResponse(CommonResult.OK);
        var header = new Dictionary<string, string>(){
            {"Access-Control-Allow-Headers", "Content-Type"},
            {"Access-Control-Allow-Origin", "*"},
            {"Access-Control-Allow-Methods", "OPTIONS,POST,GET"}
        };
        Assert.Equal(CommonResult.OK, res.StatusCode);
        Assert.Equal(header, res.Headers);
        Assert.Equal(string.Empty, res.Body);
    }

    [Fact]
    [Trait("Category", "Domain")]
    public void ˆø”‚ÌBody‚ªNULL‚Å‚È‚¢()
    {
        var message = new TestMessage2
        {
            Test = "aaa"
        };
        var res = ResponseFactory.CreateResponse(CommonResult.OK, message);

        var expected = JsonConvert.SerializeObject(message);
        Assert.Equal(expected, res.Body);
    }
}
