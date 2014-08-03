using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Esthar.Core;
using Esthar.Data;
using Esthar.Data.Transform;
using Esthar.OpenGL;
using Esthar.UI;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Xceed.Wpf.Toolkit;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace Esthar.Font
{
    /// <summary>
    /// Логика взаимодействия для FontConfigurationWindow.xaml
    /// </summary>
    public partial class FontConfigurationWindow
    {
        private static readonly Color SelectedColor = Color.FromArgb(80, 255, 90, 90);
        private static readonly Color GridColor = Color.FromArgb(127, 127, 127, 127);

        private readonly GameFont _font;
        private readonly GLTexture _palette;
        private readonly byte[] _charactersWidths;
        private readonly char[] _indicesToChars;
        private readonly HashSet<char>[] _charsToIndices;

        private TextBlock[] _characterNameControls1;
        private TextBlock[] _characterNameControls2;

        private GLTexture _texture;
        private bool _ownTexture;

        private AutoResetEvent _drawEvent;
        private int _currentIndex;
        private int _currentPalette;

        public FontConfigurationWindow(GameFont font)
        {
            _font = font;
            _texture = font.CharactersImage.Layer;
            _palette = font.CharactersImage.Palettes;

            _charactersWidths = (byte[])font.CharactersWidths.Clone();
            Options.Codepage.GetParameters(out _indicesToChars, out _charsToIndices);

            InitializeComponent();
            InitilaizeFields();
        }

        private void InitilaizeFields()
        {
            SetPalettes();

            InitializeCharacterWidths();
            InitializeCharsToIndices();
            InitializeIndicesToChars();

            GridRow1.MaxHeight = CharacterWidthsCanvas.Height + CharactersToIndexCanvas.Height + IndicesToCharacterCanvas.Height + 24 * 3;

            BindScrollViewers();
            Subscribe();
        }

        private void SetPalettes()
        {
            CurrentPaletteBox.ItemsSource = EnumCache<FF8TextTagColor>.Values.Take(FF8TextTagColor.White - FF8TextTagColor.Disabled + 1);

            CurrentPaletteBox.SelectedIndex = FF8TextTagColor.White - FF8TextTagColor.Disabled;
            _currentPalette = CurrentPaletteBox.SelectedIndex;

            CurrentPaletteBox.SelectionChanged += OnCurrentPaletteBoxSelectionChanged;
        }

        private void OnCurrentPaletteBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPalette = ((ComboBox)sender).SelectedIndex;
            _drawEvent.NullSafeSet();
        }

        #region Scrolls

        private void BindScrollViewers()
        {
            CharacterWidthsCanvasScroll.ScrollChanged += OnScrollChanged;
            IndicesToCharacterCanvasScroll.ScrollChanged += OnScrollChanged;
            CharactersToIndexCanvasScroll.ScrollChanged += OnScrollChanged;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CharacterWidthsCanvasScroll.ScrollChanged -= OnScrollChanged;
            IndicesToCharacterCanvasScroll.ScrollChanged -= OnScrollChanged;
            CharactersToIndexCanvasScroll.ScrollChanged -= OnScrollChanged;

            if (!CharacterWidthsCanvasScroll.Equals(sender))
            {
                CharacterWidthsCanvasScroll.ScrollToVerticalOffset(e.VerticalOffset);
                CharacterWidthsCanvasScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
            }

            if (!IndicesToCharacterCanvasScroll.Equals(sender))
            {
                IndicesToCharacterCanvasScroll.ScrollToVerticalOffset(e.VerticalOffset);
                IndicesToCharacterCanvasScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
            }

            if (!CharactersToIndexCanvasScroll.Equals(sender))
            {
                CharactersToIndexCanvasScroll.ScrollToVerticalOffset(e.VerticalOffset);
                CharactersToIndexCanvasScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
            }

            CharacterWidthsCanvasScroll.ScrollChanged += OnScrollChanged;
            IndicesToCharacterCanvasScroll.ScrollChanged += OnScrollChanged;
            CharactersToIndexCanvasScroll.ScrollChanged += OnScrollChanged;
        }

        #endregion

        #region Widths

        private void InitializeCharacterWidths()
        {
            _characterNameControls1 = new TextBlock[_charactersWidths.Length];

            int maxX = 0, maxY = 0;
            for (int i = 0; i < _charactersWidths.Length; i++)
            {
                int col = i % 21;
                int row = i / 21;
                int x = 3 + col * 62;
                int y = 3 + row * 28;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;

                TextBlock tb = _characterNameControls1[i] = new TextBlock { Text = _indicesToChars[i + 32].ToString(CultureInfo.InvariantCulture) };
                Canvas.SetLeft(tb, x);
                Canvas.SetTop(tb, y + 2);

                IntegerUpDown ud = new IntegerUpDown { Width = 40, Minimum = 0, Maximum = 30, Value = _charactersWidths[i], Tag = i };
                Canvas.SetLeft(ud, x + 14);
                Canvas.SetTop(ud, y);
                ud.Focusable = true;
                ud.GotFocus += OnInputableControlGotFocus;
                ud.ValueChanged += OnWidthChanged;

                CharacterWidthsCanvas.Children.Add(tb);
                CharacterWidthsCanvas.Children.Add(ud);
            }

            CharacterWidthsCanvas.Width = maxX + 62;
            CharacterWidthsCanvas.Height = maxY + 28;
        }

        private void OnWidthChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int index = (int)((IntegerUpDown)sender).Tag;
            byte value = (byte)(int)e.NewValue;
            _charactersWidths[index] = value;
            _drawEvent.NullSafeSet();
        }

        #endregion

        #region Chars to indices

        private void InitializeCharsToIndices()
        {
            _characterNameControls2 = new TextBlock[_charactersWidths.Length];

            int maxX = 0, maxY = 0;
            for (int i = 0; i < _charactersWidths.Length; i++)
            {
                int col = i % 21;
                int row = i / 21;
                int x = 3 + col * 62;
                int y = 3 + row * 28;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;

                TextBlock tb = _characterNameControls2[i] = new TextBlock { Text = _indicesToChars[i + 32].ToString(CultureInfo.InvariantCulture) };
                Canvas.SetLeft(tb, x);
                Canvas.SetTop(tb, y + 2);

                TextBox box;
                lock (_charsToIndices)
                    box = new TextBox { Tag = i, Width = 40, Text = new String(_charsToIndices[i + 32].ToArray()) };
                Canvas.SetLeft(box, x + 14);
                Canvas.SetTop(box, y);
                box.Focusable = true;
                box.GotFocus += OnInputableControlGotFocus;
                box.TextChanged += OnIndexChanged;

                CharactersToIndexCanvas.Children.Add(tb);
                CharactersToIndexCanvas.Children.Add(box);
            }

            CharactersToIndexCanvas.Width = maxX + 62;
            CharactersToIndexCanvas.Height = maxY + 28;
        }

        private void OnIndexChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            int index = (int)box.Tag;

            string text = box.Text;
            if (!string.IsNullOrEmpty(text) && text.Length > 4)
                text = text.Remove(0, text.Length - 4);

            lock (box)
            {
                box.TextChanged -= OnIndexChanged;
                box.Text = text;
                box.CaretIndex = text.Length;
                box.TextChanged += OnIndexChanged;

                lock (_charsToIndices)
                {
                    _charsToIndices[index + 32].Clear();
                    foreach (char ch in text)
                        _charsToIndices[index + 32].Add(ch);
                }
            }
        }

        #endregion

        #region Indices to chars

        private void InitializeIndicesToChars()
        {
            int maxX = 0, maxY = 0;
            for (int i = 0; i < _charactersWidths.Length; i++)
            {
                int col = i % 21;
                int row = i / 21;
                int x = 3 + col * 62;
                int y = 3 + row * 28;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;

                TextBlock tb = new TextBlock { Text = i.ToString("D3") };
                Canvas.SetLeft(tb, x);
                Canvas.SetTop(tb, y + 2);

                TextBox box = new TextBox { Tag = i, Width = 30, Text = _indicesToChars[i + 32].ToString(CultureInfo.InvariantCulture) };
                Canvas.SetLeft(box, x + 24);
                Canvas.SetTop(box, y);
                box.Focusable = true;
                box.GotFocus += OnInputableControlGotFocus;
                box.TextChanged += OnCharaterChanged;

                IndicesToCharacterCanvas.Children.Add(tb);
                IndicesToCharacterCanvas.Children.Add(box);
            }

            IndicesToCharacterCanvas.Width = maxX + 62;
            IndicesToCharacterCanvas.Height = maxY + 28;
        }

        private void OnCharaterChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            int index = (int)box.Tag;

            string text = box.Text;
            if (string.IsNullOrEmpty(text))
                text = "☻";
            else if (text.Length > 1)
                text = text[text.Length - 1].ToString(CultureInfo.InvariantCulture);

            lock (box)
            {
                box.TextChanged -= OnCharaterChanged;
                box.Text = text;
                box.CaretIndex = text.Length;
                box.TextChanged += OnCharaterChanged;

                _indicesToChars[index + 32] = text[0];
                _characterNameControls1[index].Text = text;
                _characterNameControls2[index].Text = text;
            }

            _drawEvent.NullSafeSet();
        }

        #endregion

        private void OnInputableControlGotFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement control = (FrameworkElement)sender;
            Interlocked.Exchange(ref _currentIndex, (int)control.Tag);

            _drawEvent.NullSafeSet();
        }

        private void Subscribe()
        {
            Closing += OnClosing;
            StateChanged += OnStateChanged;
            ContentRendered += OnWindowContentRendered;
            GLControlElement.Load += OnGLControlElementLoaded;
            GLControlElement.Resize += OnGLControlElementResize;
            GLControlElement.MouseClick += OnGLControlElementMouseClick;

            GLControlElement.AllowDrop = true;
            GLControlElement.DragEnter += OnGLControlElementDragEnter;
            GLControlElement.DragDrop += OnGLControlElementDragDrop;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState != System.Windows.WindowState.Minimized)
                _drawEvent.NullSafeSet();
        }

        private void OnWindowContentRendered(object sender, EventArgs e)
        {
            _drawEvent.NullSafeSet();
        }

        private void OnGLControlElementDragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = System.Windows.Forms.DragDropEffects.Copy;
        }

        private void OnGLControlElementDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 2 && files.All(f => PathComparer.Instance.Value.Equals(Path.GetExtension(f), ".tex")))
                    ReadTextureFromTexFiles(files);
                else if (files.Length == 1 && PathComparer.Instance.Value.Equals(Path.GetExtension(files[0]), ".gif"))
                    ReadTextureFromGifFile(files[0]);
                else
                    return;

                _drawEvent.NullSafeSet();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void ReadTextureFromTexFiles(string[] files)
        {
            GLTexture[] textures = new GLTexture[2];
            using (DisposableStack disposables = new DisposableStack(2))
            {
                for (int i = 0; i < 2; i++)
                {
                    using (FileStream input = File.OpenRead(files[i]))
                    using (TexFileReader texReader = new TexFileReader(input))
                        textures[i] = disposables.Add(texReader.ReadImage());
                }

                _texture = GLTextureFactory.HorizontalJoin(textures[0], textures[1]);
                _ownTexture = true;
            }
        }

        private void ReadTextureFromGifFile(string file)
        {
            _texture = GLTextureFactory.FromImageFile(file);
            _ownTexture = true;
        }

        private void OnGLControlElementMouseClick(object sender, MouseEventArgs e)
        {
            int index = ((e.Y / 24) * 21 + (e.X / 24));
            if (index < _charactersWidths.Length)
            {
                UIElement control = CharacterWidthsCanvas.Children[index * 2 + 1];
                double kx = Canvas.GetLeft(control) / CharacterWidthsCanvas.Width;
                double ky = Canvas.GetTop(control) / CharacterWidthsCanvas.Height;
                double x = ((CharacterWidthsCanvasScroll.ScrollableWidth + CharacterWidthsCanvasScroll.ViewportWidth) * kx) - CharacterWidthsCanvasScroll.ViewportWidth / 2;
                double y = ((CharacterWidthsCanvasScroll.ScrollableHeight + CharacterWidthsCanvasScroll.ViewportHeight) * ky) - CharacterWidthsCanvasScroll.ViewportHeight / 2;

                CharacterWidthsCanvasScroll.ScrollToHorizontalOffset(x);
                CharacterWidthsCanvasScroll.ScrollToVerticalOffset(y);

                CharacterWidthsCanvas.Children[index * 2 + 1].Focus();
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            //GLService.UnregisterDrawMethod(DrawPreview);

            if (DialogResult != true)
            {
                if (_ownTexture)
                    _texture.SafeDispose();
            }
        }

        private void OnGLControlElementLoaded(object sender, EventArgs e)
        {
            //GLService.SubscribeControl(GLControlHost);
            //_drawEvent = GLService.RegisterDrawMethod(DrawPreview);

            Window window = (Window)this.GetRootElement();
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
            //window.LostFocus += (s, a) => GLService.UnsubscribeControl(GLControlHost);
            //window.GotFocus += (s, a) => GLService.SubscribeControl(GLControlHost);

            //_drawEvent.Set();
        }

        private void OnWindowActivated(object sender, EventArgs e)
        {
            GLService.SubscribeControl(GLControlHost);
            _drawEvent = GLService.RegisterDrawMethod(DrawPreview);
            ConfigPreview();
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            GLService.UnregisterDrawMethod(DrawPreview);
            GLService.UnsubscribeControl(GLControlHost);
        }

        private void OnGLControlElementResize(object sender, EventArgs e)
        {
            ConfigPreview();
        }

        private void ConfigPreview()
        {
            using (GLService.AcquireContext())
            {
                GL.ClearColor(Color4.DimGray);

                int w = GLControlElement.Width;
                int h = GLControlElement.Height;
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, w, h, 0, -1, 1);
                GL.Viewport(0, 0, w, h);

                _drawEvent.NullSafeSet();
            }
        }

        private void DrawPreview()
        {
            Dictionary<char, int> charsToBytes;
            lock (_charsToIndices) charsToBytes = ExpandCollectionArray(_charsToIndices);
            string text = Dispatcher.Invoke(() => PreviewTextBox.Text);

            GLRectangle rectangle = new GLRectangle { Height = 23 };

            using (GLService.AcquireContext(GLControlElement.WindowInfo))
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                using (GLShaders.PalettedTexture.Use(_texture, _palette, _currentPalette))
                    _texture.Draw(0, 0, 0);

                for (int i = 0; i < _charactersWidths.Length; i++)
                {
                    rectangle.X = (i % 21) * 24;
                    rectangle.Y = (i / 21) * 24;
                    rectangle.Width = _charactersWidths[i] * 2;

                    if (i == _currentIndex)
                        rectangle.DrawSolid(SelectedColor);
                    else
                        rectangle.DrawBorder(GridColor);
                }

                float x = 0;
                float y = _texture.Height + 24;

                using (GLShaders.PalettedTexture.Use(_texture, _palette, FF8TextTagColor.White - FF8TextTagColor.Disabled))
                {
                    foreach (char ch in text)
                    {
                        if (ch == '\r')
                            continue;

                        if (ch == '\n')
                        {
                            x = 0;
                            y += 24;
                            continue;
                        }

                        int index;
                        if (charsToBytes.TryGetValue(ch, out index))
                        {
                            index -= 32;
                            const float h = 23;
                            float w = _charactersWidths[index] * 2;

                            _texture.Draw(x, y, 0, (index % 21) * 24f, (index / 21) * 24, w, h);
                            x += w;
                        }
                    }
                }

                GL.Disable(EnableCap.Blend);
            }
        }

        private Dictionary<T, int> ExpandCollectionArray<T>(IEnumerable<T>[] array)
        {
            Dictionary<T, int> dic = new Dictionary<T, int>();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (T key in array[i])
                    dic[key] = i;
            }
            return dic;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ownTexture)
                    _font.UpdateTexture(_texture);
                _font.UpdateWidths(_charactersWidths);
                _font.SaveHiRes();
                _font.ImportHiRes();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}