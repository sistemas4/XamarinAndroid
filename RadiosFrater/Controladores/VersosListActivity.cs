
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RadiosFrater.Services;

namespace RadiosFrater
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class VersosListActivity : AppCompatActivity
    {
        ListView versosList;
        List<Versiculo> versos;
        TextView cita, texto;
        VersosAdapter adapter;
        string wk;
        public Android.Support.V7.Widget.Toolbar toolbar { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VersosLayout);
            // Create your application here
            wk = AlarmReceiver.GetIso8601WeekOfYear(DateTime.Now).ToString();
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Versículos";
            adapter = new VersosAdapter(this.BaseContext, ItemPlay.versos, wk);
            versosList = FindViewById<ListView>(Resource.Id.listVersos);
            versosList.Adapter = adapter;
            texto = FindViewById<TextView>(Resource.Id.Texto);
            cita = FindViewById<TextView>(Resource.Id.Cita);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            versos = ItemPlay.versos;
            versosList.ItemClick += clickadapter;

        }

        private void clickadapter(object sender, AdapterView.ItemClickEventArgs e)
        {
            int id = unchecked((int)e.Id);
            Intent DVerso = new Intent(this.BaseContext, typeof(DetalleVersosActivity));
            Versiculo selected = versos[id];
            if (selected != ItemPlay.versoactual)
                MediaPlayerService.ServiceActive = null;
            ItemPlay.versoactual = selected;
            StartActivity(DVerso);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MenuVersos, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menu_edit:
                    ItemPlay.horas = ItemPlay.gethoras();
                    Intent vr = new Intent(this.BaseContext, typeof(RecordatoriosActivity));
                    StartActivity(vr);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }
    }

    public class VersosAdapter : BaseAdapter<Versiculo>
    {
        public List<Versiculo> sList;
        string wek;
        private Context sContext;
        public VersosAdapter(Context context, List<Versiculo> list, string wk)
        {
            sList = list;
            sContext = context;
            wek = wk;
        }

        public override Versiculo this[int position]
        {
            get { return sList[position]; }
        }

        public override int Count
        {
            get { return sList.Count(); }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            try
            {
                if (row == null)
                {
                    row = LayoutInflater.From(sContext).Inflate(Resource.Layout.VersosLayout, null, false);
                }
                TextView txt = row.FindViewById<TextView>(Resource.Id.Texto);
                TextView cita = row.FindViewById<TextView>(Resource.Id.Cita);
                TextView sem = row.FindViewById<TextView>(Resource.Id.Semana);
                TextView titv = row.FindViewById<TextView>(Resource.Id.titlever);
                Android.Support.V7.Widget.Toolbar toolbar = row.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                toolbar.Visibility = ViewStates.Gone;
                titv.Visibility = ViewStates.Gone;
                if (sList[position].semana == wek)
                    row.SetBackgroundColor(Android.Graphics.Color.Rgb(165, 195, 255));
                sem.Text = "Semana #" + sList[position].semana;
                txt.Text = sList[position].texto;
                cita.Text = sList[position].cita;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return row;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }


    }
}
