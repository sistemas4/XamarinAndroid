using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using Org.Apache.Http.Impl.Client;
using RadiosFrater.Services;

namespace RadiosFrater
{
	[Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
	public class PlayerActivity : AppCompatActivity
	{
		//Kids Adoración
		//Español Inglés
		//Clásica Instrumental
		string[] radioName = {
			"Kids", "Adoración",
			"Español", "Inglés",
			"Clásica", "Instrumental",
			"Prédicas", "FráterTV" };
		int[] radioImage = {
			Resource.Drawable.KIds_2, Resource.Drawable.Adoracion_2,
			Resource.Drawable.Espanol_2, Resource.Drawable.Ingles_2,
			Resource.Drawable.Clasica_2, Resource.Drawable.Instrumental_2,
			Resource.Drawable.Predicas};
		public readonly string[] Mp31 = { 
			//Kids												//Adoracion
			@"http://174.142.111.104:9302/stats?sid=1&json=1", @"http://174.142.111.104:9984/stats?sid=1&json=1", 
			//Español											//Ingles
			@"http://174.142.111.104:9998/stats?sid=1&json=1", @"http://174.142.111.104:9992/stats?sid=1&json=1", 
			//Clasica											//Instrumental
			@"http://174.142.111.104:9982/stats?sid=1&json=1", @"http://174.142.111.104:9980/stats?sid=1&json=1", 
			//Clasica											
			"@http://174.142.111.104:9986/stats?sid=1&json=1" };

		bool isBound = false;
		private MediaPlayerServiceBinder binder;
		MediaPlayerServiceConnection mediaPlayerServiceConnection;
		private Intent mediaPlayerServiceIntent;

		public event StatusChangedEventHandler StatusChanged;

		public event CoverReloadedEventHandler CoverReloaded;

		public event PlayingEventHandler Playing;

		public event BufferingEventHandler Buffering;

		public Android.Support.V7.Widget.Toolbar toolbar { get; set; }

		ImageButton btnPS;
		ImageView rImage;
		TextView songr;
		JsonValue json;
		Handler hand = new Handler();
		int id;
		System.Timers.Timer timer;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Create your application here
			SetContentView(Resource.Layout.PlayerLayout);
			toolbar = FindViewById< Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
			//var title = toolbar.FindViewById<TextView>(Resource.Id.toolbar_title);
			//Title = radioName[id];
			SetSupportActionBar(toolbar);
			id = Intent.GetIntExtra("id",8);
			SupportActionBar.Title = radioName[id];
			//title.Text = radioName[id];


			rImage = FindViewById<ImageView>(Resource.Id.rdioimg);
			rImage.SetImageResource(radioImage[id]);

			btnPS = FindViewById<ImageButton>(Resource.Id.btnPS);
			songr = FindViewById<TextView>(Resource.Id.songText);

			btnPS.Click += async (sender, e) =>
			{
				if (binder.GetMediaPlayerService().mediaPlayer != null && binder.GetMediaPlayerService().MediaPlayerState == PlaybackStateCompat.StatePlaying)
				{
					await binder.GetMediaPlayerService().Stop();
					btnPS.SetImageResource(Resource.Drawable.playRF);
				}
				else
				{
					await binder.GetMediaPlayerService().Play(id);
					btnPS.SetImageResource(Resource.Drawable.pausaRF);
				}

			};
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			if(mediaPlayerServiceConnection == null)
                InitilizeMedia();
			
			//if (!isPlay)
			//{
			//	binder.GetMediaPlayerService().Play(id).ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
			//	btnPS.SetImageResource(Resource.Drawable.pausaRF);
			//}
			//else
			//{
			//	if (idSave != id)
			//	{
			//		binder.GetMediaPlayerService().Stop().ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
			//		binder.GetMediaPlayerService().Play(id).ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
			//	}
			//	btnPS.SetImageResource(Resource.Drawable.pausaRF);
			//}
		}

		//protected override void OnStart()
		//{
		//	base.OnStart();
		//}

		public void initPlay()
		{
			if (binder.GetMediaPlayerService().mediaPlayer != null && binder.GetMediaPlayerService().MediaPlayerState == PlaybackStateCompat.StatePlaying)
			{
				binder.GetMediaPlayerService().Stop().ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
				btnPS.SetImageResource(Resource.Drawable.playRF);
			}
			else
			{
				binder.GetMediaPlayerService().Play(id).ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
				btnPS.SetImageResource(Resource.Drawable.pausaRF);
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			timer = new System.Timers.Timer();
			timer.Interval = 5000;
			timer.Elapsed += Timer_Elapsed;
			timer.Start();

		}

		void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				RunOnUiThread(() =>
				{
					go().ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
				});
			}
			catch (Exception z)
			{
				var d = z;
			}
		}

		private async Task<JsonValue> FetchWeatherAsync(string url)
		{
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";

			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync())
			{
				// Get a stream representation of the HTTP web response:
				using (var stream = response.GetResponseStream())
				{
					// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

					// Return the JSON document:
					return jsonDoc;
				}
			}
		}

		async Task go()
		{
			json = await FetchWeatherAsync(Mp31[id]);
			ParseAndDisplay();
		}

		void ParseAndDisplay()
		{
			JsonValue songTitle = json["songtitle"];
			if (songTitle != string.Empty)
				songr.Text = songTitle;

		}

		protected override void OnStop()
		{
			base.OnStop();
			if (timer != null)
			{
				timer.Elapsed -= Timer_Elapsed;
				timer.Stop();
			}
			timer = null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (timer != null)
			{
				timer.Elapsed -= Timer_Elapsed;
				timer.Stop();
			}
			timer = null;
			Finish();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					Finish();
					return true;

				default:
					return base.OnOptionsItemSelected(item);

			}
		}

		//public override void OnBackPressed()
		//{
		//	Finish();
		//	Intent intent = new Intent(this, typeof(MainActivity));
		//	intent.PutExtra("isP", isPlay);
		//	StartActivity(intent);
		//	base.OnBackPressed();
		//}

		private void InitilizeMedia()
		{
			mediaPlayerServiceIntent = new Intent(ApplicationContext, typeof(MediaPlayerService));
			mediaPlayerServiceConnection = new MediaPlayerServiceConnection(this);
			BindService(mediaPlayerServiceIntent, mediaPlayerServiceConnection, Bind.AutoCreate);   
		}

		class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
		{
			PlayerActivity instance;

			public MediaPlayerServiceConnection(PlayerActivity mediaPlayer)
			{
				this.instance = mediaPlayer;
			}

			public void OnServiceConnected(ComponentName name, IBinder service)
			{
				var mediaPlayerServiceBinder = service as MediaPlayerServiceBinder;
				if (mediaPlayerServiceBinder != null)
				{
					var binder = (MediaPlayerServiceBinder)service;
					instance.binder = binder;
					instance.isBound = true;

					binder.GetMediaPlayerService().CoverReloaded += (object sender, EventArgs e) => { if (instance.CoverReloaded != null) instance.CoverReloaded(sender, e); };
					binder.GetMediaPlayerService().StatusChanged += (object sender, EventArgs e) => { if (instance.StatusChanged != null) instance.StatusChanged(sender, e); };
					binder.GetMediaPlayerService().Playing += (object sender, EventArgs e) => { if (instance.Playing != null) instance.Playing(sender, e); };
					binder.GetMediaPlayerService().Buffering += (object sender, EventArgs e) => { if (instance.Buffering != null) instance.Buffering(sender, e); };
					instance.initPlay();
				}
			}

			public void OnServiceDisconnected(ComponentName name)
			{
				instance.isBound = false;
			}
		}
	}
}