using System.Text;
using System.Xml;
using HtmlAgilityPack;
using System.Reflection;

namespace Satori.Protocol.Elements;

public static partial class ElementSerializer {
    private readonly static string[] ElementPropertyNames =
        typeof(Element).GetProperties().Select(elementProp => elementProp.Name).ToArray();

    private static HtmlNode GetHtmlNode(HtmlDocument document, Element element) {
        if (element is TextElement te) {
            return document.CreateTextNode(te.Text);
        }

        if (element.TagName is null) {
            throw ElementException.TagNameIsNull(element.GetType());
        }

        HtmlNode? htmlElement = document.CreateElement(element.TagName);

        PropertyInfo[]? props = element.GetType().GetProperties();

        foreach (PropertyInfo? prop in props) {
            if (ElementPropertyNames.Contains(prop.Name)) {
                continue;
            }

            if (prop.GetValue(element) is not { } attrVal) {
                continue;
            }

            string? attrName = ConvertPascalToKebab(prop.Name);

            htmlElement.SetAttributeValue(attrName, attrVal.ToString());
        }

        return htmlElement;
    }

    private static string WriteHtmlNode(HtmlNode node) {
        using StringWriter? strWriter = new StringWriter();
        using XmlWriter? xmlWriter = XmlWriter.Create(strWriter, new XmlWriterSettings {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = false,
            }
        );

        node.WriteTo(xmlWriter);
        xmlWriter.Flush();
        return strWriter.ToString();
    }

    public static string Serialize(Element element) {
        HtmlDocument? document = new HtmlDocument();
        HtmlNode? htmlNode = GetHtmlNode(document, element);

        foreach (Element? childElement in element.ChildElements) {
            htmlNode.AppendChild(GetHtmlNode(document, childElement));
        }

        return WriteHtmlNode(htmlNode);
    }

    public static string Serialize(Element[] elements) {
        HtmlDocument? document = new HtmlDocument();
        StringBuilder? sb = new StringBuilder();

        foreach (Element? element in elements) {
            HtmlNode? xmlNode = GetHtmlNode(document, element);

            foreach (Element? childElement in element.ChildElements) {
                xmlNode.AppendChild(GetHtmlNode(document, childElement));
            }

            sb.Append(WriteHtmlNode(xmlNode));
        }

        return sb.ToString();
    }
}
