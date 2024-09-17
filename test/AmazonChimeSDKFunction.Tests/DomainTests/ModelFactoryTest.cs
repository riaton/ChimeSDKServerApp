using ChimeApp.Factories;
using Google.Protobuf;
using Moq;
using Xunit;
using ChimeAppTest;

namespace ChimeSDKServerApp.Tests.DomainTests
{
    public class ModelFactoryTest
    {
        [Fact]
        public void ModelFactoryクラスのテスト()
        {
            (bool ok, var model) = ModelFactory.CreateModel<TestMessage>(null);
            ok.IsFalse();

            TestMessage message = new(){ Str2 = "aaa" };
            (ok, model) = ModelFactory.CreateModel(message);
            ok.IsTrue();
            model.IsNotNull();
            model.Str2.Is("aaa");

            message = new() { Str2 = string.Empty };
            (ok, model) = ModelFactory.CreateModel(message);
            ok.IsFalse();

            var mock = new Mock<IMessage>();
            (ok, var model2) = ModelFactory.CreateModel(mock.Object);
            ok.IsFalse();
        }
    }
}
