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
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.Design;
using Android.Support.V4.App;
using System.Threading.Tasks;
using Android.Support.V4.Media.Session;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.SecureStorage;
using Java.Util;

namespace RadiosFrater
{
	[Activity(Label = "Radios Fráter", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]

	public class MainActivity : AppCompatActivity
	{
		TabLayout tabLayout;
        bool isConnected;
        public List<Predica> recursos { get; set; }
        public List<Autor> authors { get; set; }
        public List<Categoria> categorias { get; set; }
        //public List<Versiculo> versos { get; set; }

		protected async override void OnCreate(Bundle savedInstanceState)
		{
            AppCenter.Start("5d1a9d5f-ab1b-48ba-8e5c-cac97d4c7699",
                   typeof(Analytics), typeof(Crashes));
			IO.Fabric.Sdk.Android.Fabric.With(this, new Com.Crashlytics.Android.Crashlytics());
			base.OnCreate(savedInstanceState);
            isConnected = DoIHaveInternet();
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
            SecureStorageImplementation.StoragePassword = "test";
			OneSignal.Current.StartInit("da713fcc-93ea-471b-932a-02d63c02bbfb").EndInit();
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);

			tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabsIcon);
            var wk = AlarmReceiver.GetIso8601WeekOfYear(DateTime.Now).ToString();
            if (ItemPlay.recursos == null)
                recursos = await GetRecursos();
            else
                recursos = ItemPlay.recursos;

            if (ItemPlay.authors == null)
                authors = await GetAutores();
            else
                authors = ItemPlay.authors;

            if (ItemPlay.categorias == null)
                categorias = await GetCategorias();
            else
                categorias = ItemPlay.categorias;
            if (ItemPlay.versos == null)
                ItemPlay.versos = await GetVersiculos(wk);
			FnInitTabLayout();


		}

		void FnInitTabLayout()
		{
			tabLayout.SetTabTextColors(Android.Graphics.Color.Aqua, Android.Graphics.Color.AntiqueWhite);
            //Fragment array
            var fragments = new Android.Support.V4.App.Fragment[]
            {
                new RadiosFragment(),
                new AboutWebVFragment(),
                new ContactWbFragment(),
                new RecursosPrincipalLayout(recursos, authors, categorias)
			};
			//Tab title array
			var titles = CharSequence.ArrayFromStringArray(new[] {
				"Radios",
				"Acerca de",
				"Contacto",
                "Recursos"
			});
			var viewPager = FindViewById<ViewPager>(Resource.Id.viewpagerIcon);
			//viewpager holding fragment array and tab title text
			viewPager.Adapter = new TabsFragmentPagerAdapter(SupportFragmentManager, fragments, titles);

			// Give the TabLayout the ViewPager 
			tabLayout.SetupWithViewPager(viewPager);
			//tabLayout.SetTabTextColors(
			//FnSetIcons();
			FnSetupTabIconsWithText();
		}

		private void FnSetupTabIconsWithText()
		{
			View view = LayoutInflater.Inflate(Resource.Layout.custom_text, null);
			var custTabOne = view.FindViewById<TextView>(Resource.Id.txtTabText);
			custTabOne.Text = "Radios";
			//custTabOne.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_call, 0, 0, 0);
			tabLayout.GetTabAt(0).SetCustomView(custTabOne);

			View view1 = LayoutInflater.Inflate(Resource.Layout.custom_text, null);
			TextView custTabTwo = view1.FindViewById<TextView>(Resource.Id.txtTabText);//(TextView)LayoutInflater.Inflate (Resource.Layout.custom_text, null); ;
			custTabTwo.Text = "Acerca de";
			//custTabTwo.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_msg, 0, 0, 0);
			tabLayout.GetTabAt(1).SetCustomView(custTabTwo);

			View view2 = LayoutInflater.Inflate(Resource.Layout.custom_text, null);
			TextView custTabThree = view2.FindViewById<TextView>(Resource.Id.txtTabText);//(TextView)LayoutInflater.Inflate (Resource.Layout.custom_text, null); ;
			custTabThree.Text = "Contacto";
			//custTabThree.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_wifi, 0, 0, 0);
			tabLayout.GetTabAt(2).SetCustomView(custTabThree);

            View view3 = LayoutInflater.Inflate(Resource.Layout.custom_text, null);
            TextView custTabFour = view3.FindViewById<TextView>(Resource.Id.txtTabText);//(TextView)LayoutInflater.Inflate (Resource.Layout.custom_text, null); ;
            custTabFour.Text = "Recursos";
            //custTabThree.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_wifi, 0, 0, 0);
            tabLayout.GetTabAt(3).SetCustomView(custTabFour);
		}

        async Task<List<Predica>> GetRecursos()
        {
            Recursos Predicas;
            var API = "http://radiosfrater.com/api/get_recent_events";
            using (var Client = new System.Net.Http.HttpClient())
            {
                var JSON = await Client.GetStringAsync(API);
                Predicas = Newtonsoft.Json.JsonConvert.DeserializeObject<Recursos>(JSON);
            }
            return Predicas.posts;
        }

        async Task<List<Autor>> GetAutores()
        {
            Predicadores Autores;
            var API = "http://radiosfrater.com/api/get_authors";
            using (var Client = new System.Net.Http.HttpClient())
            {
                var JSON = await Client.GetStringAsync(API);
                Autores = Newtonsoft.Json.JsonConvert.DeserializeObject<Predicadores>(JSON);
            }
            return Autores.autores;
        }

        async Task<List<Categoria>> GetCategorias()
        {
            Categorias categories;
            var API = "http://radiosfrater.com/api/get_categories";
            using (var Client = new System.Net.Http.HttpClient())
            {
                var JSON = await Client.GetStringAsync(API);
                categories = Newtonsoft.Json.JsonConvert.DeserializeObject<Categorias>(JSON);
            }
            return categories.categorias;
        }

        public static async Task<List<Versiculo>> GetVersiculos(string day)
        {
            VersiculoList verso;
            var API = "http://radiosfrater.com/api/get_allverse/?week=" + day;
            using (var Client = new System.Net.Http.HttpClient())
            {
                var JSON = await Client.GetStringAsync(API);
                verso = Newtonsoft.Json.JsonConvert.DeserializeObject<VersiculoList>(JSON);
            }
            return verso.versiculo;
        }

        public bool DoIHaveInternet()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        public void destroy(){
            OnDestroy();
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();
            Finish();
		}

		protected override void OnStop()
		{
			base.OnStop();
		}

		//protected override void OnSaveInstanceState(Bundle outState)
		//{
		//	outState.PutInt("tab", ActionBar.SelectedNavigationIndex);

		//	base.OnSaveInstanceState(outState);
		//}


	}




}

