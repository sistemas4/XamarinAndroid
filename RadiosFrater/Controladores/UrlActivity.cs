
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace RadiosFrater
{
    [Activity(Label = "urlactivity")]
    public class UrlActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));

            // Create your application here
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }

        protected override void OnStop()
        {
            base.OnStop();
            Finish();
        }
    }
}
