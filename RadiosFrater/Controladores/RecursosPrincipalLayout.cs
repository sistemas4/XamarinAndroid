
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
using Plugin.SecureStorage;

namespace RadiosFrater
{
    public class RecursosPrincipalLayout : Android.Support.V4.App.Fragment
    {

        ImageView predicas, versiculos;

        //RecursosActivity r;


        public RecursosPrincipalLayout(List<Predica> p, List<Autor> a, List<Categoria> c)
        {
            ItemPlay.recursos = p;
            ItemPlay.authors = a;
            ItemPlay.categorias = c;

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.RecursosPrincipalLayout, container, false);

            int height = this.Resources.DisplayMetrics.HeightPixels - 75;
            int width = this.Resources.DisplayMetrics.WidthPixels;

            predicas = view.FindViewById<ImageView>(Resource.Id.predicasbtn);
            versiculos = view.FindViewById<ImageView>(Resource.Id.versiculosbtn);
            predicas.SetImageResource(Resource.Drawable.Audifonos);
            versiculos.SetImageResource(Resource.Drawable.libro);
            predicas.Click += (sender, e) => {
                //r = new RecursosActivity(recursos, authors, categorias);
                Intent rf = new Intent(view.Context, typeof(RecursosActivity));
                StartActivity(rf);
            };
            versiculos.Click += (sender, e) => {
                var plani = CrossSecureStorage.Current.GetValue("Programada");
                if(plani == null){
                    Intent vr = new Intent(view.Context, typeof(InstructionActivity));
                    StartActivity(vr);
                } else {
                    Intent i = new Intent(view.Context, typeof(VersosListActivity));
                    StartActivity(i);
                }
            };

            return view;
        }
    }
}
