using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Writer2
{
    public partial class MainWindow : Window
    {
        private string EditorFilePath;

        private int EditorMaxLineWidth = 0;

        public MainWindow()
        {
            InitializeComponent();

            if (Directory.Exists(Theme.ThemesDirectoryPath))
            {
                this.ThemesMenuItem.Items.Add(MenuItemUtility.CreateMenuItem("Default", ThemesThemeMenuItem_Click, null, null));

                foreach (var filePath in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Theme.ThemesDirectoryPath), "*.json", SearchOption.TopDirectoryOnly).OrderBy(x => x))
                {
                    this.ThemesMenuItem.Items.Add(MenuItemUtility.CreateMenuItem(Path.GetFileNameWithoutExtension(filePath), ThemesThemeMenuItem_Click, null, filePath));
                }
            }

            this.EditorFilePath = Path.Combine(App.StartupDirectoryPath, $"{Guid.NewGuid():N}.rtf");

            this.Window_SetTitle(null);

            var commandFileNew = new RoutedCommand(); commandFileNew.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandFileNew, this.FileNewMenuItem_Click));
            var commandFileOpen = new RoutedCommand(); commandFileOpen.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandFileOpen, this.FileOpenMenuItem_Click));
            var commandFileSave = new RoutedCommand(); commandFileSave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandFileSave, this.FileSaveMenuItem_Click));
            var commandFileSaveAs = new RoutedCommand(); commandFileSaveAs.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)); CommandBindings.Add(new CommandBinding(commandFileSaveAs, this.FileSaveAsMenuItem_Click));

            var commandEditPasteAsText = new RoutedCommand(); commandEditPasteAsText.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandEditPasteAsText, this.EditPasteAsTextMenuItem_Click));

            var commandEditSetFontStyleStrikethrough = new RoutedCommand(); commandEditSetFontStyleStrikethrough.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleStrikethrough, this.EditSetFontStyleStrikethroughMenuItem_Click));

            var commandEditSetFontStyleDefault = new RoutedCommand(); commandEditSetFontStyleDefault.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleDefault, this.EditSetFontStyleDefaultMenuItem_Click));
            var commandEditSetFontStyleHeading1 = new RoutedCommand(); commandEditSetFontStyleHeading1.InputGestures.Add(new KeyGesture(Key.D1, ModifierKeys.Control | ModifierKeys.Shift)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleHeading1, this.EditSetFontStyleHeading1MenuItem_Click));
            var commandEditSetFontStyleHeading2 = new RoutedCommand(); commandEditSetFontStyleHeading2.InputGestures.Add(new KeyGesture(Key.D2, ModifierKeys.Control | ModifierKeys.Shift)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleHeading2, this.EditSetFontStyleHeading2MenuItem_Click));
            var commandEditSetFontStyleHeading3 = new RoutedCommand(); commandEditSetFontStyleHeading3.InputGestures.Add(new KeyGesture(Key.D3, ModifierKeys.Control | ModifierKeys.Shift)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleHeading3, this.EditSetFontStyleHeading3MenuItem_Click));
            var commandEditSetFontStyleHeading4 = new RoutedCommand(); commandEditSetFontStyleHeading4.InputGestures.Add(new KeyGesture(Key.D4, ModifierKeys.Control | ModifierKeys.Shift)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleHeading4, this.EditSetFontStyleHeading4MenuItem_Click));
            var commandEditSetFontStyleCode = new RoutedCommand(); commandEditSetFontStyleCode.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control)); CommandBindings.Add(new CommandBinding(commandEditSetFontStyleCode, this.EditSetFontStyleCodeMenuItem_Click));

            var commandViewToggleFullScreenMode = new RoutedCommand(); commandViewToggleFullScreenMode.InputGestures.Add(new KeyGesture(Key.F11, ModifierKeys.None)); CommandBindings.Add(new CommandBinding(commandViewToggleFullScreenMode, this.ViewToggleFullScreenModeMenuItem_Click));
        }

        private void CustomEditingCommandsToogleTextDecoration(TextDecorationCollection textDecorationCollection, TextDecorationLocation textDecorationLocation)
        {
            TextRange editorSelection = this.Editor.Selection;

            object textDecorationPropertyValue = editorSelection.GetPropertyValue(Inline.TextDecorationsProperty);

            TextDecorationCollection textDecorations = new TextDecorationCollection();

            if ((textDecorationPropertyValue != DependencyProperty.UnsetValue) && (textDecorationPropertyValue is TextDecorationCollection existingTextDecorationPropertyValue))
            {
                textDecorations.Add(existingTextDecorationPropertyValue);
            }

            bool hasTextDecoration = false;

            foreach (var textDecoration in textDecorations)
            {
                if (textDecoration.Location == textDecorationLocation)
                {
                    hasTextDecoration = true;

                    break;
                }
            }

            if (hasTextDecoration)
            {
                TextDecorationCollection filteredTextDecorations = new TextDecorationCollection();

                foreach (var textDecoration in textDecorations)
                {
                    if (textDecoration.Location != textDecorationLocation)
                    {
                        filteredTextDecorations.Add(textDecoration);
                    }
                }

                editorSelection.ApplyPropertyValue(Inline.TextDecorationsProperty, filteredTextDecorations);
            }
            else
            {
                textDecorations.Add(textDecorationCollection);

                editorSelection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void EditCopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Copy();
        }

        private void EditCutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Cut();
        }

        private void EditDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.Delete.Execute(null, this.Editor);
        }

        private void EditPasteAsTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                this.Editor.BeginChange();

                try
                {
                    this.Editor.Selection.Text = string.Empty;

                    this.Editor.CaretPosition.InsertTextInRun(Clipboard.GetText(TextDataFormat.Text));
                }
                finally
                {
                    this.Editor.EndChange();
                }
            }
        }

        private void EditPasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Paste();
        }

        private void EditRedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Redo();
        }

        private void EditSelectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.SelectAll();
        }

        private void EditSetFontColorBackgroundBlackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Black);
        }

        private void EditSetFontColorBackgroundBlueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Blue);
        }

        private void EditSetFontColorBackgroundDarkBlueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkBlue);
        }

        private void EditSetFontColorBackgroundDarkGrayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkGray);
        }

        private void EditSetFontColorBackgroundDarkGreenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkGreen);
        }

        private void EditSetFontColorBackgroundDarkRedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkRed);
        }

        private void EditSetFontColorBackgroundDarkVioletMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkViolet);
        }

        private void EditSetFontColorBackgroundGrayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Gray);
        }

        private void EditSetFontColorBackgroundGreenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Green);
        }

        private void EditSetFontColorBackgroundGreenYellowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.GreenYellow);
        }

        private void EditSetFontColorBackgroundRedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Red);
        }

        private void EditSetFontColorBackgroundVioletMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Violet);
        }

        private void EditSetFontColorBackgroundWhiteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
        }

        private void EditSetFontColorBackgroundYellowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
        }

        private void EditSetFontColorForegroundBlackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
        }

        private void EditSetFontColorForegroundBlueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
        }

        private void EditSetFontColorForegroundDarkBlueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkBlue);
        }

        private void EditSetFontColorForegroundDarkGrayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkGray);
        }

        private void EditSetFontColorForegroundDarkGreenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkGreen);
        }

        private void EditSetFontColorForegroundDarkRedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkRed);
        }

        private void EditSetFontColorForegroundDarkVioletMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.DarkViolet);
        }

        private void EditSetFontColorForegroundGrayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Gray);
        }

        private void EditSetFontColorForegroundGreenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Green);
        }

        private void EditSetFontColorForegroundGreenYellowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.GreenYellow);
        }

        private void EditSetFontColorForegroundRedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
        }

        private void EditSetFontColorForegroundVioletMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Violet);
        }

        private void EditSetFontColorForegroundWhiteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
        }

        private void EditSetFontColorForegroundYellowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
        }

        private void EditSetFontStyleBoldMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, this.Editor);
        }

        private void EditSetFontStyleCodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.BeginChange();

            try
            {
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, Theme.Instance.EditorCodeFontFamily);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, Theme.Instance.EditorCodeFontSize);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, Theme.Instance.EditorCodeFontWeight);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, Theme.Instance.EditorCodeFontStyle);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStretchProperty, Theme.Instance.EditorCodeFontStretch);
                this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Theme.Instance.EditorCodeForegroundColor);
                this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Theme.Instance.EditorCodeBackgroundColor);
                this.Editor.Selection.ApplyPropertyValue(Block.LineHeightProperty, Theme.Instance.EditorCodeLineHeight);
                this.Editor.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, Theme.Instance.EditorCodeTextAlignment);
            }
            finally
            {
                this.Editor.EndChange();
            }
        }

        private void EditSetFontStyleDefaultMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.BeginChange();

            try
            {
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, Theme.Instance.EditorDefaultFontFamily);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, Theme.Instance.EditorDefaultFontSize);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, Theme.Instance.EditorDefaultFontWeight);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, Theme.Instance.EditorDefaultFontStyle);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStretchProperty, Theme.Instance.EditorDefaultFontStretch);
                this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Theme.Instance.EditorDefaultForegroundColor);
                this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Theme.Instance.EditorDefaultBackgroundColor);
                this.Editor.Selection.ApplyPropertyValue(Block.LineHeightProperty, Theme.Instance.EditorDefaultLineHeight);
                this.Editor.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, Theme.Instance.EditorDefaultTextAlignment);
            }
            finally
            {
                this.Editor.EndChange();
            }
        }

        private void EditSetFontStyleHeading1MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.BeginChange();

            try
            {
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, Theme.Instance.EditorHeading1FontFamily);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, Theme.Instance.EditorHeading1FontSize);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, Theme.Instance.EditorHeading1FontWeight);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, Theme.Instance.EditorHeading1FontStyle);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStretchProperty, Theme.Instance.EditorHeading1FontStretch);
                this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Theme.Instance.EditorHeading1ForegroundColor);
                this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Theme.Instance.EditorHeading1BackgroundColor);
                this.Editor.Selection.ApplyPropertyValue(Block.LineHeightProperty, Theme.Instance.EditorHeading1LineHeight);
                this.Editor.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, Theme.Instance.EditorHeading1TextAlignment);
            }
            finally
            {
                this.Editor.EndChange();
            }
        }

        private void EditSetFontStyleHeading2MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.BeginChange();

            try
            {
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, Theme.Instance.EditorHeading2FontFamily);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, Theme.Instance.EditorHeading2FontSize);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, Theme.Instance.EditorHeading2FontWeight);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, Theme.Instance.EditorHeading2FontStyle);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStretchProperty, Theme.Instance.EditorHeading2FontStretch);
                this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Theme.Instance.EditorHeading2ForegroundColor);
                this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Theme.Instance.EditorHeading2BackgroundColor);
                this.Editor.Selection.ApplyPropertyValue(Block.LineHeightProperty, Theme.Instance.EditorHeading2LineHeight);
                this.Editor.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, Theme.Instance.EditorHeading2TextAlignment);
            }
            finally
            {
                this.Editor.EndChange();
            }
        }

        private void EditSetFontStyleHeading3MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.BeginChange();

            try
            {
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, Theme.Instance.EditorHeading3FontFamily);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, Theme.Instance.EditorHeading3FontSize);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, Theme.Instance.EditorHeading3FontWeight);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, Theme.Instance.EditorHeading3FontStyle);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStretchProperty, Theme.Instance.EditorHeading3FontStretch);
                this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Theme.Instance.EditorHeading3ForegroundColor);
                this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Theme.Instance.EditorHeading3BackgroundColor);
                this.Editor.Selection.ApplyPropertyValue(Block.LineHeightProperty, Theme.Instance.EditorHeading3LineHeight);
                this.Editor.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, Theme.Instance.EditorHeading3TextAlignment);
            }
            finally
            {
                this.Editor.EndChange();
            }
        }

        private void EditSetFontStyleHeading4MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.BeginChange();

            try
            {
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, Theme.Instance.EditorHeading4FontFamily);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontSizeProperty, Theme.Instance.EditorHeading4FontSize);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontWeightProperty, Theme.Instance.EditorHeading4FontWeight);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStyleProperty, Theme.Instance.EditorHeading4FontStyle);
                this.Editor.Selection.ApplyPropertyValue(RichTextBox.FontStretchProperty, Theme.Instance.EditorHeading4FontStretch);
                this.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Theme.Instance.EditorHeading4ForegroundColor);
                this.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Theme.Instance.EditorHeading4BackgroundColor);
                this.Editor.Selection.ApplyPropertyValue(Block.LineHeightProperty, Theme.Instance.EditorHeading4LineHeight);
                this.Editor.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, Theme.Instance.EditorHeading4TextAlignment);
            }
            finally
            {
                this.Editor.EndChange();
            }
        }

        private void EditSetFontStyleItalicMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, this.Editor);
        }

        private void EditSetFontStyleStrikethroughMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CustomEditingCommandsToogleTextDecoration(TextDecorations.Strikethrough, TextDecorationLocation.Strikethrough);
        }

        private void EditSetFontStyleUnderlineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleUnderline.Execute(null, this.Editor);
        }

        private void EditSetListStyleBulletsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleBullets.Execute(null, this.Editor);
        }

        private void EditSetListStyleNumbersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.ToggleNumbering.Execute(null, this.Editor);
        }

        private void EditSetParagraphAlignmentCenterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignCenter.Execute(null, this.Editor);
        }

        private void EditSetParagraphAlignmentJustifiedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignJustify.Execute(null, this.Editor);
        }

        private void EditSetParagraphAlignmentLeftMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignLeft.Execute(null, this.Editor);
        }

        private void EditSetParagraphAlignmentRightMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.AlignRight.Execute(null, this.Editor);
        }

        private void EditSetParagraphIndentationDecreaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.DecreaseIndentation.Execute(null, this.Editor);
        }

        private void EditSetParagraphIndentationIncreaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditingCommands.IncreaseIndentation.Execute(null, this.Editor);
        }

        private void EditUndoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Editor.Undo();
        }

        private void FileExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FileNewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.EditorFilePath = Path.Combine(App.StartupDirectoryPath, $"{Guid.NewGuid():N}.rtf");

            this.Editor.Document.Blocks.Clear();

            this.Window_SetTitle(null);
        }

        private void FileOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "Rich Text Format files (*.rtf)|*.rtf|All files (*.*)|*.*";
            dialog.Title = "Open File...";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var range = new TextRange(this.Editor.Document.ContentStart, this.Editor.Document.ContentEnd);

                    using (var fileStream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        range.Load(fileStream, DataFormats.Rtf);
                    }

                    this.EditorFilePath = dialog.FileName;

                    this.Window_SetTitle(null);
                }
                catch (Exception ex)
                {
                    this.Window_HandleError(ex);
                }
            }
        }

        private void FileSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            dialog.Filter = "Rich Text Format files (*.rtf)|*.rtf|All files (*.*)|*.*";
            dialog.Title = "Save File As...";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var range = new TextRange(this.Editor.Document.ContentStart, this.Editor.Document.ContentEnd);

                    using (var fileStream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        range.Save(fileStream, DataFormats.Rtf);
                    }

                    this.EditorFilePath = dialog.FileName;

                    this.Window_SetTitle($"Last saved at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }
                catch (Exception ex)
                {
                    this.Window_HandleError(ex);
                }
            }
        }

        private void FileSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var range = new TextRange(this.Editor.Document.ContentStart, this.Editor.Document.ContentEnd);

                using (var fileStream = new FileStream(this.EditorFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    range.Save(fileStream, DataFormats.Rtf);
                }

                this.Window_SetTitle($"Last saved at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                this.Window_HandleError(ex);
            }
        }

        private void HelpAboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Writer2 is a distraction free writing application.", $"Writer2 (v. {App.Version})", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HelpProjectPageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/mayakron/writer2");
        }

        private void ThemesThemeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem == null)
            {
                return;
            }

            if (menuItem.Tag == null)
            {
                Theme.Reset();

                this.Window_SetTheme();

                return;
            }

            var filePath = menuItem.Tag as string;

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                return;
            }

            Theme.Load(filePath);

            this.Window_SetTheme();
        }

        private void ViewSetLineMaxWidthTo1000pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 1000;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo1250pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 1250;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo1500pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 1500;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo1750pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 1750;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo2000pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 2000;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo2250pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 2250;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo2500pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 2500;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo2750pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 2750;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthTo3000pxMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 3000;

            this.Window_ResetEditorPadding();
        }

        private void ViewSetLineMaxWidthToFullWidthMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditorMaxLineWidth = 0;

            this.Editor.Padding = new Thickness(40);
        }

        private void ViewToggleFullScreenModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowStyle != WindowStyle.None)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void Window_HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Editor.Focus();
        }

        private void Window_ResetEditorPadding()
        {
            var horizontalPadding = Math.Max(40, Math.Round(0.5D * (this.ActualWidth - (double)EditorMaxLineWidth)));

            this.Editor.Padding = new Thickness(horizontalPadding, this.Editor.Padding.Top, horizontalPadding, this.Editor.Padding.Bottom);
        }

        private void Window_SetTheme()
        {
            this.Editor.FontFamily = new FontFamily(Theme.Instance.EditorDefaultFontFamily);
            this.Editor.FontSize = Theme.Instance.EditorDefaultFontSize;
            this.Editor.FontWeight = Theme.Instance.EditorDefaultFontWeight;
            this.Editor.FontStyle = Theme.Instance.EditorDefaultFontStyle;
            this.Editor.FontStretch = Theme.Instance.EditorDefaultFontStretch;
            this.Editor.Background = Theme.Instance.EditorDefaultBackgroundColor;
            this.Editor.Foreground = Theme.Instance.EditorDefaultForegroundColor;
            this.Editor.SetValue(Block.LineHeightProperty, Theme.Instance.EditorDefaultLineHeight);
            this.Editor.SetValue(Block.TextAlignmentProperty, Theme.Instance.EditorDefaultTextAlignment);
        }

        private void Window_SetTitle(string message)
        {
            this.Title = (message != null) ? $"Writer2 (v. {App.Version}) - {Path.GetFileName(this.EditorFilePath)} - {message}" : $"Writer2 (v. {App.Version}) - {Path.GetFileName(this.EditorFilePath)}";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (EditorMaxLineWidth > 0)
            {
                this.Window_ResetEditorPadding();
            }
        }
    }
}