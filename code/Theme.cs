using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
﻿using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace Writer2
{
    public class Theme
    {
        public const string ThemesDirectoryPath = "Themes";

        public SolidColorBrush EditorCodeBackgroundColor { get; set; } = new SolidColorBrush(Colors.White);

        public string EditorCodeFontFamily { get; set; } = "Consolas";

        public double EditorCodeFontSize { get; set; } = 24D;

        public FontStretch EditorCodeFontStretch { get; set; } = FontStretches.Normal;

        public FontStyle EditorCodeFontStyle { get; set; } = FontStyles.Normal;

        public FontWeight EditorCodeFontWeight { get; set; } = FontWeights.Normal;

        public SolidColorBrush EditorCodeForegroundColor { get; set; } = new SolidColorBrush(Colors.Black);

        public double EditorCodeLineHeight { get; set; } = 32D;

        public TextAlignment EditorCodeTextAlignment { get; set; } = TextAlignment.Left;

        public SolidColorBrush EditorDefaultBackgroundColor { get; set; } = new SolidColorBrush(Colors.White);

        public string EditorDefaultFontFamily { get; set; } = "Arial";

        public double EditorDefaultFontSize { get; set; } = 24D;

        public FontStretch EditorDefaultFontStretch { get; set; } = FontStretches.Normal;

        public FontStyle EditorDefaultFontStyle { get; set; } = FontStyles.Normal;

        public FontWeight EditorDefaultFontWeight { get; set; } = FontWeights.Normal;

        public SolidColorBrush EditorDefaultForegroundColor { get; set; } = new SolidColorBrush(Colors.Black);

        public double EditorDefaultLineHeight { get; set; } = 32D;

        public TextAlignment EditorDefaultTextAlignment { get; set; } = TextAlignment.Left;

        public SolidColorBrush EditorHeading1BackgroundColor { get; set; } = new SolidColorBrush(Colors.White);

        public string EditorHeading1FontFamily { get; set; } = "Arial";

        public double EditorHeading1FontSize { get; set; } = 72D;

        public FontStretch EditorHeading1FontStretch { get; set; } = FontStretches.Normal;

        public FontStyle EditorHeading1FontStyle { get; set; } = FontStyles.Normal;

        public FontWeight EditorHeading1FontWeight { get; set; } = FontWeights.Bold;

        public SolidColorBrush EditorHeading1ForegroundColor { get; set; } = new SolidColorBrush(Colors.Black);

        public double EditorHeading1LineHeight { get; set; } = 96D;

        public TextAlignment EditorHeading1TextAlignment { get; set; } = TextAlignment.Left;

        public SolidColorBrush EditorHeading2BackgroundColor { get; set; } = new SolidColorBrush(Colors.White);

        public string EditorHeading2FontFamily { get; set; } = "Arial";

        public double EditorHeading2FontSize { get; set; } = 48D;

        public FontStretch EditorHeading2FontStretch { get; set; } = FontStretches.Normal;

        public FontStyle EditorHeading2FontStyle { get; set; } = FontStyles.Normal;

        public FontWeight EditorHeading2FontWeight { get; set; } = FontWeights.Bold;

        public SolidColorBrush EditorHeading2ForegroundColor { get; set; } = new SolidColorBrush(Colors.Black);

        public double EditorHeading2LineHeight { get; set; } = 64D;

        public TextAlignment EditorHeading2TextAlignment { get; set; } = TextAlignment.Left;

        public SolidColorBrush EditorHeading3BackgroundColor { get; set; } = new SolidColorBrush(Colors.White);

        public string EditorHeading3FontFamily { get; set; } = "Arial";

        public double EditorHeading3FontSize { get; set; } = 36D;

        public FontStretch EditorHeading3FontStretch { get; set; } = FontStretches.Normal;

        public FontStyle EditorHeading3FontStyle { get; set; } = FontStyles.Normal;

        public FontWeight EditorHeading3FontWeight { get; set; } = FontWeights.Bold;

        public SolidColorBrush EditorHeading3ForegroundColor { get; set; } = new SolidColorBrush(Colors.Black);

        public double EditorHeading3LineHeight { get; set; } = 48D;

        public TextAlignment EditorHeading3TextAlignment { get; set; } = TextAlignment.Left;

        public SolidColorBrush EditorHeading4BackgroundColor { get; set; } = new SolidColorBrush(Colors.White);

        public string EditorHeading4FontFamily { get; set; } = "Arial";

        public double EditorHeading4FontSize { get; set; } = 30D;

        public FontStretch EditorHeading4FontStretch { get; set; } = FontStretches.Normal;

        public FontStyle EditorHeading4FontStyle { get; set; } = FontStyles.Normal;

        public FontWeight EditorHeading4FontWeight { get; set; } = FontWeights.Bold;

        public SolidColorBrush EditorHeading4ForegroundColor { get; set; } = new SolidColorBrush(Colors.Black);

        public double EditorHeading4LineHeight { get; set; } = 40D;

        public TextAlignment EditorHeading4TextAlignment { get; set; } = TextAlignment.Left;

        private static Theme instance;

        public static Theme Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Theme();
                }

                return instance;
            }
        }

        public static void Reset()
        {
            instance = new Theme();
        }

        public static void Load(string filePath)
        {
            string json = File.ReadAllText(filePath);

            var dict = JsonSerializerUtility.DeserializeFromString<Dictionary<string, object>>(json);

            if (dict == null)
            {
                return;
            }

            instance = new Theme();

            foreach (var kvp in dict)
            {
                var property = typeof(Theme).GetProperty(kvp.Key);

                if (property != null && property.CanWrite)
                {
                    property.SetValue(instance, ConvertValue(kvp.Value, property.PropertyType));
                }
            }
        }

        private static object ConvertValue(object value, System.Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            var converter = TypeDescriptor.GetConverter(targetType);

            if (converter != null && converter.CanConvertFrom(value.GetType()))
            {
                return converter.ConvertFrom(value);
            }

            if (targetType == typeof(SolidColorBrush) && value is string s)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(s));
            }

            return value;
        }
    }
}