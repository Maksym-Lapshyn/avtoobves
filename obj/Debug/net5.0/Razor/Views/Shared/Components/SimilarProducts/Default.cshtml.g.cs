#pragma checksum "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "243c036632a955764f91083f2fe1950464849c5d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Components_SimilarProducts_Default), @"mvc.1.0.view", @"/Views/Shared/Components/SimilarProducts/Default.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "/Users/mlaps/Documents/Projects/Avtoobves/Views/_ViewImports.cshtml"
using Avtoobves;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/mlaps/Documents/Projects/Avtoobves/Views/_ViewImports.cshtml"
using Avtoobves.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"243c036632a955764f91083f2fe1950464849c5d", @"/Views/Shared/Components/SimilarProducts/Default.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"977054466d3773d89a314795b95bcf55f4e15502", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared_Components_SimilarProducts_Default : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Product>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 3 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
   
    bool first = false;

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<div class=\"col-xs-12 wow fadeInUp delay-01s\">\n    <div class=\"carousel slide\" data-ride=\"carousel\" data-type=\"multi\" data-interval=\"5000\" id=\"myCarousel\">\n        <div class=\"carousel-inner\">\n");
#nullable restore
#line 10 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
             foreach (Product product in Model)
            {
                if (!first)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div class=\"item active\">\n                        <div class=\"col-md-3 col-xs-6\">\n                            <a");
            BeginWriteAttribute("href", " href=\"", 493, "\"", 562, 1);
#nullable restore
#line 16 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 500, Url.Action("Product", "Home", new { productId = product.Id }), 500, 62, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\n                                <div class=\"portfolio-box\">\n                                    <img");
            BeginWriteAttribute("src", " src=\"", 665, "\"", 700, 2);
            WriteAttributeValue("", 671, "../Images/", 671, 10, true);
#nullable restore
#line 18 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 681, product.SmallImage, 681, 19, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("alt", " alt=\"", 701, "\"", 720, 1);
#nullable restore
#line 18 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 707, product.Name, 707, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("name", " name=\"", 721, "\"", 741, 1);
#nullable restore
#line 18 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 728, product.Name, 728, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"img-responsive\">\n                                    <div class=\"portfolio-box-caption\">\n                                        <div class=\"portfolio-box-caption-content-smaller\">\n                                            <h1>");
#nullable restore
#line 21 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
                                           Write(product.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\n                                        </div>\n                                    </div>\n                                </div>\n                            </a>\n                        </div>\n                    </div>\n");
#nullable restore
#line 28 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
                    first = true;
                }
                else
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div class=\"item\">\n                        <div class=\"col-md-3 col-xs-6\">\n                            <a");
            BeginWriteAttribute("href", " href=\"", 1434, "\"", 1503, 1);
#nullable restore
#line 34 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 1441, Url.Action("Product", "Home", new { productId = product.Id }), 1441, 62, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\n                                <div class=\"portfolio-box\">\n                                    <img");
            BeginWriteAttribute("src", " src=\"", 1606, "\"", 1641, 2);
            WriteAttributeValue("", 1612, "../Images/", 1612, 10, true);
#nullable restore
#line 36 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 1622, product.SmallImage, 1622, 19, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("alt", " alt=\"", 1642, "\"", 1661, 1);
#nullable restore
#line 36 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 1648, product.Name, 1648, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            BeginWriteAttribute("name", " name=\"", 1662, "\"", 1682, 1);
#nullable restore
#line 36 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
WriteAttributeValue("", 1669, product.Name, 1669, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"img-responsive\">\n                                    <div class=\"portfolio-box-caption\">\n                                        <div class=\"portfolio-box-caption-content-smaller\">\n                                            <h1>");
#nullable restore
#line 39 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
                                           Write(product.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\n                                        </div>\n                                    </div>\n                                </div>\n                            </a>\n                        </div>\n                    </div>\n");
#nullable restore
#line 46 "/Users/mlaps/Documents/Projects/Avtoobves/Views/Shared/Components/SimilarProducts/Default.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        </div>
        <a class=""left carousel-control"" href=""#myCarousel"" data-slide=""prev""><i class=""glyphicon glyphicon-chevron-left""></i></a>
        <a class=""right carousel-control"" href=""#myCarousel"" data-slide=""next""><i class=""glyphicon glyphicon-chevron-right""></i></a>
    </div>
</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Product>> Html { get; private set; }
    }
}
#pragma warning restore 1591
