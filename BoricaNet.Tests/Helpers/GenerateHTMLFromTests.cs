using BoricaNet.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoricaNet.Tests.Helpers
{
    public class GenerateHTMLFromTests
    {
        public class FormGeneratorTests
        {
            [Theory]
            [InlineData("{\"a\": \"1\", \"b\": \"2\"}", true, "<form method=\"post\" action=\"https://3dsgate-dev.borica.bg/cgi-bin/cgi_link\"><input type=\"hidden\" name=\"a\" size=\"1\" value=\"1\"><input type=\"hidden\" name=\"b\" size=\"1\" value=\"2\"><input type='submit' value='Submit'></form>")]
            [InlineData("{\"a\": \"1\", \"b\": \"2\"}", false, "<form method=\"post\" action=\"https://3dsgate.borica.bg/cgi-bin/cgi_link\"><input type=\"hidden\" name=\"a\" size=\"1\" value=\"1\"><input type=\"hidden\" name=\"b\" size=\"1\" value=\"2\"><input type='submit' value='Submit'></form>")]
            public void GenerateHTMLForm_GivenJsonAndEnvironment_ShouldReturnCorrectHtml(string json, bool isDev, string expectedHtml)
            {
                string generatedHtml = FormGenerator.GenerateHTMLForm(json, isDev);

                Assert.Equal(expectedHtml, generatedHtml);
            }
        }
    }
}
