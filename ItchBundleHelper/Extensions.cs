using Notion.Client;

namespace ItchBundleHelper
{
    public static class Extensions
    {
        public static List<RichTextBase> ToRichTextList(this string value)
        {
            return new List<RichTextBase>()
            {
                new RichTextText()
                {
                    Text = new Text
                    {
                        Content = value ?? "",
                    }
                }
            };
        }

        public static TitlePropertyValue ToTitlePropertyValue(this string text)
        {
            return new TitlePropertyValue()
            {
                Title = text.ToRichTextList(),
            };
        }

        public static NumberPropertyValue ToNumberPropertyValue(this double value)
        {
            return new NumberPropertyValue()
            {
                Number = value,
            };
        }

        public static UrlPropertyValue ToUrlPropertyValue(this string value)
        {
            return new UrlPropertyValue()
            {
                // Url can't be null or empty, so put something there if it is
                Url = string.IsNullOrEmpty(value) ? "http://example.invalid" : value,
            };
        }

        public static RichTextPropertyValue ToRichTextPropertyValue(this string value)
        {
            return new RichTextPropertyValue
            {
                RichText = value.ToRichTextList(),
            };
        }
        public static CheckboxPropertyValue ToCheckboxPropertyValue(this bool value)
        {
            return new CheckboxPropertyValue
            {
                Checkbox = value,
            };
        }

        public static List<RichTextBaseInput> ToInputList(this string text)
        {
            return new List<RichTextBaseInput>
            {
                new RichTextTextInput
                {
                    Text = new Text
                    {
                        Content = text ?? "",
                        Link = null,
                    }
                }
            };
        }
    }
}
