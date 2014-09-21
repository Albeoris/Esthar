using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Esthar.Core;
using Esthar.Data.Transform;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationControlTextTabEdit.xaml
    /// </summary>
    public partial class LocationControlTextTabEdit
    {
        private bool _windowChanging;

        public LocationTextControlContext Context { get; set; }

        public LocationControlTextTabEdit()
        {
            InitializeComponent();

            SetEditorSettings();
            TextEditorInstance.FontFamily = new FontFamily("Calibri");
            TextEditorInstance.FontSize = 24;

            Subscribe();
        }

        private void SetEditorSettings()
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Esthar.MainWindow.Locations.LocationControl.LocationControlTextTab.TextHighlighting.xshd"))
            using (XmlTextReader reader = new XmlTextReader(input))
                TextEditorInstance.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }

        private void Subscribe()
        {
            TextEditorInstance.Document.TextChanged += OnTextChanged;
            TextEditorInstance.TextArea.TextEntered += OnTextEntered;
            TextEditorInstance.TextArea.TextEntering += OnTextEntering;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (Context.Location == null)
                return;

            Context.UpdateTrigger.Set();
            if (!_windowChanging)
                Context.SaveRequest(LocationProperty.Monologues);
        }

        public void ChangeWindow(MessageWindow window)
        {
            _windowChanging = true;
            TextEditorInstance.Document.Text = window == null ? string.Empty : LocalizableString.ResolveNewLine(window.Message.Current);
            TextBoxInstance.Text = window == null ? string.Empty : LocalizableString.ResolveNewLine(window.Message.Original);
            TextEditorInstance.Document.UndoStack.ClearAll();
            _windowChanging = false;
        }

        #region Auto completion

        private static CompletionWindow _tagCompletionWindow;
        private static CompletionWindow _argCompletionWindow;

        private void OnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "{")
            {
                _tagCompletionWindow = new CompletionWindow(TextEditorInstance.TextArea);
                IList<ICompletionData> data = _tagCompletionWindow.CompletionList.CompletionData;
                foreach (TagCompletionData cd in TagCompletionData.Tags)
                    data.Add(cd);
                _tagCompletionWindow.Closed += (o, args) => _tagCompletionWindow = null;
                _tagCompletionWindow.Show();
            }
            else if (e.Text == "\"")
            {
                int position = TextEditorInstance.TextArea.Caret.Offset - 1;

                string finded;
                int pageBegin = StringHelper.IndexOfAny(TextEditorInstance.Text, position, int.MinValue, out finded, FF8TextTag.PageSeparator);
                if (pageBegin < 0) pageBegin = 0; else pageBegin += finded.Length + 1;

                int pageEnd = StringHelper.IndexOfAny(TextEditorInstance.Text, position, int.MaxValue, out finded, FF8TextTag.PageSeparator);
                if (pageEnd < 0) pageEnd = TextEditorInstance.Text.Length;                

                if (TextEditorInstance.Text.IndexOf('“', pageBegin, position - pageBegin) < 0)
                {
                    TextEditorInstance.Document.Replace(position, 1, "“");
                }
                else if (TextEditorInstance.Text.LastIndexOf('”', pageEnd - 1, pageEnd - pageBegin - 1) < 0)
                {
                    TextEditorInstance.Document.Replace(position, 1, "”");
                }
                else
                {
                    int lineBegin = TextEditorInstance.Text.LastIndexOf('\n', position, position - pageBegin);
                    if (lineBegin < 0) lineBegin = 0;

                    string repl;
                    int index = TextEditorInstance.Text.LastIndexOfAny(new[] {'«', '»'}, position, position - lineBegin);
                    if (index < 0)
                        repl = "«";
                    else
                        repl = TextEditorInstance.Text[index] == '«' ? "»" : "«";
                    
                    TextEditorInstance.Document.Replace(position, 1, repl);
                }
            }
        }

        private void OnTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length < 1)
                return;

            if (_tagCompletionWindow != null && !char.IsLetterOrDigit(e.Text[0]))
                _tagCompletionWindow.CompletionList.RequestInsertion(e);

            if (_argCompletionWindow != null && !char.IsLetterOrDigit(e.Text[0]))
                _argCompletionWindow.CompletionList.RequestInsertion(e);
        }

        private sealed class TagCompletionData : CompletionData
        {
            private static readonly Lazy<TagCompletionData[]> LazyTags = new Lazy<TagCompletionData[]>(CreateTags);

            public static TagCompletionData[] Tags
            {
                get { return LazyTags.Value; }
            }

            private static TagCompletionData[] CreateTags()
            {
                return new[]
                {
                    new TagCompletionData(FF8TextTagCode.Next),
                    new TagCompletionData(FF8TextTagCode.Speaker),
                    new TagCompletionData(FF8TextTagCode.Var),
                    new TagCompletionData(FF8TextTagCode.Pause),
                    new TagCompletionData(FF8TextTagCode.Char, EnumCache<FF8TextTagCharacter>.Values),
                    new TagCompletionData(FF8TextTagCode.Key, EnumCache<FF8TextTagKey>.Values),
                    new TagCompletionData(FF8TextTagCode.Color, EnumCache<FF8TextTagColor>.Values),
                    new TagCompletionData(FF8TextTagCode.Dialog, EnumCache<FF8TextTagDialog>.Values),
                    new TagCompletionData(FF8TextTagCode.Term, EnumCache<FF8TextTagTerm>.Values),
                };
            }

            private readonly List<CompletionData> _values = new List<CompletionData>();

            public TagCompletionData(FF8TextTagCode code, IEnumerable values = null)
                : base(code)
            {
                if (values == null)
                    return;

                foreach (Enum value in values)
                {
                    DisplayAttribute data = GetDisplayAttribute(value);
                    if (data != null)
                        _values.Add(new CompletionData(value));
                }
            }

            public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                if (_values.Count == 0)
                {
                    textArea.Document.Replace(completionSegment, Text + '}');
                    return;
                }

                textArea.Document.Replace(completionSegment, Text + ' ');

                _argCompletionWindow = new CompletionWindow(textArea);

                IList<ICompletionData> data = _argCompletionWindow.CompletionList.CompletionData;
                foreach (CompletionData value in _values)
                    data.Add(value);

                _argCompletionWindow.Closed += (sender, args) => _argCompletionWindow = null;
                _argCompletionWindow.Show();
            }
        }

        private class CompletionData : ICompletionData
        {
            private readonly Enum _value;

            public CompletionData(Enum value)
            {
                _value = value;

                DisplayAttribute data = GetDisplayAttribute(value);
                if (data == null) throw new ArgumentException("code");
                Text = data.Name;
                Description = data.Description;
                Priority = Convert.ToInt32(_value);
            }

            public ImageSource Image { get; private set; }
            public string Text { get; private set; }
            public object Description { get; private set; }
            public double Priority { get; private set; }

            public object Content
            {
                get { return Text; }
            }

            public virtual void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, Text + '}');
            }

            protected DisplayAttribute GetDisplayAttribute(Enum value)
            {
                return (DisplayAttribute)value
                    .GetType()
                    .GetMember(value.ToString())[0]
                    .GetCustomAttribute(TypeCache<DisplayAttribute>.Type, false);
            }
        }

        #endregion

        public void InsertCharater(FF8TextTagCharacter tag)
        {
            TextEditorInstance.TextArea.Selection.ReplaceSelectionWithText(new FF8TextTag(FF8TextTagCode.Char, tag).ToString());
        }
    }
}