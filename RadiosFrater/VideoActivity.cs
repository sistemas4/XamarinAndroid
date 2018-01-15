
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Media;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace RadiosFrater
{
	[Activity(Label = "Fráter TV", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,  Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
	public class VideoActivity : Activity
	{

		WebView count;
		VideoView vidView;
		const string vidUrl = "http://64.15.158.43:1935/tvfrater/tvfrater/playlist.m3u8";
		//const string vidUrl = "https://fratertv.s3.amazonaws.com/El%20Nacimiento%20de%20Juan%20El%20Bautista.mp4";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			// Create your application here
			SetContentView(Resource.Layout.VideoStream);

			//ActionBar.SetHomeButtonEnabled(true);
			//ActionBar.SetDisplayHomeAsUpEnabled(true);


			count = FindViewById<WebView>(Resource.Id.countview);
			count.Settings.JavaScriptEnabled = true;
			count.SetWebViewClient(new WebViewClient());
			count.LoadUrl("http://frater.tv/countdown");


			vidView = FindViewById<VideoView>(Resource.Id.videoStream);
			Android.Net.Uri vidUri = Android.Net.Uri.Parse(vidUrl);
			vidView.SetVideoURI(vidUri);
			var s = vidUri.EncodedQuery;

			vidView.Error += VidView_Error;
			MediaController vidControl = new MediaController(this);
			vidControl.SetAnchorView(vidView);
			vidView.SetMediaController(vidControl);
			vidView.Start();

		}

		void VidView_Error(object sender, MediaPlayer.ErrorEventArgs e)
		{
			//AlertDialog.Builder alert = new AlertDialog.Builder(this);
			//LayoutInflater mInflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
			//ViewGroup viewgr = (ViewGroup)mInflater.Inflate(Resource.Layout.CountLayout, null);
			////count.RemoveView(vidView);
			//viewgr.AddView(count);
			//alert.SetTitle("Próxima Transmisión");
			//alert.SetView(viewgr);
			//alert.SetNegativeButton("Ok", (senderAlert, args) =>
			//{
			//	Toast.MakeText(this, "ok", ToastLength.Short).Show();
			//});

			//Dialog dialog = alert.Create();
			//dialog.Show();
			vidView.Visibility = ViewStates.Invisible;
			count.Visibility = ViewStates.Visible;

		}

		public override void OnBackPressed()
		{
			FinishAffinity();
			Intent intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
			base.OnBackPressed();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}

		protected override void OnRestoreInstanceState(Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState(savedInstanceState);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					FinishAffinity();
					Intent intent = new Intent(this, typeof(MainActivity));
					StartActivity(intent);
					return true;

				default:
					return base.OnOptionsItemSelected(item);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();

		}
	}
}
