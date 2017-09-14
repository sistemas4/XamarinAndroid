
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace RadiosFrater
{
	public class AboutWebVFragment : Android.Support.V4.App.Fragment
	{
		WebView aboutview2;
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
aboutview2.SaveState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			aboutview2.RestoreState(savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment 

			View view =inflater.Inflate(Resource.Layout.aboutWbLayout, container, false);
			aboutview2 = view.FindViewById<WebView>(Resource.Id.aboutView2);
			aboutview2.LoadUrl("http://radiosfrater.com/acerca-de-app/");
			return view;
		}
	}
}

