= Araña web testing library =

Araña ("spider" in Spanish) is a simple web testing library, written in C#. It can be used to integrate simple testing of web applications into unit testing, so the parts of your web application can be tested separately as well as how they work together.

Araña can follow links, post forms and, through simple CSS selectors, ensure that the content on the pages of your web application is what you expect, and thus can be tested with unit test assert statements.

Araña uses the [http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx HttpWebRequest] and [http://msdn.microsoft.com/en-us/library/system.net.httpwebresponse.aspx HttpWebResponse] objects to fetch HTML documents and the [http://code.google.com/p/fizzler/ Fizzler] library to select elements from it to perform actions on.

Araña works seamlessly with all unit testing frameworks and can be used in all open source applications running on either Microsoft.NET or Mono. The CSS querying capabilities is built with [http://jquery.com/ jQuery] in mind and will, where it fits, match the jQuery API.

= Examples =

== Follow a link ==

The following example;

  # Selects the {{{a}}} element with {{{@class="first"}}} inside the {{{ul}}} element with {{{@id="menu"}}}
  # Follows it to the referenced URI (defined in the {{{a@href}}} attribute).
  # Asserts that the title on the referenced page is equal to "Expected title".

{{{
// Create an Araña engine for a given web application
AranaEngine engine = new AranaEngine("http://example.com/");

// Select the anchor from the menu and follow it
engine.Select("ul#menu a.first").Follow();

// Assert that the expected title is correct on the resulting page
Assert.AreEqual("Expected title", engine.Select("title").InnerText);
}}}

== Post a form ==

The following example;
  
  # Selects the {{{form}}} element with {{{@id="aspnetForm"}}}
  # Submits the {{{form}}};
   # Without following redirect (indicated by the {{{false}}} argument to the {{{Submit()}}} method)
   # With the form elements named {{{username}}} and {{{password}}} set to the values specified in the given {{{NameValueCollection}}}.
  # Asserts that the location being redirected to is equal to "/success".

{{{
// Create an Araña engine for a given web application
AranaEngine engine = new AranaEngine("http://example.com/");

// Select the form and submit it without following redirect and with the given values.
engine.Select("form#aspnetForm").Submit(false, new NameValueCollection
{
  {"username", "Administrator"},
  {"password", "2fmckX32a"},
});

// Assert that the location being redirected to is what we expect
Assert.AreEqual("/success", engine.Response.Location);
}}}

== More examples ==

You can find more examples on the [Examples] page.
