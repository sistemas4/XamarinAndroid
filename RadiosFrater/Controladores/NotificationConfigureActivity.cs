
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.SfPicker;
using Plugin.SecureStorage;
using Java.Util;

namespace RadiosFrater
{
    [Activity(Label = "NotificationConfigureActivity")]
    public class NotificationConfigureActivity : AppCompatActivity
    {
        string hora, min, frmt, id;
        public Android.Support.V7.Widget.Toolbar toolbar { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TimePicker timePicker = new TimePicker(this);
            base.OnCreate(savedInstanceState);

            int height = this.Resources.DisplayMetrics.HeightPixels - 75;
            int width = this.Resources.DisplayMetrics.WidthPixels;

            // Create your application here
            var edit = Intent.GetBooleanExtra("Editando", false);
            id = Intent.GetStringExtra("id");
            timePicker.BackgroundColor = new Android.Graphics.Color(255, 255, 255);
            timePicker.SelectedItemTextcolor = new Android.Graphics.Color(0, 19, 145);
            timePicker.ColumnHeaderHeight = 70;
            timePicker.HeaderTextSize = 18;
            timePicker.PickerHeight = 400;
            timePicker.PickerWidth = 300;
            timePicker.PickerMode = PickerMode.Default;
            timePicker.HeaderBackgroundColor = new Android.Graphics.Color(0, 19, 145);

            GridLayout gl = new GridLayout(this);
            gl.LayoutParameters = new ViewGroup.LayoutParams(Android.Views.ViewGroup.LayoutParams.MatchParent, Android.Views.ViewGroup.LayoutParams.MatchParent);

            FrameLayout.LayoutParams layoutParams1 = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, GravityFlags.Center);

            FrameLayout grid1 = new FrameLayout(this);
            Button cancel = new Button(this);
            cancel.Text = "Cancelar";
            cancel.SetBackgroundColor(Android.Graphics.Color.White);
            grid1.AddView(cancel, layoutParams1);
            grid1.LayoutParameters = new ViewGroup.LayoutParams((width - 20) / 2, ViewGroup.LayoutParams.WrapContent);



            FrameLayout grid2 = new FrameLayout(this);
            Button ok = new Button(this);
            ok.Text = "Aceptar";
            ok.SetBackgroundColor(Android.Graphics.Color.White);
            grid2.AddView(ok, layoutParams1);
            grid2.LayoutParameters = new ViewGroup.LayoutParams((width - 20) / 2, ViewGroup.LayoutParams.WrapContent);

            gl.AddView(grid1);
            gl.AddView(grid2);


            timePicker.FooterView = gl;
            timePicker.ShowFooter = true;

            timePicker.OnSelectionChanged += Picker_SelectionChanged;
            ok.Click += (sender, e) => {
                CrossSecureStorage.Current.SetValue("Id" + id, id);
                CrossSecureStorage.Current.SetValue("Hora" + id, hora + ":" + min + "  " + frmt);
                Remind();
                CrossSecureStorage.Current.SetValue("Programada", "S");
                Finish();
                if(!edit){
                    ItemPlay.horas = ItemPlay.gethoras();
                    Intent i = new Intent(this.BaseContext, typeof(RecordatoriosActivity));
                    i.PutExtra("first", true);
                    StartActivity(i);
                    Finish();
                } else {
                    ItemPlay.horas = ItemPlay.gethoras();
                    Intent va = new Intent(this.BaseContext, typeof(RecordatoriosActivity));
                    StartActivity(va);
                    Finish();
                }

            };
            cancel.Click += (sender, e) => {
                if (!edit)
                {
                    ItemPlay.horas = ItemPlay.gethoras();
                    Intent va = new Intent(this.BaseContext, typeof(VersosListActivity));
                    StartActivity(va);
                    Finish();
                }
                else
                {
                    Finish();
                }
            };
            SetContentView(timePicker);
        }

        private void Picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            hora = (e.NewValue as System.Collections.ObjectModel.ObservableCollection<object>)[0].ToString();
            min = (e.NewValue as System.Collections.ObjectModel.ObservableCollection<object>)[1].ToString();
            frmt = (e.NewValue as System.Collections.ObjectModel.ObservableCollection<object>)[2].ToString();
        }

        public void Remind()
        {
            int h = Int32.Parse(hora), m = Int32.Parse(min);
            int idint = Int32.Parse(id);
            if(frmt == "PM" && h != 12){
                h = h + 12;
            }
            Intent alarmIntent = new Intent(this.BaseContext, typeof(AlarmReceiver));
            Calendar e = Calendar.GetInstance(Java.Util.TimeZone.Default);
            e.Set(CalendarField.HourOfDay, h);
            e.Set(CalendarField.Minute, m);
            e.Set(CalendarField.Second, 0);


            PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, idint, alarmIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)this.GetSystemService(Context.AlarmService);

            alarmManager.SetRepeating(AlarmType.RtcWakeup, e.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
        }
    }

    public class TimePicker : SfPicker
    {
        // Time api is used to modify the Hour collection as per change in Time
        /// <summary>
        /// Time is the acutal DataSource for SfPicker control which will holds the collection of Hour ,Minute and Format
        /// </summary>
        public ObservableCollection<object> Time { get; set; }

        //Minute is the collection of minute numbers

        public ObservableCollection<object> Minute;

        //Hour is the collection of hour numbers

        public ObservableCollection<object> Hour;

        //Format is the collection of AM and PM

        public ObservableCollection<object> Format;

        public ObservableCollection<string> SelectedTime;

        public TimePicker(Context context) : base(context)
        {
            ObservableCollection<string> todaycollection = new ObservableCollection<string>();

            DateTime daytime = DateTime.Now;
            daytime.ToString("dd/mm/yyy HH:mm:ss");

            //Current hour is selected if hour is less than 13 else it is subtracted by 12 to maintain 12hour format
            if (daytime.Hour < 13)
            {
                todaycollection.Add(DateTime.Now.Hour.ToString());
            }
            else
            {
                todaycollection.Add((DateTime.Now.Hour - 12).ToString());
            }

            //Current minute is selected
            todaycollection.Add(DateTime.Now.Minute.ToString());

            //Format is selected as AM if hour is less than 12 else PM is selected
            if (daytime.Hour < 12)
            {
                todaycollection.Add("AM");
            }
            else
            {
                todaycollection.Add("PM");
            }

            //Update the current time
            this.SelectedTime = todaycollection;



            Time = new ObservableCollection<object>();
            Hour = new ObservableCollection<object>();
            Minute = new ObservableCollection<object>();
            Format = new ObservableCollection<object>();
            Headers = new ObservableCollection<string>();

            Headers.Add("HORA");
            Headers.Add("MINUTO");
            Headers.Add("FORMATO");

            //Enable Footer of SfPicker
            ShowFooter = true;

            //Enable Header of SfPicker
            ShowHeader = true;

            //Enable Column Header of SfPicker
            ShowColumnHeader = true;


            //SfPicker header text
            HeaderText = "Selecciona la hora para tu alerta diaria";

            PopulateTimeCollection();
            this.ItemsSource = Time;

            // Column header text collection
            this.ColumnHeaderText = Headers;
        }


        /// <summary>
        /// Header api is holds the column name for every column in time picker
        /// </summary>

        public ObservableCollection<string> Headers { get; set; }

        private void PopulateTimeCollection()
        {
            //Populate Hour
            for (int i = 1; i <= 12; i++)
            {
                Hour.Add(i.ToString());
            }

            //Populate Minute
            for (int j = 0; j < 60; j++)
            {
                if (j < 10)
                {
                    Minute.Add("0" + j);
                }
                else
                    Minute.Add(j.ToString());
            }

            //Populate Format
            Format.Add("AM");
            Format.Add("PM");

            Time.Add(Hour);
            Time.Add(Minute);
            Time.Add(Format);
        }
    }
}
