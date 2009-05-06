using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class SimplePostTest
   {
      [Test]
      public void Test()
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/");

         engine.Select("li#simple-post-test a").Follow();

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/simple_post_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         const string textBoxValue = "Textbox1";
         const string textAreaValue = "Textarea1";

         engine.Select("form").Submit(new Preselection
         {
            { "p.textbox input", input => input.Value(textBoxValue) },
            { "p.radio input", radio => radio.Check() },
            { "p.textarea textarea", textarea => textarea.Value(textAreaValue) },
            { "p.select select", select => select.Choose(3) },
         });

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         string submittedTextBoxValue = engine.Select("p.textbox span.value").InnerText;
         string submittedTextAreaValue = engine.Select("p.textarea span.value").InnerText;
         string submittedRadioButtonValue = engine.Select("p.radio span.value").InnerText;
         string submittedSelectValue = engine.Select("p.select span.value").InnerText;
         string submittedSubmitValue = engine.Select("p.submit span.value").InnerText;

         Assert.AreEqual(textBoxValue,
                         submittedTextBoxValue,
                         "The submitted value of the textbox is invalid.");

         Assert.AreEqual(textAreaValue,
                         submittedTextAreaValue,
                         "The submitted value of the textarea is invalid.");

         Assert.AreEqual("on",
                         submittedRadioButtonValue,
                         "The submitted value of the radio button is invalid.");

         Assert.AreEqual("4",
                         submittedSelectValue,
                         "The submitted value of the 'select' element is invalid.");

         Assert.AreEqual("Submit",
                         submittedSubmitValue,
                         "The submitted value of the submit button is invalid.");
      }
   }
}