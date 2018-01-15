
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace RadiosFrater
{
    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public async override void OnReceive(Context context, Intent intent)
        {
            var day = GetIso8601WeekOfYear(DateTime.Now);
            if (ItemPlay.versos == null)
                ItemPlay.versos = await MainActivity.GetVersiculos(day.ToString());
            
            var verso = await GetVersiculo(day.ToString());

            var message = verso.texto + "          -" + verso.cita;
            var title = "Semana #" + verso.semana;
            var subtitle = verso.cita;


            var notIntent = new Intent(context, typeof(VersosListActivity));
            var contentIntent = PendingIntent.GetActivity(context, 0, notIntent, PendingIntentFlags.CancelCurrent);
            var manager = NotificationManagerCompat.From(context);

            var style = new NotificationCompat.BigTextStyle();
            style.BigText(message);

            int resourceId;
            resourceId = Resource.Drawable.notification_bg_normal;

            var wearableExtender = new NotificationCompat.WearableExtender()
    .SetBackground(BitmapFactory.DecodeResource(context.Resources, resourceId))
                ;

            //Generate a notification with just short text and small icon
            var builder = new NotificationCompat.Builder(context)
                            .SetContentIntent(contentIntent)
                            .SetSmallIcon(Resource.Drawable.IconoMicrofono)
                            .SetContentTitle(title)
                                                .SetSubText(subtitle)
                            .SetContentText(message)
                            .SetStyle(style)
                            .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                            .SetAutoCancel(true)
                            .Extend(wearableExtender)
                            .SetDefaults((int)NotificationDefaults.All);
            var notification = builder.Build();
            manager.Notify(0, notification);
        }

        async Task<Versiculo> GetVersiculo(string day)
        {
            Versiculo verso;
            var API = "http://radiosfrater.com/api/get_verse/?week=" + day;
            using (var Client = new System.Net.Http.HttpClient())
            {
                var JSON = await Client.GetStringAsync(API);
                var versolist = Newtonsoft.Json.JsonConvert.DeserializeObject<VersiculoList>(JSON);
                verso = versolist.versiculo[0];
            }
            return verso;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}

