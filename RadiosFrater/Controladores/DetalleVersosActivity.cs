
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Media.Session;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RadiosFrater.Services;

namespace RadiosFrater
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class DetalleVersosActivity : AppCompatActivity
    {
        readonly string[] PermissionWrite =
        {
             Manifest.Permission.WriteExternalStorage,
             Manifest.Permission.ReadExternalStorage
        };
        const int RequestLocationId = 0;

        Android.Support.V7.Widget.Toolbar toolbar { get; set; }
        TextView semana;
        ImageButton btnPS, compartir;
        ImageView imgverso;
        Versiculo verso;

        bool isBound = false;
        private MediaPlayerServiceBinder binder;
        MediaPlayerServiceConnection mediaPlayerServiceConnection;
        private Intent mediaPlayerServiceIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DetalleVersoLayout);
            // Create your application here
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            verso = ItemPlay.versoactual;

            semana = FindViewById<TextView>(Resource.Id.titsem);
            imgverso = FindViewById<ImageView>(Resource.Id.imgverso);
            btnPS = FindViewById<ImageButton>(Resource.Id.btnFRV);
            compartir = FindViewById<ImageButton>(Resource.Id.compartirverso);


            semana.Text = "Semana #" + verso.semana;

            var imageBitmap = GetImageBitmapFromUrl(verso.imagenv);
            imgverso.SetImageBitmap(imageBitmap);
            const string permission = Manifest.Permission.WriteExternalStorage;
            btnPS.Click += async (sender, e) =>
            {
                if (binder.GetMediaPlayerService().mediaPlayer != null && binder.GetMediaPlayerService().MediaPlayerState == PlaybackStateCompat.StatePlaying)
                {
                    await binder.GetMediaPlayerService().Pause();
                    btnPS.SetImageResource(Resource.Drawable.playRF);
                }
                else
                {
                    btnPS.Enabled = false;
                    btnPS.SetImageResource(Resource.Drawable.pausaRF);
                    await binder.GetMediaPlayerService().Play(verso.fragverso);
                    btnPS.Enabled = true;
                }
            };
            compartir.Click += async (sender, e) =>
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    await Share(imageBitmap);
                    return;
                }
                if (CheckSelfPermission(permission) == (int)Permission.Granted)
                {
                    await Share(imageBitmap);
                    return;
                }
                await GetMemoryPermissionAsync(imageBitmap);
            };

            SupportActionBar.Title = "Regresar";

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (mediaPlayerServiceConnection == null)
                InitilizeMedia();


        }

        async Task GetMemoryPermissionAsync(Bitmap c)
        {
            //Check to see if any permission in our group is available, if one, then all are
            const string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                await Share(c);
                return;
            }

            //Finally request permissions with the list of permissions and Id
            RequestPermissions(PermissionWrite, RequestLocationId);
        }

        async Task Share(Bitmap c)
        {
            if (c == null)
                return;

            Bitmap b = c;

            var tempFilename = "test.png";
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = System.IO.Path.Combine(sdCardPath, tempFilename);
            using (var os = new FileStream(filePath, FileMode.Create))
            {
                b.Compress(Bitmap.CompressFormat.Png, 100, os);
            }
            b.Dispose();

            var imageUri = Android.Net.Uri.Parse($"file://{sdCardPath}/{tempFilename}");
            var sharingIntent = new Intent();
            sharingIntent.SetAction(Intent.ActionSend);
            sharingIntent.SetType("image/*");
            sharingIntent.PutExtra(Intent.ExtraStream, imageUri);
            sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            StartActivity(Intent.CreateChooser(sharingIntent, "Compartir cita"));
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

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        public void initPlay()
        {
            if (binder.GetMediaPlayerService().mediaPlayer != null && binder.GetMediaPlayerService().MediaPlayerState == PlaybackStateCompat.StatePlaying)
            {
                btnPS.SetImageResource(Resource.Drawable.pausaRF);
            }
            else
            {
                btnPS.SetImageResource(Resource.Drawable.playRF);

            }
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }

        private void InitilizeMedia()
        {
            mediaPlayerServiceIntent = new Intent(ApplicationContext, typeof(MediaPlayerService));
            mediaPlayerServiceConnection = new MediaPlayerServiceConnection(this);
            BindService(mediaPlayerServiceIntent, mediaPlayerServiceConnection, Bind.AutoCreate);
        }

        class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
        {
            DetalleVersosActivity instance;

            public MediaPlayerServiceConnection(DetalleVersosActivity mediaPlayer)
            {
                this.instance = mediaPlayer;
            }

            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                var mediaPlayerServiceBinder = service as MediaPlayerServiceBinder;
                if (mediaPlayerServiceBinder != null)
                {
                    var binder = (MediaPlayerServiceBinder)service;
                    if (MediaPlayerService.ServiceActive != null)
                        instance.binder = MediaPlayerService.ServiceActive;
                    else if (MediaPlayerService.ServiceActive == null)
                    {
                        instance.binder = binder;
                        MediaPlayerService.ServiceActive = binder;
                    }
                    instance.isBound = true;
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
