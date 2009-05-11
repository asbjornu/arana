using System;
using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class PostTest
   {
      /// <summary>
      /// Initializes the engine and follows the anchor found with the given <paramref name="anchorSelector"/>
      /// </summary>
      /// <param name="anchorSelector">The anchor selector.</param>
      /// <returns></returns>
      private static AranaEngine Initialize(string anchorSelector)
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/")
            .Select(anchorSelector)
            .Follow();

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         return engine;
      }


      [Test]
      public void EmptySubmit()
      {
         AranaEngine engine = Initialize("li#simple-post-test a");

         Assert.AreEqual("/simple_post_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         engine.Select("form").Submit();

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         string submittedTextBoxValue = engine.Select("p.textbox span.value").InnerText;
         string submittedTextAreaValue = engine.Select("p.textarea span.value").InnerText;
         string submittedRadioButtonValue = engine.Select("p.radio span.value").InnerText;
         string submittedCheckBoxValue = engine.Select("p.checkbox span.value").InnerText;
         string submittedSelectValue = engine.Select("p.select span.value").InnerText;
         string submittedSubmitValue = engine.Select("p.submit span.value").InnerText;

         Assert.IsEmpty(submittedTextBoxValue,
                        "The textbox should not have submitted any value.");

         Assert.IsEmpty(submittedTextAreaValue,
                        "The textarea should not have submitted any value.");

         Assert.IsEmpty(submittedRadioButtonValue,
                        "The radio button should not have submitted any value.");

         Assert.IsEmpty(submittedCheckBoxValue,
                        "The checkbox should not have submitted any value.");

         Assert.IsEmpty(submittedSelectValue,
                        "The 'select' element should not have submitted any value.");

         Assert.AreEqual("Submit",
                         submittedSubmitValue,
                         "The submitted value of the submit button is invalid.");
      }

      [Test]
      public void SimpleSubmit()
      {
         AranaEngine engine = Initialize("li#simple-post-test a");

         Assert.AreEqual("/simple_post_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         const string textBoxValue = "Textbox1";
         const string textAreaValue = "Textarea1";

         engine.Select("form").Submit(new Preselection
         {
            { "p.textbox input", input => input.Value(textBoxValue) },
            { "p.radio input", radio => radio.Check() },
            { "p.checkbox input", checkbox => checkbox.Check() },
            { "p.checkbox-with-value input", checkbox => checkbox.Check() },
            { "p.textarea textarea", textarea => textarea.Value(textAreaValue) },
            { "p.select select", select => select.Choose(3) },
         });

         Console.WriteLine(engine.Response);

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         string submittedTextBoxValue = engine.Select("p.textbox span.value").InnerText;
         string submittedTextAreaValue = engine.Select("p.textarea span.value").InnerText;
         string submittedRadioButtonValue = engine.Select("p.radio span.value").InnerText;
         string submittedCheckBoxValue = engine.Select("p.checkbox span.value").InnerText;
         string submittedCheckBoxWithValueValue =
            engine.Select("p.checkbox-with-value span.value").InnerText;
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

         Assert.AreEqual("on",
                         submittedCheckBoxValue,
                         "The submitted value of the checkbox is invalid.");

         Assert.IsNotEmpty(submittedCheckBoxWithValueValue,
                           "The submitted value of the checkbox is invalid.");

         Assert.AreEqual("4",
                         submittedSelectValue,
                         "The submitted value of the 'select' element is invalid.");

         Assert.AreEqual("Submit",
                         submittedSubmitValue,
                         "The submitted value of the submit button is invalid.");
      }
   }
}