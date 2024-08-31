using Xunit;
using ChimeApp.Factories;
using ChimeApp.Domain;
using ChimeApp.Models;
using Newtonsoft.Json;

namespace ChimeSDKServerApp.Tests.DomainTests;

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
        res.StatusCode.Is(CommonResult.OK);
        res.Headers.Is(header);
        res.Body.Is(string.Empty);
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
        res.Body.Is(expected);
    }
}
