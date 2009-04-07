using System;
using System.Collections.Specialized;
using System.Security.Cryptography;

using Arana.Core;

namespace Arana.Test.ConsoleApplication
{
   internal static class Program
   {
      private static AranaEngine engine;


      /// <summary>
      /// Gets a random hash.
      /// </summary>
      /// <returns>A random hash.</returns>
      private static string GetRandomHash()
      {
         using (MD5 md5 = MD5.Create())
         {
            byte[] data = md5.ComputeHash(Guid.NewGuid().ToByteArray());
            return BitConverter.ToString(data).Replace("-", String.Empty).ToLowerInvariant();
         }
      }


      private static void Main()
      {
         try
         {
            TestArana();
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }

         PressAnyKey("Press any key to exit.");
      }


      private static void PressAnyKey(string message)
      {
         Console.WriteLine();
         Console.WriteLine();
         Console.WriteLine(message);
         Console.WriteLine();
         Console.ReadKey(true);
      }


      /// <summary>
      /// Initializes the order process by following the "order" link.
      /// </summary>
      private static void Step1()
      {
         string header = engine
            .Select("div.order a")
            .Follow()
            .Select("div.content-middle>h1").InnerText;

         Console.WriteLine(header);
      }


      /// <summary>
      /// Selects the "oslo" ticket type and submits the form.
      /// </summary>
      private static void Step2()
      {
         string osloTicketTypeValue = engine.Select("input#ticket-type-oslo").Value();

         Console.WriteLine(osloTicketTypeValue);

         engine.Select("form#aspnetForm").Submit(new NameValueCollection
         {
            { "ticket-type", osloTicketTypeValue }
         });

         string header = engine.Select("div.content-middle>h1").InnerText;

         Console.WriteLine(header);
      }


      /// <summary>
      /// Selects the first product alternative and submits the form.
      /// </summary>
      private static void Step3()
      {
         string productAlternativeValue = engine.Select(
            "table.products td.alternative input[name='product-alternative']").Value();

         Console.WriteLine(productAlternativeValue);

         engine.Select("form#aspnetForm").Submit(new NameValueCollection
         {
            { "product-alternative", productAlternativeValue }
         });

         string header = engine.Select("div.content-middle>h1").InnerText;

         Console.WriteLine(header);
      }


      /// <summary>
      /// Fills out the card user information and submits the form.
      /// </summary>
      private static void Step4()
      {
         engine.Select("form#aspnetForm").Submit(new NameValueCollection
         {
            { "card-name", "Donald Duck" },
            { "birth-day", "24" },
            { "birth-month", "07" },
            { "birth-year", "1975" },
         });

         string header = engine.Select("div.content-middle>h1").InnerText;

         Console.WriteLine(header);
      }


      /// <summary>
      /// Fills out the registration form and submits.
      /// </summary>
      private static void Step5()
      {
         string hash = GetRandomHash();
         string email = String.Format("donald.duck.{0}@okb.no", hash);
         string genderValue = engine.Select("li.gender input#gender-male").Value();

         Console.WriteLine(genderValue);

         engine.Select("form#aspnetForm").Submit(new NameValueCollection
         {
            { "first-name", "Donald" },
            { "last-name", "Duck" },
            { "birth-day", "24" },
            { "birth-month", "07" },
            { "birth-year", "1975" },
            { "email", email },
            { "email-again", email },
            { "password", "okb123" },
            { "verified-password", "okb123" },
            { "cellphone-number", "98765421" },
            { "Address", "Kranglefjellveien c.o Industrirøykepassasjen 14, leilighet 1088" },
            { "postal-code", "6544" },
            { "locality", "Andeby" },
            { "gender", genderValue },
            { "accept-information-sharing", "1" },
         });

         string title = engine.Select("title").InnerText;

         Console.WriteLine(title);
      }


      /// <summary>
      /// Fills out the credit card information and submits the form.
      /// </summary>
      private static void Step6()
      {
         string type = engine.Select(
            "select#ctl00_BodyContentPlaceHolder_PaymentControl_ddCardType").Name();

         string number = engine.Select(
            "input#ctl00_BodyContentPlaceHolder_PaymentControl_txtCardNumber").Name();

         string holder = engine.Select(
            "input#ctl00_BodyContentPlaceHolder_PaymentControl_txtCardHolderName").Name();

         string expirationMonth = engine.Select(
            "select#ctl00_BodyContentPlaceHolder_PaymentControl_ddExpireMonth").Name();

         string expirationYear = engine.Select(
            "select#ctl00_BodyContentPlaceHolder_PaymentControl_ddExpireYear").Name();

         string cvc = engine.Select(
            "input#ctl00_BodyContentPlaceHolder_PaymentControl_txtCVCCode").Name();

         Console.WriteLine(type, "The 'card type' input field's name could not be found.");
         Console.WriteLine(number, "The 'card number' input field's name could not be found.");
         Console.WriteLine(holder, "The 'card holder' input field's name could not be found.");
         Console.WriteLine(expirationMonth,
                           "The 'card expiration month' input field's name could not be found.");
         Console.WriteLine(expirationYear,
                           "The 'card expiration year' input field's name could not be found.");
         Console.WriteLine(cvc, "The 'card CVC' input field's name could not be found.");

         engine.Select("form#aspnetForm").Submit(new NameValueCollection
         {
            { type, "VISA" },
            { number, "4925000000000004" },
            { holder, "Donald Duck" },
            { expirationMonth, "8" },
            { expirationYear, "2010" },
            { cvc, "123" },
         });

         Console.WriteLine(engine.Uri);
         Console.WriteLine(engine.Response.Body);

         string header = engine.Select("div.content-middle>h1").InnerText;

         Console.WriteLine(header);
      }


      private static void TestArana()
      {
         engine = new AranaEngine(Settings.WebSiteUri);
         Step1();
         Step2();
         Step3();
         Step4();
         Step5();
         Step6();
      }
   }
}