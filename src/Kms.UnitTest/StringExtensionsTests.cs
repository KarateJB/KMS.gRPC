using Kms.Core.Utils.Extensions;
using NUnit.Framework;

namespace Kms.UnitTest
{
    public class StringExtensionsTests
    {
        private const string ALL_MASKED = "***********";

        [SetUp]
        public void Setup()
        {
        }


        [Test]
        [TestCase("1", ALL_MASKED)]
        [TestCase("12", ALL_MASKED)]
        [TestCase("123", ALL_MASKED)]
        [TestCase("1234", ALL_MASKED)]
        [TestCase("12345", ALL_MASKED)]
        [TestCase("123456", "12**56")]
        [TestCase("1234567", "12**567")]
        [TestCase("12345678", "12**5678")]
        [TestCase("123456789", "123***789")]
        [TestCase("123456789A", "123***789A")]
        [TestCase("123456789AB", "123***789AB")]
        [TestCase("123456789ABC", "1234****9ABC")]
        [TestCase("123456789ABCDEFGHIJKLMNOPQRSTUV", "123456******OPQRSTUV")]
        public void TestMask(string text, string expected)
        {
            var actual = text.Mask();
            Assert.AreEqual(expected, actual);
        }
    }
}