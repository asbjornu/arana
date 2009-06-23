using System.Net;

using NUnit.Framework;

namespace Arana.Core.Test.Web
{
   [TestFixture]
   [Category("Web")]
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

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         Assert.That(Engine.Uri.PathAndQuery,
                     Is.EqualTo("/simple_post_test/"),
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

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         Assert.That(Engine.Select("p.radio1 span.value").InnerText,
                         Is.EqualTo("on"),
                         "The submitted value of the radio button is invalid.");

         Assert.That(Engine.Select("p.checkbox span.value").InnerText,
                         Is.EqualTo("on"),
                         "The submitted value of the checkbox is invalid.");

         Engine.Select("form").Submit(new Preselection
         {
            { "p.radio1 input", radio => radio.Uncheck() },
            { "p.checkbox input", checkbox => checkbox.Uncheck() },
         });

         Assert.That(Engine.Select("p.radio1 span.value").InnerText,
                     Is.Empty,
                     "The radio button should not have submitted any value.");

         Assert.That(Engine.Select("p.checkbox span.value").InnerText,
                     Is.Empty,
                     "The checkbox should not have submitted any value.");
      }


      [Test]
      public void EmptySubmit()
      {
         Engine.Select("form").Submit();

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         var submitted = new
         {
            TextBoxValue = Engine.Select("p.textbox span.value").InnerText,
            TextAreaValue = Engine.Select("p.textarea span.value").InnerText,
            RadioButtonValue = Engine.Select("p.radio span.value").InnerText,
            CheckBoxValue = Engine.Select("p.checkbox span.value").InnerText,
            CheckBoxWithValueValue =
               Engine.Select("p.checkbox-with-value span.value").InnerText,
            SelectValue = Engine.Select("p.select span.value").InnerText,
            FirstSubmitValue = Engine.Select("p.first-submit span.value").InnerText,
            SecondSubmitValue = Engine.Select("p.second-submit span.value").InnerText,
         };

         Assert.That(submitted.TextBoxValue,
                     Is.Empty,
                     "The textbox should not have submitted any value.");

         Assert.That(submitted.TextAreaValue,
                     Is.Empty,
                     "The textarea should not have submitted any value.");

         Assert.That(submitted.RadioButtonValue,
                     Is.Empty,
                     "The radio button should not have submitted any value.");

         Assert.That(submitted.CheckBoxValue,
                     Is.Empty,
                     "The checkbox should not have submitted any value.");

         Assert.That(submitted.CheckBoxWithValueValue,
                     Is.Empty,
                     "The checkbox should not have submitted any value.");

         Assert.That(submitted.SelectValue,
                     Is.Empty,
                     "The 'select' element should not have submitted any value.");

         Assert.That(submitted.FirstSubmitValue,
                     Is.Empty,
                     "The first submit button should not have submitted any value.");

         Assert.That(submitted.SecondSubmitValue,
                     Is.Empty,
                     "The second submit button should not have submitted any value.");
      }


      [Test]
      public void MultiButtonSubmit()
      {
         Engine.Select("form").Submit("input#first-submit");

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         string submittedFirstSubmitValue =
            Engine.Select("p.first-submit span.value").InnerText;

         Assert.That(submittedFirstSubmitValue,
                     Is.EqualTo("First submit"),
                     "The submitted value of the first submit button is invalid.");

         Engine.Select("form").Submit("input#second-submit");

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         string submittedSecondSubmitValue =
            Engine.Select("p.second-submit span.value").InnerText;

         Assert.That(submittedSecondSubmitValue,
                     Is.EqualTo("Second submit"),
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

         Assert.That(Engine.Response.Status,
                     Is.EqualTo(HttpStatusCode.OK),
                     "The HTTP status code is invalid.");

         var submitted = new
         {
            TextBoxValue = Engine.Select("p.textbox span.value").InnerText,
            TextAreaValue = Engine.Select("p.textarea span.value").InnerText,
            RadioButtonValue = Engine.Select("p.radio1 span.value").InnerText,
            CheckBoxValue = Engine.Select("p.checkbox span.value").InnerText,
            CheckBoxWithValueValue =
               Engine.Select("p.checkbox-with-value span.value").InnerText,
            SelectValue = Engine.Select("p.select span.value").InnerText,
            FirstSubmitValue = Engine.Select("p.submit span.value").InnerText,
            SecondSubmitValue = Engine.Select("p.second-submit span.value").InnerText,
         };

         Assert.That(submitted.TextBoxValue,
                     Is.EqualTo(textBoxValue),
                     "The submitted value of the textbox is invalid.");

         Assert.That(submitted.TextAreaValue,
                     Is.EqualTo(textAreaValue),
                     "The submitted value of the textarea is invalid.");

         Assert.That(submitted.RadioButtonValue,
                     Is.EqualTo("on"),
                     "The submitted value of the radio button is invalid.");

         Assert.That(submitted.CheckBoxValue,
                     Is.EqualTo("on"),
                     "The submitted value of the checkbox is invalid.");

         Assert.That(submitted.CheckBoxWithValueValue,
                     Is.Not.Empty,
                     "The submitted value of the checkbox is invalid.");

         Assert.That(submitted.SelectValue,
                     Is.EqualTo("4"),
                     "The submitted value of the 'select' element is invalid.");

         Assert.That(submitted.FirstSubmitValue,
                     Is.Empty,
                     "The first submit button should not have submitted any value.");

         Assert.That(submitted.SecondSubmitValue,
                     Is.Empty,
                     "The second submit button should not have submitted any value.");
      }
   }
}