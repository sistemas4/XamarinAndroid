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

namespace RadiosFrater
{
	[Activity(Label = "Radios Fráter", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]

	public class MainActivity : AppCompatActivity
	{
		TabLayout tabLayout;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			IO.Fabric.Sdk.Android.Fabric.With(this, new Com.Crashlytics.Android.Crashlytics());
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			OneSignal.Current.StartInit("da713fcc-93ea-471b-932a-02d63c02bbfb").EndInit();
			var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
			SetSupportActionBar(toolbar);

			tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabsIcon);


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


			};
			//Tab title array
			var titles = CharSequence.ArrayFromStringArray(new[] {
				"Radios",
				"Acerca de",
				"Contáctenos"
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
			custTabThree.Text = "Contáctenos";
			//custTabThree.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_wifi, 0, 0, 0);
			tabLayout.GetTabAt(2).SetCustomView(custTabThree);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
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

