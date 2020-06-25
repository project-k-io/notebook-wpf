using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectK.Utils.Tests
{
    [TestClass]
    public class StringHelperTest
    {
        [TestMethod]
        public void ConvertTextInMultipleLinesTest()
        {
            //      0         1         2         3         4         5         6         7     
            // 01234567890123456789012345678901234567890123456789012345678901234567890123456789 - Text     - input
            // with disruptive business models and/or that are building products and services for markets / demographics that have historically been underserved\r\n"
            // 'with disruptive business models and/or that are building products and services '
            // 'for markets / demographics that have historically been underserved\r\n"'


            // Arrange
            var text  = "with disruptive business models and/or that are building products and services for markets / demographics that have historically been underserved\r\n";
            var line0 = "with disruptive business models and/or that are building products and services ";
            var line1 = "for markets / demographics that have historically been underserved\r\n";

            var maxLength = 80;

            // Act
            var lines = StringHelper.ConvertTextInMultipleLines(text, maxLength);

            // Assert
            //AK double actual = account.Balance;
            //AK Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");

            Assert.AreEqual(lines.Count, 2);
            Assert.AreEqual(lines[0], line0);
            Assert.AreEqual(lines[1], line1);
        }
    }
}
