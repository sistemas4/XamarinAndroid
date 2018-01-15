
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
using Android.Widget;

namespace RadiosFrater
{
    [Activity(Label = "Fráter TV", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize, Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
	public class RadiosFragment : Android.Support.V4.App.Fragment
	{
		GridView rdiogrid;
		public int nRadio;


		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment 
			View view = inflater.Inflate(Resource.Layout.RadiosLayout, container, false);
			rdiogrid = view.FindViewById<GridView>(Resource.Id.gridrdios);
			rdiogrid.Adapter = new ImageAdapter(view.Context);
			rdiogrid.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			{
				nRadio = args.Position;
				Intent player = new Intent(view.Context, typeof(PlayerActivity));
				Intent Vplayer = new Intent(view.Context, typeof(VideoActivity));
				player.PutExtra("id", nRadio);

				if (nRadio == 7)
					StartActivity(Vplayer);
				else
					StartActivity(player);
			};

			return view;
		}


	}

	internal class ImageAdapter : BaseAdapter
	{
		Context context;

		public ImageAdapter(Context c)
		{
			context = c;
		}

		public override int Count
		{
			get { return thumbIds.Length; }
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override long GetItemId(int position)
		{
			return 0;
		}

		// create a new ImageView for each item referenced by the Adapter
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			ImageView imageView;

			if (convertView == null)
			{  // if it's not recycled, initialize some attributes
				imageView = new ImageView(context);
				imageView.LayoutParameters = new GridView.LayoutParams(200, 200);
				imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
				imageView.SetPadding(8, 16, 8, 8);
			}
			else
			{
				imageView = (ImageView)convertView;
			}

			imageView.SetImageResource(thumbIds[position]);
			return imageView;
		}

		// references to our images
		int[] thumbIds = {
		Resource.Drawable.KIds_2, Resource.Drawable.Adoracion_2,
			Resource.Drawable.Espanol_2, Resource.Drawable.Ingles_2,
			Resource.Drawable.Clasica_2, Resource.Drawable.Instrumental_2,
            Resource.Drawable.Predicas,	Resource.Drawable.FraterTV_2};	}
}

