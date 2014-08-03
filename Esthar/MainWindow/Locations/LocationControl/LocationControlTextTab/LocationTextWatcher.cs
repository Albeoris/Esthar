using System;
using System.Linq;
using System.Threading;
using Esthar.Core;

namespace Esthar
{
    internal sealed class LocationTextWatcher : IDisposable
    {
        private readonly LocationTextControlContext _context;

        private volatile bool _watching = true;

        public LocationTextWatcher(LocationTextControlContext context)
        {
            _context = Exceptions.CheckArgumentNull(context, "context");
            BeginWatching();
        }

        public void Dispose()
        {
            _watching = false;
        }

        private void BeginWatching()
        {
            Thread watchingThread = new Thread(WatchThreadProc);
            watchingThread.Start();
        }

        private void WatchThreadProc()
        {
            while (_watching)
            {
                if (!_context.UpdateTrigger.WaitOne(500))
                    continue;

                DateTime begin = DateTime.Now;

                string text = _context.GetCurrentText();
                string[] pages = PrepareTexts(text);
                _context.UpdatePreviewText(pages);

                int span = 500 - (int)(DateTime.Now - begin).TotalMilliseconds;
                if (span > 0)
                    Thread.Sleep(span);
            }
        }

        private string[] PrepareTexts(string text)
        {
            return text
                .Split(FF8TextTag.PageSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0).ToArray();
        }
    }
}