
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
using Plugin.SecureStorage;

namespace RadiosFrater
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class RecordatoriosActivity : AppCompatActivity
    {
        ListView alarmList;
        TextView alarm;
        AlarmAdapter adapter;
        List<string> alarmL;
        public Android.Support.V7.Widget.Toolbar toolbar { get; set; }
        bool first;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditarALayout);  
            // Create your application here
            first = Intent.GetBooleanExtra("first", false);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Alertas";

            //for (int i = 1; i <= 3; i++){
            //    var hora = CrossSecureStorage.Current.GetValue("Hora" + i.ToString());
            //    if (hora == null)
            //        hora = "00:00 --";
            //    ItemPlay.horas.Add(hora);
            //}
            TextView tit = FindViewById<TextView>(Resource.Id.tit);
            Button brrprog = FindViewById<Button>(Resource.Id.erprog);
            alarmList = FindViewById<ListView>(Resource.Id.listAlarms);
            brrprog.Click += BorrarAlarmas;
            alarmList.Adapter = new AlarmAdapter(this.BaseContext, ItemPlay.horas);
            alarmList.ItemClick += clickItem;
            alarmL = ItemPlay.horas;
            if (!first){
                tit.Text = "Alertas diarias";
            }   
            else{
                tit.Text = "Configura las tres alertas con la cita bíblica a memorizar";
                string txtconf = "", txtnext = "";
                int i = 3, a = 0, c = 0;
                foreach(var hour in ItemPlay.horas){
                    if (hour != "00:00 --")
                        i--;
                }
                switch (i)
                {
                    case 1:
                        txtconf = "segunda alerta,";
                        txtnext = " ahora puedes programar la tercera.";
                        a = 3;
                        c = 2;
                        break;
                    case 2:
                        txtconf = "primera alerta,";
                        txtnext = " ahora puedes programar la segunda.";
                        a = 2;
                        c = 1;
                        break;
                    case 0:
                        txtconf = "tercera alerta.";
                        txtnext = " Has terminado con la configuración.";
                        break;
                    default:
                        break;
                }


                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Configuración exitosa");
                alert.SetMessage("Has configurado la " + txtconf + txtnext);
                if(a == 0)
                    alert.SetButton("Aceptar", (sender, e) => { 
                    Android.App.AlertDialog confalert = dialog.Create();
                    confalert.SetTitle("Configuración exitosa");
                    confalert.SetMessage("Has configurado 3 alertas.");
                    confalert.SetButton("Aceptar",(s, es) => {});
                    confalert.Show();
                });
                else {
                    alert.SetButton("Continuar", (sender, e) => {
                        Intent vr = new Intent(this.BaseContext, typeof(NotificationConfigureActivity));
                        vr.PutExtra("id", a.ToString());
                        Finish();
                        StartActivity(vr);
                    });
                    alert.SetButton2("Cancelar", (sender, e) => { 
                        Android.App.AlertDialog confalert = dialog.Create();
                        confalert.SetTitle("Configuración exitosa");
                        if(c == 1)
                            confalert.SetMessage("Has configurado 1 alerta.");
                        else 
                            confalert.SetMessage("Has configurado 2 alertas.");
                        confalert.SetButton("Aceptar", (s, es) => { });
                        confalert.Show();
                    });
                }
                alert.Show();

            }
                
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private void BorrarAlarmas(object sender, EventArgs e)
        {
            CrossSecureStorage.Current.DeleteKey("id" + 1);
            CrossSecureStorage.Current.DeleteKey("Hora" + 1);
            CrossSecureStorage.Current.DeleteKey("id" + 2);
            CrossSecureStorage.Current.DeleteKey("Hora" + 2);
            CrossSecureStorage.Current.DeleteKey("id" + 3);
            CrossSecureStorage.Current.DeleteKey("Hora" + 3);
            CrossSecureStorage.Current.DeleteKey("Programada");
            for (int i = 1; i <= 3; i++)
            {
                alarmL[i - 1] = "00:00 --";
                DeleteIntent(i);
            }
            ItemPlay.horas = ItemPlay.gethoras();
            adapter = new AlarmAdapter(this.BaseContext, ItemPlay.horas);
            alarmList.Adapter = adapter;
            alarmL = ItemPlay.horas;
        }

        private void clickItem(object sender, AdapterView.ItemClickEventArgs e)
        {
            int id = unchecked((int)e.Id);
            id = id + 1;
            Intent vr = new Intent(this.BaseContext, typeof(NotificationConfigureActivity));
            if(!first)
                vr.PutExtra("Editando", true);
            vr.PutExtra("id", id.ToString());
            Finish();
            StartActivity(vr);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if(!first)
                        Finish();
                    else {
                        Intent vc = new Intent(this.BaseContext, typeof(VersosListActivity));
                        StartActivity(vc);
                        Finish();
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        public override void OnBackPressed()
        {
            if (!first)
                Finish();
            else
            {
                Intent vc = new Intent(this.BaseContext, typeof(VersosListActivity));
                StartActivity(vc);
                Finish();
            }
            base.OnBackPressed();
        }

        public void DeleteIntent(int i)
        {
            Intent alarmIntent = new Intent(this.BaseContext, typeof(AlarmReceiver));

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(this.BaseContext, i, alarmIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)this.BaseContext.GetSystemService(Context.AlarmService);
            pendingIntent.Cancel();
            alarmManager.Cancel(pendingIntent);

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }
    }

    public class AlarmAdapter : BaseAdapter<string>
    {
        public List<string> rList;
        private Context rContext;
        public AlarmAdapter(Context context, List<string> list)
        {
            rList = list;
            rContext = context;
        }

        public override String this[int position]
        {
            get { return rList[position]; }
        }

        public override int Count
        {
            get { return rList.Count(); }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row != null)
                row = null;
            try
            {
                var id = position + 1;
                var hora = CrossSecureStorage.Current.GetValue("Hora" + id);
                if (row == null)
                {
                    row = LayoutInflater.From(rContext).Inflate(Resource.Layout.EditarALayout, null, false);
                }
                TextView tit = row.FindViewById<TextView>(Resource.Id.tit);
                TextView txt = row.FindViewById<TextView>(Resource.Id.Alarm);
                ImageView borrar = row.FindViewById<ImageView>(Resource.Id.erase);
                Button brrprog = row.FindViewById<Button>(Resource.Id.erprog);
                borrar.SetImageResource(Resource.Drawable.erase);
                Android.Support.V7.Widget.Toolbar toolbar = row.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                toolbar.Visibility = ViewStates.Gone;
                tit.Visibility = ViewStates.Gone;
                brrprog.Visibility = ViewStates.Gone;
                if (hora == null)
                    borrar.Visibility = ViewStates.Gone;
                borrar.Click += (sender, e) => {
                    CrossSecureStorage.Current.DeleteKey("id" + id);
                    CrossSecureStorage.Current.DeleteKey("Hora" + id);
                    borrar.Visibility = ViewStates.Gone;
                    txt.Text = "00:00 --";
                    DeleteIntent(id);
                };
                txt.Text = rList[position];

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return row;
        }

        public void DeleteIntent(int i)
        {
            Intent alarmIntent = new Intent(rContext, typeof(AlarmReceiver));

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(rContext, i, alarmIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)rContext.GetSystemService(Context.AlarmService);
            pendingIntent.Cancel();
            alarmManager.Cancel(pendingIntent);

        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }


    }
}
