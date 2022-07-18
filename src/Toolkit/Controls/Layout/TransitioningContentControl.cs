using System;
using System.Linq;
using Aldwych.AvaloniaToolkit.ViewModels;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Styling;


namespace Aldwych.AvaloniaToolkit.Controls
{
    ///// <summary>
    ///// A ContentControl that animates the transition when its content is changed.
    ///// </summary>
    //public class TransitioningContentControl : ContentControl, IStyleable
    //{

    //    public static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
    //        AvaloniaProperty.Register<TransitioningContentControl, IPageTransition?>(nameof(PageTransition));


    //    public static readonly StyledProperty<object?> DefaultContentProperty =
    //        AvaloniaProperty.Register<TransitioningContentControl, object?>(nameof(DefaultContent));


    //    public static readonly StyledProperty<TimeSpan> DurationProperty =
    //        AvaloniaProperty.Register<TransitioningContentControl, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(0.3));

    //    public static readonly StyledProperty<TransitionType> TransitionTypeProperty =
    //       AvaloniaProperty.Register<TransitioningContentControl, TransitionType>(nameof(Transition), TransitionType.None);


    //    public TransitionType Transition
    //    {
    //        get => GetValue(TransitionTypeProperty);
    //        set => SetValue(TransitionTypeProperty, value);
    //    }


    //    public TimeSpan Duration
    //    {
    //        get => GetValue(DurationProperty);
    //        set => SetValue(DurationProperty, value);
    //    }

    //    /// <summary>
    //    /// Gets or sets the animation played when content appears and disappears.
    //    /// </summary>
    //    public IPageTransition? PageTransition
    //    {
    //        get => GetValue(PageTransitionProperty);
    //        set => SetValue(PageTransitionProperty, value);
    //    }

    //    /// <summary>
    //    /// Gets or sets the content displayed whenever there is no page currently routed.
    //    /// </summary>
    //    public object? DefaultContent
    //    {
    //        get => GetValue(DefaultContentProperty);
    //        set => SetValue(DefaultContentProperty, value);
    //    }

    //    /// <summary>
    //    /// Gets or sets the content with animation.
    //    /// </summary>
    //    public new object? Content
    //    {
    //        get => base.Content;
    //        set => UpdateContentWithTransition(value);
    //    }

    //    /// <summary>
    //    /// TransitioningContentControl uses the default ContentControl 
    //    /// template from Avalonia default theme.
    //    /// </summary>
    //    Type IStyleable.StyleKey => typeof(ContentControl);


    //    /// <summary>
    //    /// Updates the content with transitions.
    //    /// </summary>
    //    /// <param name="content">New content to set.</param>
    //    private async void UpdateContentWithTransition(object? content)
    //    {           
    //        if (PageTransition != null)
    //        {
    //            if (content is Visual vContent)
    //            {
    //                await PageTransition.Start(this, vContent, true);
    //                base.Content = content;
    //            }
    //            else
    //            {                  
    //                base.Content = content;                    
    //                await PageTransition.Start(null, this, true);                    
    //            }
    //        }
    //        else 
    //             base.Content = content;

    //    }


    //    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    //    {
    //        base.OnPropertyChanged(change);
              

    //        if (change.Property == TransitionTypeProperty)
    //        {
    //            var nv = change.NewValue.Value.ToString();
    //            switch (Enum.Parse<TransitionType>(nv))
    //            {
    //                case TransitionType.Crossfade:
    //                    PageTransition = new CrossFade(Duration);
    //                    break;
    //                case TransitionType.Slide:
    //                    PageTransition = new PageSlide(Duration);
    //                    break;
    //                default:
    //                    PageTransition = null;
    //                    break;
    //            }
    //        }
    //    }
    //}
}
