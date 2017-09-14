
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
	public class ContactWbFragment : Android.Support.V4.App.Fragment
	{
		WebView contactview;
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState); 
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment 
			View view =inflater.Inflate(Resource.Layout.ContactWbLayout, container, false);
			contactview = view.FindViewById<WebView>(Resource.Id.contactView);
			contactview.LoadUrl("http://radiosfrater.com/contacto-app/");
			return view;
		}
	}
}

