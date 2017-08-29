using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using RadiosFrater.Services;
using Com.OneSignal;

namespace RadiosFrater
{
	[Activity(Label = "Radios Fráter", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]

	public class MainActivity : Activity
	{
		GridView rdiogrid;
		WebView aboutView2, contactView;
		public int nRadio;
		bool isPlay;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			IO.Fabric.Sdk.Android.Fabric.With(this, new Com.Crashlytics.Android.Crashlytics());
			base.OnCreate(savedInstanceState);

			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			OneSignal.Current.StartInit("da713fcc-93ea-471b-932a-02d63c02bbfb").EndInit();

			ISharedPreferences sharedpref = GetSharedPreferences("PS", 0);
			isPlay = sharedpref.GetBoolean("isPlay", false);

			AddTab("Radios", 0, new SampleTabFragment());
			AddTab("Acerca de", 0, new SampleTabFragment());
			AddTab("Contáctenos", 0, new SampleTabFragment());



			rdiogrid = FindViewById<GridView>(Resource.Id.gridrdios);
			rdiogrid.Adapter = new ImageAdapter(this);

			aboutView2 = FindViewById<WebView>(Resource.Id.aboutView2);
			aboutView2.LoadUrl("http://radiosfrater.com/acerca-de-app/");
			contactView = FindViewById<WebView>(Resource.Id.contactView);
			contactView.LoadUrl("http://radiosfrater.com/contacto-app/");

			rdiogrid.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			{
				nRadio = args.Position;
				Intent player = new Intent(this, typeof(PlayerActivity));
				//player.PutExtra("id", nRadio);
				ISharedPreferencesEditor editor = sharedpref.Edit();
				editor.PutInt("idNot", nRadio);
				editor.Commit();
				player.PutExtra("isP", isPlay);
				if (nRadio == 6)
					StartActivityForResult(player, 7);
				else
					StartActivity(player);
			};
			//toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			//         SetActionBar(toolbar);
			//ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			//ActionBar.Title = "My Toolbar";

		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Finish();
		}

		protected override void OnStop()
		{
			base.OnStop();
			Finish();
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
		Resource.Drawable.FraterTV_2
			};
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt("tab", this.ActionBar.SelectedNavigationIndex);

			base.OnSaveInstanceState(outState);
		}

		void AddTab(string tabText, int iconResourceId, Fragment view)
		{
			//this-----------------
			var tab = this.ActionBar.NewTab();
			tab.SetText(tabText);
			//tab.SetIcon(iconResourceId);

			// must set event handler before adding tab
			tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				var fragment = this.FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);
				if (fragment != null)
					e.FragmentTransaction.Remove(fragment);
				e.FragmentTransaction.Add(Resource.Id.fragmentContainer, view);
				if (fragment != null && rdiogrid != null)
				{
					switch (tab.Position)
					{
						case 0:
							rdiogrid.Visibility = ViewStates.Visible;
							aboutView2.Visibility = ViewStates.Invisible;
							contactView.Visibility = ViewStates.Invisible;
							break;
						case 1:
							rdiogrid.Visibility = ViewStates.Invisible;
							aboutView2.Visibility = ViewStates.Visible;
							contactView.Visibility = ViewStates.Invisible;
							break;
						case 2:
							rdiogrid.Visibility = ViewStates.Invisible;
							aboutView2.Visibility = ViewStates.Invisible;
							contactView.Visibility = ViewStates.Visible;
							break;
						default:
							break;
					}
				}
			};
			tab.TabUnselected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				e.FragmentTransaction.Remove(view);
			};

			//this.ActionBar.AddTab(tab);
			ActionBar.AddTab(tab);
		}

		class SampleTabFragment : Fragment
		{
			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView(inflater, container, savedInstanceState);

				var view = inflater.Inflate(Resource.Layout.Tab, container, false);
				var sampleTextView = view.FindViewById<TextView>(Resource.Id.sampleTextView);
				sampleTextView.Text = "sample fragment text";

				return view;
			}
		}

		class SampleTabFragment2 : Fragment
		{
			public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
			{
				base.OnCreateView(inflater, container, savedInstanceState);

				var view = inflater.Inflate(Resource.Layout.Tab, container, false);
				var sampleTextView = view.FindViewById<TextView>(Resource.Id.sampleTextView);
				sampleTextView.Text = "sample fragment text 2";

				return view;
			}
		}


	}


}

