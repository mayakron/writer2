using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace Writer2
{
    public class Theme
    {
        public const string ThemesDirectoryPath = "Themes";

        public SolidColorBrush EditorCodeBackgroundColor = new SolidColorBrush(Colors.White);

        public string EditorCodeFontFamily = "Consolas";

        public double EditorCodeFontSize = 24D;

        public FontStretch EditorCodeFontStretch = FontStretches.Normal;

        public FontStyle EditorCodeFontStyle = FontStyles.Normal;

        public FontWeight EditorCodeFontWeight = FontWeights.Normal;

        public SolidColorBrush EditorCodeForegroundColor = new SolidColorBrush(Colors.Black);

        public double EditorCodeLineHeight = 32D;

        public TextAlignment EditorCodeTextAlignment = TextAlignment.Left;

        public SolidColorBrush EditorDefaultBackgroundColor = new SolidColorBrush(Colors.White);

        public string EditorDefaultFontFamily = "Arial";

        public double EditorDefaultFontSize = 24D;

        public FontStretch EditorDefaultFontStretch = FontStretches.Normal;

        public FontStyle EditorDefaultFontStyle = FontStyles.Normal;

        public FontWeight EditorDefaultFontWeight = FontWeights.Normal;

        public SolidColorBrush EditorDefaultForegroundColor = new SolidColorBrush(Colors.Black);

        public double EditorDefaultLineHeight = 32D;

        public TextAlignment EditorDefaultTextAlignment = TextAlignment.Left;

        public SolidColorBrush EditorHeading1BackgroundColor = new SolidColorBrush(Colors.White);

        public string EditorHeading1FontFamily = "Arial";

        public double EditorHeading1FontSize = 72D;

        public FontStretch EditorHeading1FontStretch = FontStretches.Normal;

        public FontStyle EditorHeading1FontStyle = FontStyles.Normal;

        public FontWeight EditorHeading1FontWeight = FontWeights.Bold;

        public SolidColorBrush EditorHeading1ForegroundColor = new SolidColorBrush(Colors.Black);

        public double EditorHeading1LineHeight = 96D;

        public TextAlignment EditorHeading1TextAlignment = TextAlignment.Left;

        public SolidColorBrush EditorHeading2BackgroundColor = new SolidColorBrush(Colors.White);

        public string EditorHeading2FontFamily = "Arial";

        public double EditorHeading2FontSize = 48D;

        public FontStretch EditorHeading2FontStretch = FontStretches.Normal;

        public FontStyle EditorHeading2FontStyle = FontStyles.Normal;

        public FontWeight EditorHeading2FontWeight = FontWeights.Bold;

        public SolidColorBrush EditorHeading2ForegroundColor = new SolidColorBrush(Colors.Black);

        public double EditorHeading2LineHeight = 64D;

        public TextAlignment EditorHeading2TextAlignment = TextAlignment.Left;

        public SolidColorBrush EditorHeading3BackgroundColor = new SolidColorBrush(Colors.White);

        public string EditorHeading3FontFamily = "Arial";

        public double EditorHeading3FontSize = 36D;

        public FontStretch EditorHeading3FontStretch = FontStretches.Normal;

        public FontStyle EditorHeading3FontStyle = FontStyles.Normal;

        public FontWeight EditorHeading3FontWeight = FontWeights.Bold;

        public SolidColorBrush EditorHeading3ForegroundColor = new SolidColorBrush(Colors.Black);

        public double EditorHeading3LineHeight = 48D;

        public TextAlignment EditorHeading3TextAlignment = TextAlignment.Left;

        public SolidColorBrush EditorHeading4BackgroundColor = new SolidColorBrush(Colors.White);

        public string EditorHeading4FontFamily = "Arial";

        public double EditorHeading4FontSize = 30D;

        public FontStretch EditorHeading4FontStretch = FontStretches.Normal;

        public FontStyle EditorHeading4FontStyle = FontStyles.Normal;

        public FontWeight EditorHeading4FontWeight = FontWeights.Bold;

        public SolidColorBrush EditorHeading4ForegroundColor = new SolidColorBrush(Colors.Black);

        public double EditorHeading4LineHeight = 40D;

        public TextAlignment EditorHeading4TextAlignment = TextAlignment.Left;

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
            instance = JsonSerializerUtility.DeserializeFromString<Theme>(File.ReadAllText(filePath));
        }
    }
}