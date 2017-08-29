﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Media;
using Android.Net.Wifi;

namespace RadiosFrater.Services
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop })]
    public class StreamingBackgroundService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        //Actions
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";


		public readonly string[] Mp31 = { 
			//Kids							//Adoracion
			@"http://174.142.111.104:9302", @"http://174.142.111.104:9984", 
			//Español						//Ingles
			@"http://174.142.111.104:9998", @"http://174.142.111.104:9992", 
			//Clasica						//Instrumental
			@"http://174.142.111.104:9982", @"http://174.142.111.104:9980" };
		string[] radioName = {
			"Kids", "Adoración",
			"Español", "Inglés",
			"Clásica", "Instrumental"};


		//private player player;
		MediaPlayer player;
        private AudioManager audioManager;
        private WifiManager wifiManager;
        private WifiManager.WifiLock wifiLock;
		private NotificationManager manager;
		private Notification myNotification;
        private bool paused;
        private int NotificationId;


		/// <summary>
		/// On create simply detect some of our managers
		/// </summary>
		public override void OnCreate()
		{
			base.OnCreate();
			//Find our audio and notificaton managers
			audioManager = (AudioManager)GetSystemService(AudioService);
			wifiManager = (WifiManager)GetSystemService(WifiService);
			manager = (NotificationManager)GetSystemService(NotificationService);
		}
        /// <summary>
        /// Don't do anything on bind
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
			NotificationId = sharedpref.GetInt("idNot", 8);

			manager.Cancel(11);
			switch (intent.Action)
			{
				case ActionPlay: Play(NotificationId); break;
				case ActionStop: Stop(); break;
					//case ActionPause: Pause(); break;
			}

			//Set sticky as we are a long running operation
			return StartCommandResult.Sticky;
		}

        private void IntializePlayer()
        {
			//player = new player(Stream.Music);
			player = new MediaPlayer();

            //Tell our player to sream music
            player.SetAudioStreamType(Stream.Music);

            //Wake mode will be partial to keep the CPU still running under lock screen
            player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

            //When we have prepared the song start playback
            player.Prepared += (sender, args) => player.Start();

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            //player.Completion += (sender, args) => Stop();


            player.Error += (sender, args) =>
            {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
               Stop();//this will clean up and reset properly.
            };
        }



        private void Play(int i)
        {

			if (paused && player != null)
			{
				paused = false;
				//We are simply paused so just start again
				player.Start();
				StartForeground();
				return;
			}

            if (player == null)
            {
                IntializePlayer();
            }

			if (player.IsPlaying)
				return;
                

            try
            {
                player.SetDataSource(ApplicationContext, Android.Net.Uri.Parse(Mp31[i]));

                var focusResult = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    //could not get audio focus
                    Console.WriteLine("Could not get audio focus");
                }

                player.PrepareAsync();
                AquireWifiLock();
                StartForeground();
            }
            catch (Exception ex)
            {
                //unable to start playback log error
                Console.WriteLine("Unable to start playback: " + ex);
            }
        }

		/// <summary>
		/// When we start on the foreground we will present a notification to the user
		/// When they press the notification it will take them to the main page so they can control the music
		/// </summary>
		private void StartForeground()
		{
			Intent aint = new Intent(this, typeof(PlayerActivity));
			var pendingIntent = PendingIntent.GetActivity(this, 0, aint, 0);


			Notification.Builder builder = new Notification.Builder(this);

			builder.SetAutoCancel(false);
			builder.SetTicker("Radios Fráter");
			builder.SetTicker("Reproduciendo");
			builder.SetContentTitle("Reproduciendo");
			builder.SetContentText(radioName[NotificationId]);
			builder.SetSmallIcon(Resource.Drawable.IconoMicrofono);
			builder.SetContentIntent(pendingIntent);
			builder.SetOngoing(true);
			//builder.SetSubText("This is subtext...");   //API level 16
														//builder.Build();

			myNotification = builder.Build();
			manager.Notify(11, myNotification);
			ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
			ISharedPreferencesEditor editor = sharedpref.Edit();
			editor.PutInt("idNot", NotificationId);
			editor.Commit();

		}

		//private void Pause()
		//{
		//    if (player == null)
		//        return;

		//    if(player.IsPlaying)
		//        player.Pause();

		//    StopForeground(true);
		//    paused = true;
		//}

		private void Stop()
		{
			if (player == null)
				return;

			if (player.IsPlaying)
				player.Stop();

			player.Reset();
			paused = false;
			//StopForeground(true);
			ReleaseWifiLock();

		}

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        private void AquireWifiLock()
        {
            if (wifiLock == null)
            {
                wifiLock = wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            }
            wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private void ReleaseWifiLock()
        {
            if (wifiLock == null)
                return;

            wifiLock.Release();
            wifiLock = null;
        }

		/// <summary>
		/// Properly cleanup of your player by releasing resources
		/// </summary>
		public override void OnDestroy()
		{
			base.OnDestroy();
			if (player != null)
			{
				player.Release();
				player = null;
			}

		}

		public override void OnTaskRemoved(Intent rootIntent)
		{
			if (player != null)
			{
				player.Release();
				player = null;
			}
			manager.Cancel(11);
			ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
			ISharedPreferencesEditor editor = sharedpref.Edit();
			editor.Clear();
			editor.Commit();

			base.OnTaskRemoved(rootIntent);
			System.Environment.Exit(0);
		}

		/// <summary>
		/// For a good user experience we should account for when audio focus has changed.
		/// There is only 1 audio output there may be several media services trying to use it so
		/// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
		/// All applications are encouraged to follow this, but are not enforced.
		/// </summary>
		/// <param name="focusChange"></param>
		public void OnAudioFocusChange(AudioFocus focusChange)
		{
			switch (focusChange)
			{
				case AudioFocus.Gain:
					if (player == null)
						IntializePlayer();

					if (!player.IsPlaying)
					{
						player.Start();
						paused = false;
					}

					player.SetVolume(1.0f, 1.0f);//Turn it up!
					break;
				case AudioFocus.Loss:
					//We have lost focus stop!
					Stop();
					break;
				case AudioFocus.LossTransient:
					//We have lost focus for a short time, but likely to resume so pause
					Stop();
					break;
				case AudioFocus.LossTransientCanDuck:
					//We have lost focus but should till play at a muted 10% volume
					if (player.IsPlaying)
						player.SetVolume(.1f, .1f);//turn it down!
					break;

			}
		}
    }
}