
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
    [Activity(Label = "InstructionActivity")]
    public class InstructionActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.InstruccionesLayout);
            ImageView nxt = FindViewById<ImageView>(Resource.Id.nxtbtn);
            nxt.Click += (sender, e) => {
                Intent conf = new Intent(this.BaseContext, typeof(NotificationConfigureActivity));
                conf.PutExtra("id", "1");
                Finish();
                StartActivity(conf);
            };

        }
    }
}
