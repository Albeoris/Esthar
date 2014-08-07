using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Esthar.Core;
using Esthar.Data.Transform;
using Esthar.OpenGL;

namespace Esthar
{
    public sealed class LocationTextControlContext
    {
        public readonly LocationControlTextTabList List;
        public readonly LocationControlTextTabEdit Edit;
        public readonly LocationControlTextTabPreview Preview;
        public readonly AutoResetEvent UpdateTrigger;
        public Location Location;
        public MessageWindow Window;

        public LocationTextControlContext(LocationControlTextTabList list, LocationControlTextTabEdit edit, LocationControlTextTabPreview preview)
        {
            List = Exceptions.CheckArgumentNull(list, "list");
            Edit = Exceptions.CheckArgumentNull(edit, "edit");
            Preview = Exceptions.CheckArgumentNull(preview, "preview");
            UpdateTrigger = new AutoResetEvent(false);

            SetContext();
        }

        private void SetContext()
        {
            List.Context = this;
            Edit.Context = this;
            Preview.Context = this;
        }

        public string GetCurrentText()
        {
            return Edit.Dispatcher.Invoke(() => Edit.TextEditorInstance.Document.Text);
        }

        public void ChangeLocation(Location location)
        {
            if (Location != null)
            {
                Location.UnloadText();
                Location.SaveRequested -= OnLocationSaveRequested;
            }

            Location = location;
            if (Location != null)
            {
                location.LoadText();
                Location.SaveRequested += OnLocationSaveRequested;
            }

            UpdateList();
            UpdatePrviewBackground();
        }

        private void OnLocationSaveRequested(LocationProperty saveRequest)
        {
            if ((saveRequest & LocationProperty.Monologues) == LocationProperty.Monologues && Window != null)
            {
                string text = GetCurrentText();
                Window.Message.Current = LocalizableString.ReturnNewLine(text);
            }
        }

        public void ChangeWindow(MessageWindow window)
        {
            if (Window != null)
            {
                string text = GetCurrentText();
                Window.Message.Current = LocalizableString.ReturnNewLine(text);
            }

            Window = window;
            Edit.Dispatcher.Invoke(() => Edit.ChangeWindow(window));
            Preview.Dispatcher.Invoke(() => Preview.ChangeWindow(window));
        }

        private void UpdateList()
        {
            List<MessageWindow> windows = null;
            if (Location != null)
                windows = Location.MessageWindows.DistinctBy(wnd=>wnd.MessageId).ToList();

            List.Dispatcher.Invoke(() => List.SetMessageWindows(windows));
        }

        private void UpdatePrviewBackground()
        {
            GLLocationRenderer.Initialize(GameFont.HiResFont.GetRenderer(), ResourcesAccessor.CursorImage, ResourcesAccessor.DisabledCursorImage);
            GLLocationRenderer.SetBackground(Location.GetBackgroundReader());
            UpdatePreviewText(null);
        }

        public void UpdatePreviewText(string[] pages)
        {
            GLLocationRenderer.SetText(pages);
        }

        public void SaveRequest(LocationProperty property)
        {
            Location.SaveRequest |= property;
        }
    }
}