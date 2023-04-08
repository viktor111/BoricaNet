using BoricaNet.Exceptions;
using BoricaNet.Helpers;

namespace BoricaNet.Tests.Helpers
{
    public class GeneratorTests
    {
        [Fact]
        public void ByteArrayToHexString_GivenNullInput_ShouldThrowException()
        {
            Assert.Throws<BoricaNetException>(() => Generator.ByteArrayToHexString(null));
        }

        [Fact]
        public void ByteArrayToHexString_GivenValidInput_ShouldReturnHexString()
        {
            byte[] input = { 0x12, 0x34, 0xAB };
            string expectedOutput = "1234AB";

            string result = Generator.ByteArrayToHexString(input);

            Assert.Equal(expectedOutput, result);
        }

        [Theory]
        [InlineData(-1, 5)]
        [InlineData(5, 3)]
        public void GenerateRandomByteArray_GivenInvalidLengths_ShouldThrowException(int minLength, int maxLength)
        {
            Assert.Throws<BoricaNetException>(() => Generator.GenerateRandomByteArray(minLength, maxLength));
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(2, 10)]
        public void GenerateRandomByteArray_GivenValidLengths_ShouldReturnArrayWithValidLength(int minLength, int maxLength)
        {
            byte[] result = Generator.GenerateRandomByteArray(minLength, maxLength);

            Assert.NotNull(result);
            Assert.True(result.Length >= minLength && result.Length <= maxLength);
        }

        [Fact]
        public void GenerateOrderNumber_GivenValidNonce_ShouldReturnSixDigitOrderNumber()
        {
            string nonce = "test_nonce";

            int orderNumber = Generator.GenerateOrderNumber(nonce);

            Assert.True(orderNumber >= 100000 && orderNumber <= 999999);
        }
    }
}
