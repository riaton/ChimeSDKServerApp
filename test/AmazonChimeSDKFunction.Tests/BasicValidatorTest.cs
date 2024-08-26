using ChimeApp.Domain;
using ChimeApp.Models;
using Xunit;


namespace ChimeSDKServerApp.Tests
{
    public class BasicValidatorTest
    {
        private TestMessage1 _testMessage;
        public BasicValidatorTest()
        {
            var subMessage1 = new SubMessage()
            {
                TestBool = true
            };

            var subMessage2 = new SubMessage()
            {
                TestBool = false
            };

            _testMessage = new TestMessage1()
            {
                TestString = "1234567890",
                TestInteger = 1,
                TestSubMessage = subMessage1
            };

            _testMessage.TestStringList.Add("aaa");
            _testMessage.TestStringList.Add("bbb");
            _testMessage.TestSubMessageList.Add(subMessage1);
            _testMessage.TestSubMessageList.Add(subMessage2);
        }

        [Fact]
        [Trait("Category", "Domain")]
        public void 文字列のバリデーション()
        {
            var results = BasicValidator.Validate(_testMessage);
            Assert.Empty(results);

            _testMessage.TestString = string.Empty;
            results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to Required Check. FieldName = TestString");

            _testMessage.TestString = "12345678910";
            results = BasicValidator.Validate(_testMessage);
            Assert.Single(results);
            results[0].Is("Failed to StrLen Check. FieldName = TestString");

            _testMessage.TestString = "abcde";
            results = BasicValidator.Validate(_testMessage);
            Assert.Single(results);
            results[0].Is("Failed to Regex Check. FieldName = TestString");
        }

        [Fact]
        [Trait("Category", "Domain")]
        public void 数値のバリデーション()
        {
            _testMessage.TestInteger = null;
            var results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to Required Check. FieldName = TestInteger");

            _testMessage.TestInteger = 0;
            results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to MinVal Check. FieldName = TestInteger");

            _testMessage.TestInteger = 10;
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestInteger = 11;
            results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to MaxVal Check. FieldName = TestInteger");
        }

        [Fact]
        [Trait("Category", "Domain")]
        public void サブメッセージのバリデーション()
        {
            _testMessage.TestSubMessage = null;
            var results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to Required Check. FieldName = TestSubMessage");
            _testMessage.TestSubMessage = new SubMessage();
            results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to Required Check. FieldName = TestBool");
            _testMessage.TestSubMessage.TestBool = true;
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestSubMessage.TestBool = false;
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);
        }

        [Fact]
        [Trait("Category", "Domain")]
        public void 文字列リストのバリデーション()
        {
            _testMessage.TestStringList.Clear();
            var results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestStringList.Add("aaa");
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestStringList.Add("bbb");
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);
        }

        [Fact]
        [Trait("Category", "Domain")]
        public void サブメッセージリストのバリデーション()
        {
            _testMessage.TestSubMessageList.Clear();
            var results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestSubMessageList.Add(new SubMessage());
            results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to Required Check. FieldName = TestBool");
            _testMessage.TestSubMessageList[0].TestBool = false;
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestSubMessageList.Add(new SubMessage()
            {
                TestBool = true
            });
            results = BasicValidator.Validate(_testMessage);

            Assert.Empty(results);

            _testMessage.TestSubMessageList[1].TestBool = null;
            results = BasicValidator.Validate(_testMessage);

            Assert.Single(results);
            results[0].Is("Failed to Required Check. FieldName = TestBool");
        }
    }
}
