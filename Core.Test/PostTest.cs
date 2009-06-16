using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test
{
   [TestFixture]
   public class PostTest : TestBase
   {
      /// <summary>
      /// Initializes the Engine and follows the "simple post test" anchor.
      /// </summary>
      /// <returns></returns>
      [SetUp]
      public override void SetUp()
      {
         base.SetUp();

         Engine.Select("li#simple-post-test a").Follow();

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("/simple_post_test/",
                         Engine.Uri.PathAndQuery,
                         "The URI is incorrect.");
      }


      [Test]
      public void CheckAndUncheck()
      {
         Engine.Select("form").Submit(new Preselection
         {
            { "p.radio1 input", radio => radio.Check() },
            { "p.checkbox input", checkbox => checkbox.Check() },
         });

         Assert.AreEqual(200,
                         Engine.Response.StatusBase,
                         "The HTTP status code is invalid.");

         Assert.AreEqual("on",
                         Engine.Select("p.radio1 span.value").InnerText,
                         "The submitted value of the radio button is invalid.");

         Assert.AreEqual("on",
                         Engine.Select("p.checkbox span.value").InnerText,
                         "The submitted value of the checkbox is invalid.");

         Engine.Select("form").Submit(new Preselection
         {
            { "p.radio1 input", radio => radio.Uncheck() },
            { "p.checkbox input", checkbox => checkbox.Uncheck() },
         });

         Assert.IsEmpty(Engine.Select("p.radio1 span.value").InnerText,
                        "The radio button should not have submitted any value.");

         Assert.IsEmpty(Engine.Select("p.checkbox span.value").InnerText,
                        "The checkbox should not have submitted any value.");
      }


      [Test]
      public void EmptySubmit()
      {
         Engine.Select("form").Submit();

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         string submittedTextBoxValue = Engine.Select("p.textbox span.value").InnerText;
         string submittedTextAreaValue = Engine.Select("p.textarea span.value").InnerText;
         string submittedRadioButtonValue = Engine.Select("p.radio span.value").InnerText;
         string submittedCheckBoxValue = Engine.Select("p.checkbox span.value").InnerText;
         string submittedCheckBoxWithValueValue =
            Engine.Select("p.checkbox-with-value span.value").InnerText;
         string submittedSelectValue = Engine.Select("p.select span.value").InnerText;
         string submittedFirstSubmitValue =
            Engine.Select("p.first-submit span.value").InnerText;
         string submittedSecondSubmitValue =
            Engine.Select("p.second-submit span.value").InnerText;

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
         Engine.Select("form").Submit("input#first-submit");

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         string submittedFirstSubmitValue =
            Engine.Select("p.first-submit span.value").InnerText;

         Assert.AreEqual("First submit",
                         submittedFirstSubmitValue,
                         "The submitted value of the first submit button is invalid.");

         Engine.Select("form").Submit("input#second-submit");

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         string submittedSecondSubmitValue =
            Engine.Select("p.second-submit span.value").InnerText;

         Assert.AreEqual("Second submit",
                         submittedSecondSubmitValue,
                         "The submitted value of the second submit button is invalid.");
      }


      [Test]
      public void SimpleSubmit()
      {
         const string textBoxValue = "Textbox1";
         const string textAreaValue = "Textarea1";

         Engine.Select("form").Submit(new Preselection
         {
            { "p.textbox input", input => input.Value(textBoxValue) },
            { "p.radio1 input", radio => radio.Check() },
            { "p.checkbox input", checkbox => checkbox.Check() },
            { "p.checkbox-with-value input", checkbox => checkbox.Check() },
            { "p.textarea textarea", textarea => textarea.Value(textAreaValue) },
            { "p.select select", select => select.Choose(3) },
         });

         Assert.AreEqual(HttpStatusCode.OK,
                         Engine.Response.Status,
                         "The HTTP status code is invalid.");

         string submittedTextBoxValue = Engine.Select("p.textbox span.value").InnerText;
         string submittedTextAreaValue = Engine.Select("p.textarea span.value").InnerText;
         string submittedRadioButtonValue = Engine.Select("p.radio1 span.value").InnerText;
         string submittedCheckBoxValue = Engine.Select("p.checkbox span.value").InnerText;
         string submittedCheckBoxWithValueValue =
            Engine.Select("p.checkbox-with-value span.value").InnerText;
         string submittedSelectValue = Engine.Select("p.select span.value").InnerText;
         string submittedFirstSubmitValue = Engine.Select("p.submit span.value").InnerText;
         string submittedSecondSubmitValue =
            Engine.Select("p.second-submit span.value").InnerText;

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