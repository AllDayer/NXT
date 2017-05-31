using System;

using Xamarin.Forms;

namespace NXT.Controls
{
    public class CardView : Frame
    {
        public CardView()
        {
            //if (Device.OS == TargetPlatform.iOS)
            {
                HasShadow = true;
                OutlineColor = Color.Transparent;
                BackgroundColor = Color.Transparent;
                CornerRadius = 5;

            }
        }
    }
}
