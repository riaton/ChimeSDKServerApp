using ChimeApp.Domain;
using ChimeApp.Models;
using ChimeAppTest;
using Xunit;
using Xunit.Abstractions;


namespace ChimeSDKServerApp.Tests.DomainTests
{
    public class BasicValidatorTest
    {
        private static TestMessage _test1 = new()
        {
            Str1 = null,
            Str2 = "bbb",
            Str3 = "cccccccccc",
            Str4 = "1234",
        };

        private static TestMessage _test2 = new()
        {
            Str1 = null,
            Str2 = null,
            Str3 = "cccccccccc",
            Str4 = "1234",
        };

        private static TestMessage _test3 = new()
        {
            Str1 = null,
            Str2 = "",
            Str3 = "cccccccccc",
            Str4 = "1234",
        };

        private static TestMessage _test4 = new()
        {
            Str1 = null,
            Str2 = "bbb",
            Str3 = "cccccccccca",
            Str4 = "1234",
        };

        private static TestMessage _test5 = new()
        {
            Str1 = null,
            Str2 = "bbb",
            Str3 = "cccccccccc",
            Str4 = "123a",
        };

        [Theory]
        [MemberData(nameof(TestData))]
        [Trait("Category", "Domain")]
        public void 文字列のバリデーション(TestMessage message, int errorNum, string errorMessage)
        {
            var results = BasicValidator.Validate(message);

            if(errorNum == 0)
            {
                Assert.Empty(results);
            }

            if(errorNum == 1)
            {
                Assert.Single(results);
                results[0].Is(errorMessage);
            }
        }

        public static IEnumerable<object[]> TestData =>
            new List<object[]>
        {
            new object[] { _test1, 0, string.Empty },
            new object[] { _test2, 1, "Failed to Required Check. FieldName = Str2" },
            new object[] { _test3, 1, "Failed to Required Check. FieldName = Str2" },
            new object[] { _test4, 1, "Failed to StrLen Check. FieldName = Str3" },
            new object[] { _test5, 1, "Failed to Regex Check. FieldName = Str4" },
        };
    }
}
