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
    [Activity(Label = "recursos", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class DetalleRecursosActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar { get; set; }
        TextView titulo, autor, fragmento, cargando;
        ImageButton btnPS, compartir;
        ImageView autorimg;
        Predica Predica;

        bool isBound = false;
        private MediaPlayerServiceBinder binder;
        MediaPlayerServiceConnection mediaPlayerServiceConnection;
        private Intent mediaPlayerServiceIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.DetalleRecLayout);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            Predica = ItemPlay.shareobj;

            titulo = FindViewById<TextView>(Resource.Id.titulo);
            autor = FindViewById<TextView>(Resource.Id.autor);
            fragmento = FindViewById<TextView>(Resource.Id.fragmento);
            cargando = FindViewById<TextView>(Resource.Id.cargando);
            compartir = FindViewById<ImageButton>(Resource.Id.compartir);
            autorimg = FindViewById<ImageView>(Resource.Id.autorimg);
            btnPS = FindViewById<ImageButton>(Resource.Id.btnPSDR);
            //compartir.SetImageResource(Resource.Drawable.Compartir);
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
                    cargando.Visibility = ViewStates.Visible;
                    await binder.GetMediaPlayerService().Play(Predica.url);
                    cargando.Visibility = ViewStates.Invisible;
                    btnPS.Enabled = true;
                }
            };
            compartir.Click += (sender, e) => {
                Intent shareIntent = new Intent(Intent.ActionSend);
                shareIntent.SetType("text/plain");
                shareIntent.PutExtra(Intent.ExtraText, "Estoy escuchando " + Predica.titulo + ", en Radios Fráter " + Predica.urlpost);
                StartActivity(Intent.CreateChooser(shareIntent, "Compartir predica"));
            };

            titulo.Text = Predica.titulo;
            autor.Text = "Por: " + Predica.autor;
            fragmento.Text = Predica.descripcion;

            var imageBitmap = GetImageBitmapFromUrl(Predica.imgAutor);
            autorimg.SetImageBitmap(imageBitmap);

            SupportActionBar.Title = "Regresar";

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (mediaPlayerServiceConnection == null)
                InitilizeMedia();
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
            DetalleRecursosActivity instance;

            public MediaPlayerServiceConnection(DetalleRecursosActivity mediaPlayer)
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
                    else if (MediaPlayerService.ServiceActive == null){
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
