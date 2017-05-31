using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NXT.Droid.Renderers;
using NXT.Controls;

[assembly: ExportRenderer(typeof(CustomImageButton), typeof(CustomImageButtonRenderer))]
namespace NXT.Droid.Renderers
{
    class CustomImageButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
        }
    }
}