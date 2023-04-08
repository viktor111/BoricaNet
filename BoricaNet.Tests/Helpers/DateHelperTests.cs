using BoricaNet.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoricaNet.Tests.Helpers
{
    public class DateHelperTests
    {
        public void GetTimezoneOffset_ShouldReturnCorrectOffset()
        {
            string expectedOffset = DateTimeOffset.Now.ToString("zz");
            string actualOffset = DateHelper.GetTimezoneOffset();

            Assert.Equal(expectedOffset, actualOffset);
        }

        [Fact]
        public void FormatDateForBorica_ShouldReturnCorrectFormattedDate()
        {
            DateTime inputDate = new DateTime(2022, 11, 30, 15, 30, 45, DateTimeKind.Local);
            string expectedFormattedDate = inputDate.ToUniversalTime().ToString("yyyyMMddHHmmss");

            string actualFormattedDate = DateHelper.FormatDateForBorica(inputDate);

            Assert.Equal(expectedFormattedDate, actualFormattedDate);
        }
    }
}
