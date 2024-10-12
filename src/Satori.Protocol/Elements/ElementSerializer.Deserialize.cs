using System.ComponentModel;
using HtmlAgilityPack;
using System.Reflection;

namespace Satori.Protocol.Elements;

public static partial class ElementSerializer {
    private readonly static Type[] BuiltinElementTypes = {
        // 基础元素
        typeof(TextElement), typeof(AtElement), typeof(SharpElement), typeof(LinkElement),
        // 资源元素
        typeof(ImageElement), typeof(AudioElement), typeof(VideoElement), typeof(FileElement),
        // 修饰元素
        typeof(BoldElement), typeof(ItalicElement), typeof(UnderlineElement), typeof(DeleteElement),
        typeof(SpoilerElement), typeof(CodeElement), typeof(SuperscriptElement), typeof(SubscriptElement),
        // 排版元素
        typeof(BreakElement), typeof(ParagraphElement), typeof(MessageElement),
        // 元信息元素
        typeof(QuoteElement), typeof(AuthorElement),
    };

    private static Dictionary<string, Type>? _builtinElementMap;

    private static Dictionary<string, Type> GetBuiltinElementMap() {
        Dictionary<string, Type>? map = new Dictionary<string, Type>();

        foreach (Type? type in BuiltinElementTypes) {
            Element? element = (Element)Activator.CreateInstance(type)!;

            if (element.TagName is null) {
                continue;
            }

            map[element.TagName] = type;

            if (element.AlternativeTagNames is null) {
                continue;
            }

            foreach (string? tag in element.AlternativeTagNames) {
                map[tag] = type;
            }
        }

        return map;
    }

    /// <summary>
    /// 递归调用
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="map"></param>
    /// <param name="node"></param>
    private static void AddChildElements(IList<Element> parent, IDictionary<string, Type> map, HtmlNode node) {
        Element? element = null;

        switch (node.NodeType) {
        // 纯文本直接为 TextElement
        case HtmlNodeType.Text:
            element = new TextElement { Text = node.InnerText };
            break;

        case HtmlNodeType.Element:
            if (map.TryGetValue(node.Name, out Type? type)) {
                element = (Element)Activator.CreateInstance(type)!;

                foreach (HtmlAttribute? attr in node.Attributes) {
                    element.Attributes[attr.Name] = attr.Value;
                    string? propName = ConvertKebabToPascal(attr.Name);
                    PropertyInfo? prop = type.GetProperty(propName);

                    if (prop is null) {
                        continue;
                    }

                    object? value;

                    // 对于一个 HTML Boolean Attributes，只要出现了就直接设为 true
                    if (prop.PropertyType == typeof(bool?) || prop.PropertyType == typeof(bool)) {
                        value = true;
                    } else {
                        value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromString(attr.Value);
                    }

                    prop.SetValue(element, value);
                }
            } else {
                element = new Element();

                foreach (HtmlAttribute? attrObj in node.Attributes) {
                    HtmlAttribute? attr = (HtmlAttribute)attrObj;
                    element.Attributes[attr.Name] = attr.Value;
                }
            }

            foreach (HtmlNode? childNode in node.ChildNodes) {
                AddChildElements(element.ChildElements, map, childNode);
            }

            break;
        }

        if (element is not null) {
            parent.Add(element);
        }
    }

    public static IEnumerable<Element> Deserialize(
        string content, IDictionary<string, Type>? externalElementMap = null
    ) {
        _builtinElementMap ??= GetBuiltinElementMap();

        Dictionary<string, Type>? map = externalElementMap is not null
            ? new Dictionary<string, Type>(_builtinElementMap.Concat(externalElementMap))
            : _builtinElementMap;

        HtmlDocument? document = new HtmlDocument();
        document.LoadHtml(content);

        List<Element>? list = new List<Element>();

        foreach (HtmlNode? htmlNode in document.DocumentNode.ChildNodes) {
            AddChildElements(list, map, htmlNode);
        }

        return list;
    }
}
