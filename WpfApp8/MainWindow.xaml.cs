using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace WpfApp8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private OpenFileDialog _openFileDialog;
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_openFileDialog == null)
            {
                _openFileDialog = new OpenFileDialog
                {
                    Filter = "TXT files (*.txt) |*.txt"
                };
            }
            if (_openFileDialog.ShowDialog() == true)
            {
                textBox.Text = File.ReadAllText(_openFileDialog.FileName, Encoding.UTF8);
                textBox.Focus();
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Open.Execute(null, this);
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new MainWindow().ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.ShowDialog();
            //new SaveFileDialog().ShowDialog();
            new Microsoft.Win32.SaveFileDialog { Filter = "TXT files (*.txt) |*.txt" }.ShowDialog();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isDirty;
        }

        private bool _isDirty;

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _isDirty = !String.IsNullOrEmpty(textBox.Text);
        }

        private void QueryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("BestCommand");
        }

        private FontDialog fontDialog1;
        private void Click_Format(object sender, RoutedEventArgs e)
        {
            fontDialog1 = new System.Windows.Forms.FontDialog();
            fontDialog1.ShowDialog();

            Font font = fontDialog1.Font;
            rich.FontFamily = new System.Windows.Media.FontFamily(font.Name);
            rich.FontSize = font.Size;
            rich.FontWeight = font.Bold ? FontWeights.Bold : FontWeights.Regular;
            rich.FontStyle = font.Italic ? FontStyles.Italic : FontStyles.Normal;
        }

        public TextRange FindTextInRange(TextRange searchRange, string searchText)
        {
            int offset = searchRange.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (offset < 0)
                return null;  // Not found

            var start = GetTextPositionAtOffset(searchRange.Start, offset);
            TextRange result = new TextRange(start, GetTextPositionAtOffset(start, searchText.Length));

            return result;
        }

        TextPointer GetTextPositionAtOffset(TextPointer position, int characterCount)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    int count = position.GetTextRunLength(LogicalDirection.Forward);
                    if (characterCount <= count)
                    {
                        return position.GetPositionAtOffset(characterCount);
                    }

                    characterCount -= count;
                }

                TextPointer nextContextPosition = position.GetNextContextPosition(LogicalDirection.Forward);
                if (nextContextPosition == null)
                    return position;

                position = nextContextPosition;
            }

            return position;
        }

        private void Undo_ExeCuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Copy_ExeCuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Redo_ExeCuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Cut_ExeCuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Delete_ExeCuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Find_ExeCuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Click_Find(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(rich.Document.ContentStart, rich.Document.ContentEnd);

            //clear up highlighted text before starting a new search
            textRange.ClearAllProperties();
            lable_message.Content = "";

            //get the richtextbox text
            string textBoxText = textRange.Text;

            //get search text
            string searchText = find_text.Text;

            if (string.IsNullOrWhiteSpace(textBoxText) || string.IsNullOrWhiteSpace(searchText))
            {
                lable_message.Content = "Please provide search text or source text to search from";
            }

            else
            {
                //using regex to get the search count
                //this will include search word even it is part of another word
                //say we are searching "hi" in "hi, how are you Mahi?" --> match count will be 2 (hi in 'Mahi' also)

                Regex regex = new Regex(searchText);
                int count_MatchFound = Regex.Matches(textBoxText, regex.ToString()).Count;

                for (TextPointer startPointer = rich.Document.ContentStart;
                            startPointer.CompareTo(rich.Document.ContentEnd) <= 0;
                                startPointer = startPointer.GetNextContextPosition(LogicalDirection.Forward))
                {
                    //check if end of text
                    if (startPointer.CompareTo(rich.Document.ContentEnd) == 0)
                    {
                        break;
                    }

                    //get the adjacent string
                    string parsedString = startPointer.GetTextInRun(LogicalDirection.Forward);

                    //check if the search string present here
                    int indexOfParseString = parsedString.IndexOf(searchText);

                    if (indexOfParseString >= 0) //present
                    {
                        //setting up the pointer here at this matched index
                        startPointer = startPointer.GetPositionAtOffset(indexOfParseString);

                        if (startPointer != null)
                        {
                            //next pointer will be the length of the search string
                            TextPointer nextPointer = startPointer.GetPositionAtOffset(searchText.Length);

                            //create the text range
                            TextRange searchedTextRange = new TextRange(startPointer, nextPointer);

                            //color up 
                            searchedTextRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));

                            //add other setting property

                        }
                    }
                }

                //update the label text with count
                if (count_MatchFound > 0)
                {
                    lable_message.Content = "Total Match Found : " + count_MatchFound;
                }
                else
                {
                    lable_message.Content = "No Match Found !";
                }
            }
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new SaveFileDialog { Filter = "TXT files(*.txt)|*.txt" }.ShowDialog();
        }
    }
}
