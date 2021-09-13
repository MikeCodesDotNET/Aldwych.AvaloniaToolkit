using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;

namespace Aldwych.AvaloniaToolkit.Controls
{
    public class DigitalClock : TemplatedControl
    {
        public static readonly StyledProperty<string> CurrentTimeProperty = AvaloniaProperty.Register <DigitalClock, string>(nameof(CurrentTime));
         
    
        private System.Timers.Timer _timer;

        public string CurrentTime
        {
            get { return GetValue(CurrentTimeProperty); }
            private set { SetValue(CurrentTimeProperty, value); }
        }


        public DigitalClock()
        {
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += _timer_Elapsed;
            CurrentTime = DateTime.Now.ToString("hh:mm:ss");
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            _timer.Start();
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            _timer.Stop();
        }

        protected override Size MeasureCore(Size availableSize)
        {
            FontSize = (int)(availableSize.Width / 5);
            return base.MeasureCore(availableSize);
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentTime = DateTime.Now.ToString("hh:mm:ss");
            });
        }
    }
}
