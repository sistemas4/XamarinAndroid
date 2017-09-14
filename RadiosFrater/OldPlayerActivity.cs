//using System;
//using System.Json;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Android.App;
//using Android.Content;
//using Android.Content.PM;
//using Android.Media;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Java.Util;
//using Org.Apache.Http.Impl.Client;
//using RadiosFrater.Services;

//namespace RadiosFrater
//{
//	[Activity(Label = "PlayerActivity", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
//	public class OldPlayerActivity : Activity
//	{
//		//Kids Adoración
//		//Español Inglés
//		//Clásica Instrumental
//		string[] radioName = {
//			"Kids", "Adoración",
//			"Español", "Inglés",
//			"Clásica", "Instrumental",
//			"FráterTV" };
//		int[] radioImage = {
//			Resource.Drawable.KIds_2, Resource.Drawable.Adoracion_2,
//			Resource.Drawable.Espanol_2, Resource.Drawable.Ingles_2,
//			Resource.Drawable.Clasica_2, Resource.Drawable.Instrumental_2 };
//		public readonly string[] Mp31 = { 
//			//Kids												//Adoracion
//			@"http://174.142.111.104:9302/stats?sid=1&json=1", @"http://174.142.111.104:9984/stats?sid=1&json=1", 
//			//Español											//Ingles
//			@"http://174.142.111.104:9998/stats?sid=1&json=1", @"http://174.142.111.104:9992/stats?sid=1&json=1", 
//			//Clasica											//Instrumental
//			@"http://174.142.111.104:9982/stats?sid=1&json=1", @"http://174.142.111.104:9980/stats?sid=1&json=1" };
//		ImageButton btnPS;
//		ImageView rImage;
//		TextView songr;
//		JsonValue json;
//		Handler hand = new Handler();
//		bool isPlay;
//		int id, idSave;
//		System.Timers.Timer timer;

//		protected override void OnCreate(Bundle savedInstanceState)
//		{
//			base.OnCreate(savedInstanceState);

//			ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
//			var idN = sharedpref.GetInt("idNot", 8);
//			//id = Intent.GetIntExtra("id", 8);
//			id = idN;
//			isPlay = sharedpref.GetBoolean("isPlay", false);
//			idSave = sharedpref.GetInt("rPlay", 8);
//			if (id == 6)
//			{
//				if (isPlay)
//					SendAudioCommand(StreamingBackgroundService.ActionStop, id);
//				Intent player = new Intent(this, typeof(VideoActivity));
//				player.PutExtra("id", id);
//				StartActivity(player);
//			}
//			else
//			{
//				// Create your application here
//				SetContentView(Resource.Layout.PlayerLayout);
//				//Title = radioName[id];
//				this.ActionBar.Title = radioName[id];

//				rImage = FindViewById<ImageView>(Resource.Id.rdioimg);
//				if (id != 6)
//					rImage.SetImageResource(radioImage[id]);

//				btnPS = FindViewById<ImageButton>(Resource.Id.btnPS);
//				songr = FindViewById<TextView>(Resource.Id.songText);

//				btnPS.Click += delegate
//				{
//					PlayStop();

//				};

//				ActionBar.SetHomeButtonEnabled(true);
//				ActionBar.SetDisplayHomeAsUpEnabled(true);
//				if (!isPlay)
//				{
//					SendAudioCommand(StreamingBackgroundService.ActionPlay, id);
//					btnPS.SetImageResource(Resource.Drawable.pausaRF);
//				}
//				else
//				{
//					if (idSave != id)
//					{
//						SendAudioCommand(StreamingBackgroundService.ActionStop, id);
//						SendAudioCommand(StreamingBackgroundService.ActionPlay, id);
//					}
//					btnPS.SetImageResource(Resource.Drawable.pausaRF);
//				}
//			}
//		}

//		protected override void OnResume()
//		{
//			base.OnResume();
//			timer = new System.Timers.Timer();
//			timer.Interval = 5000;
//			timer.Elapsed += Timer_Elapsed;
//			timer.Start();

//		}

//		void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
//		{
//			try
//			{
//				RunOnUiThread(() =>
//				{
//					go().ContinueWith(t => Console.Write(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
//				});
//			}
//			catch (Exception z)
//			{
//				var d = z;
//			}
//		}

//		private async Task<JsonValue> FetchWeatherAsync(string url)
//		{
//			// Create an HTTP web request using the URL:
//			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
//			request.ContentType = "application/json";
//			request.Method = "GET";

//			// Send the request to the server and wait for the response:
//			using (WebResponse response = await request.GetResponseAsync())
//			{
//				// Get a stream representation of the HTTP web response:
//				using (var stream = response.GetResponseStream())
//				{
//					// Use this stream to build a JSON document object:
//					JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
//					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

//					// Return the JSON document:
//					return jsonDoc;
//				}
//			}
//		}

//		async Task go()
//		{
//			json = await FetchWeatherAsync(Mp31[id]);
//			ParseAndDisplay();
//		}

//		void ParseAndDisplay()
//		{
//			JsonValue songTitle = json["songtitle"];
//			if (songTitle != string.Empty)
//				songr.Text = songTitle;

//		}

//		protected override void OnStop()
//		{
//			base.OnStop();
//			if (timer != null)
//			{
//				timer.Elapsed -= Timer_Elapsed;
//				timer.Stop();
//			}
//			timer = null;
//			Finish();
//		}

//		protected override void OnDestroy()
//		{
//			base.OnDestroy();
//			if (timer != null)
//			{
//				timer.Elapsed -= Timer_Elapsed;
//				timer.Stop();
//			}
//			timer = null;
//			Finish();

//		}




//		private void SendAudioCommand(string action, int Id)
//		{
//			if (id != 6)
//				btnPS.Enabled = false;
//			Intent intent = new Intent(action);
//			intent.PutExtra("id", Id);
//			StartService(intent);
//			if (action.Contains("PLAY"))
//			{
//				isPlay = true;
//				Android.Net.Uri uri = Android.Net.Uri.Parse(Mp31[id]);
//				var scheme = uri.Scheme;
//				ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
//				ISharedPreferencesEditor editor = sharedpref.Edit();
//				editor.PutBoolean("isPlay", isPlay);
//				editor.PutInt("rPlay", id);
//				editor.Commit();


//				//var r = new MediaMetadataRetriever();
//				//var s = new Dictionary<string, string>();
//				//r.SetDataSource(Mp31[id], s);
//				//songr.Text = r.ExtractMetadata(20);
//				//var u = r.ExtractMetadata(MetadataKey.HasAudio);

//			}
//			else
//			{
//				if (action.Contains("STOP"))
//				{
//					isPlay = false;
//					ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
//					ISharedPreferencesEditor editor = sharedpref.Edit();
//					editor.PutBoolean("isPlay", isPlay);
//					editor.Commit();
//				}
//			}
//			if (btnPS != null && id != 6)
//				btnPS.Enabled = true;
//		}

//		void PlayStop()
//		{
//			if (!isPlay)
//			{
//				SendAudioCommand(StreamingBackgroundService.ActionPlay, id);
//				btnPS.SetImageResource(Resource.Drawable.pausaRF);
//				isPlay = true;
//				ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
//				ISharedPreferencesEditor editor = sharedpref.Edit();
//				editor.PutBoolean("isPlay", isPlay);
//				editor.Commit();
//			}
//			else
//			{
//				SendAudioCommand(StreamingBackgroundService.ActionStop, id);
//				btnPS.SetImageResource(Resource.Drawable.playRF);
//				isPlay = false;
//				ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
//				ISharedPreferencesEditor editor = sharedpref.Edit();
//				editor.PutBoolean("isPlay", isPlay);
//				editor.Commit();
//			}
//		}

//		public override bool OnOptionsItemSelected(IMenuItem item)
//		{
//			switch (item.ItemId)
//			{
//				case Android.Resource.Id.Home:
//					Finish();
//					Intent intent = new Intent(this, typeof(MainActivity));
//					intent.PutExtra("isP", isPlay);
//					StartActivity(intent);
//					return true;

//				default:
//					return base.OnOptionsItemSelected(item);
//			}
//		}

//		public override void OnBackPressed()
//		{
//			Finish();
//			Intent intent = new Intent(this, typeof(MainActivity));
//			intent.PutExtra("isP", isPlay);
//			StartActivity(intent);
//			base.OnBackPressed();
//		}

//	}
//}