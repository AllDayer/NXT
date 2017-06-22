using NXT.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;

[assembly:
    ExportRenderer
        (
            typeof(NXT.Views.GoogleAuthPage),
            typeof(NXT.Droid.Renderers.GoogleAuthPageRenderer)
        )
]
namespace NXT.Droid.Renderers
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class GoogleAuthPageRenderer : Xamarin.Forms.Platform.Android.PageRenderer
    {
        protected Xamarin.Auth.Authenticator Authenticator = null;
        protected NXT.Views.GoogleAuthPage authenticator_page = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            // this is a ViewGroup - so should be able to load an AXML file and FindView<>
            global::Android.App.Activity activity = this.Context as global::Android.App.Activity;


            authenticator_page = (NXT.Views.GoogleAuthPage)base.Element;

            Authenticator = authenticator_page.Authenticator;
            Authenticator.Completed += Authentication_Completed;
            Authenticator.Error += Authentication_Error;

            global::Android.Content.Intent ui_object = Authenticator.GetUI(activity);

            activity.StartActivity(ui_object);

            return;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }


        protected void Authentication_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            authenticator_page.Authentication_Completed(sender, e);

            return;
        }

        protected void Authentication_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            authenticator_page.Authentication_Error(sender, e);

            return;
        }

        protected void Authentication_BrowsingCompleted(object sender, EventArgs e)
        {
            authenticator_page.Authentication_BrowsingCompleted(sender, e);

            return;
        }
    }
}