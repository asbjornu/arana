using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class SimplePostTest
   {
      /// <summary>
      /// Initializes the engine and follows the "simple post test" anchor.
      /// </summary>
      /// <returns></returns>
      private static AranaEngine Initialize()
      {
         AranaEngine engine = new AranaEngine("http://test.aranalib.net/")
            .Select("li#simple-post-test a")
            .Follow();

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         return engine;
      }


      [Test]
      public void CheckAndUncheck()
      {
         AranaEngine engine = Initialize();

         Assert.AreEqual("/simple_post_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         engine.Select("form").Submit(new Preselection
         {
            { "p.radio1 input", radio => radio.Check() },
            { "p.checkbox input", checkbox => checkbox.Check() },
         });

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("on",
                         engine.Select("p.radio1 span.value").InnerText,
                         "The submitted value of the radio button is invalid.");

         Assert.AreEqual("on",
                         engine.Select("p.checkbox span.value").InnerText,
                         "The submitted value of the checkbox is invalid.");

         engine.Select("form").Submit(new Preselection
         {
            { "p.radio1 input", radio => radio.Uncheck() },
            { "p.checkbox input", checkbox => checkbox.Uncheck() },
         });

         Assert.IsEmpty(engine.Select("p.radio1 span.value").InnerText,
                        "The radio button should not have submitted any value.");

         Assert.IsEmpty(engine.Select("p.checkbox span.value").InnerText,
                        "The checkbox should not have submitted any value.");
      }


      [Test]
      public void EmptySubmit()
      {
         AranaEngine engine = Initialize();

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
         string submittedCheckBoxWithValueValue =
            engine.Select("p.checkbox-with-value span.value").InnerText;
         string submittedSelectValue = engine.Select("p.select span.value").InnerText;
         string submittedFirstSubmitValue =
            engine.Select("p.first-submit span.value").InnerText;
         string submittedSecondSubmitValue =
            engine.Select("p.second-submit span.value").InnerText;

         Assert.IsEmpty(submittedTextBoxValue,
                        "The textbox should not have submitted any value.");

         Assert.IsEmpty(submittedTextAreaValue,
                        "The textarea should not have submitted any value.");

         Assert.IsEmpty(submittedRadioButtonValue,
                        "The radio button should not have submitted any value.");

         Assert.IsEmpty(submittedCheckBoxValue,
                        "The checkbox should not have submitted any value.");

         Assert.IsEmpty(submittedCheckBoxWithValueValue,
                        "The checkbox should not have submitted any value.");

         Assert.IsEmpty(submittedSelectValue,
                        "The 'select' element should not have submitted any value.");

         Assert.IsEmpty(submittedFirstSubmitValue,
                        "The first submit button should not have submitted any value.");

         Assert.IsEmpty(submittedSecondSubmitValue,
                        "The second submit button should not have submitted any value.");
      }


      [Test]
      public void MultiButtonSubmit()
      {
         AranaEngine engine = Initialize();

         Assert.AreEqual("/simple_post_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         engine.Select("form").Submit("input#first-submit");

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         string submittedFirstSubmitValue =
            engine.Select("p.first-submit span.value").InnerText;

         Assert.AreEqual("First submit",
                         submittedFirstSubmitValue,
                         "The submitted value of the first submit button is invalid.");

         engine.Select("form").Submit("input#second-submit");

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         string submittedSecondSubmitValue =
            engine.Select("p.second-submit span.value").InnerText;

         Assert.AreEqual("Second submit",
                         submittedSecondSubmitValue,
                         "The submitted value of the second submit button is invalid.");
      }


      [Test]
      public void SimpleSubmit()
      {
         AranaEngine engine = Initialize();

         Assert.AreEqual("/simple_post_test/",
                         engine.Uri.PathAndQuery,
                         "The URI is incorrect.");

         const string textBoxValue = "Textbox1";
         const string textAreaValue = "Textarea1";

         engine.Select("form").Submit(new Preselection
         {
            { "p.textbox input", input => input.Value(textBoxValue) },
            { "p.radio1 input", radio => radio.Check() },
            { "p.checkbox input", checkbox => checkbox.Check() },
            { "p.checkbox-with-value input", checkbox => checkbox.Check() },
            { "p.textarea textarea", textarea => textarea.Value(textAreaValue) },
            { "p.select select", select => select.Choose(3) },
         });

         Assert.AreEqual(200,
                         engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         string submittedTextBoxValue = engine.Select("p.textbox span.value").InnerText;
         string submittedTextAreaValue = engine.Select("p.textarea span.value").InnerText;
         string submittedRadioButtonValue = engine.Select("p.radio1 span.value").InnerText;
         string submittedCheckBoxValue = engine.Select("p.checkbox span.value").InnerText;
         string submittedCheckBoxWithValueValue =
            engine.Select("p.checkbox-with-value span.value").InnerText;
         string submittedSelectValue = engine.Select("p.select span.value").InnerText;
         string submittedFirstSubmitValue = engine.Select("p.submit span.value").InnerText;
         string submittedSecondSubmitValue =
            engine.Select("p.second-submit span.value").InnerText;

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

         Assert.IsEmpty(submittedFirstSubmitValue,
                        "The first submit button should not have submitted any value.");

         Assert.IsEmpty(submittedSecondSubmitValue,
                        "The second submit button should not have submitted any value.");
      }
   }
}