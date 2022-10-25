using NLog.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace CSCE3513_LaserTag_Project.Utils
{
    [Target("flowDocument")]
    public sealed class FlowDocumentTarget : TargetWithLayout
    {
        private FlowDocument _document = new FlowDocument { Background = new SolidColorBrush(Colors.Black) };
        private readonly Paragraph _paragraph = new Paragraph();
        private readonly int _maxLines = 500;

        public FlowDocument Document => _document;

        public FlowDocumentTarget()
        {
            _document.Blocks.Add(_paragraph);
        }

        /// <inheritdoc />
        protected override void Write(LogEventInfo logEvent)
        {
            _document.Dispatcher.BeginInvoke((Action)(() =>
            {
                var message = $"{Layout.Render(logEvent)}\n";
                _paragraph.Inlines.Add(new Run(message) { Foreground = LogLevelColors[logEvent.Level] });

                // A massive paragraph slows the UI down
                if (_paragraph.Inlines.Count > _maxLines)
                    _paragraph.Inlines.Remove(_paragraph.Inlines.FirstInline);
            }));
        }

        private static readonly Dictionary<LogLevel, SolidColorBrush> LogLevelColors = new Dictionary<LogLevel, SolidColorBrush>
        {
            [LogLevel.Trace] = new SolidColorBrush(Colors.DimGray),
            [LogLevel.Debug] = new SolidColorBrush(Colors.DarkGray),
            [LogLevel.Info] = new SolidColorBrush(Colors.White),
            [LogLevel.Warn] = new SolidColorBrush(Colors.Magenta),
            [LogLevel.Error] = new SolidColorBrush(Colors.Yellow),
            [LogLevel.Fatal] = new SolidColorBrush(Colors.Red),
        };
    }
}
