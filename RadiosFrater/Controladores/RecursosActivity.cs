
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RadiosFrater.Services;

namespace RadiosFrater
{
    [Activity(Label = "Prédicas")]
    public class RecursosActivity : AppCompatActivity
    {
        bool filter, RisShowed, filtera, filterc;
        public List<Predica> recursos;
        public List<Predica> recursosF;
        public List<Autor> authors;
        public List<Categoria> categorias;
        ListView RecursosV;
        private SearchView sv2;
        private Button btnaut, btncat;
        RecursosAdapter adapter;
        AutorAdapter Aadapter;
        CategoryAdapter Cadapter;
        public Android.Support.V7.Widget.Toolbar toolbar { get; set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RecursosLayout);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Recursos";
            recursos = ItemPlay.recursos;
            authors = ItemPlay.authors;
            categorias = ItemPlay.categorias;
            RecursosV = FindViewById<ListView>(Resource.Id.listRecursos);
            //sv = view.FindViewById<EditText>(Resource.Id.sv);
            sv2 = FindViewById<SearchView>(Resource.Id.sv2);
            //btnaut = FindViewById<Button>(Resource.Id.autbtn);
            //btncat = FindViewById<Button>(Resource.Id.catbtn);
            adapter = new RecursosAdapter(this.BaseContext, recursos);
            //Aadapter = new AutorAdapter(this.BaseContext, authors);
            //Cadapter = new CategoryAdapter(this.BaseContext, categorias);
            setPAdapter();
            clickAdapterItem();
            // FILTRO DE AUTORES
            //btnaut.Click += (sender, e) => {
            //    if (!filter)
            //    {
            //        RecursosV.Adapter = Aadapter;
            //        btnaut.Text = "TODO";
            //        btncat.Visibility = ViewStates.Invisible;
            //        RisShowed = false;
            //        filter = true;
            //        filtera = true;

            //    }
            //    else
            //    {
            //        btnaut.Text = "Oradores";
            //        btncat.Visibility = ViewStates.Visible;
            //        RisShowed = true;
            //        filter = false;
            //        filtera = false;
            //        setPAdapter();
            //    }
            //};

            // FILTRO DE CATEGORIAS
            //btncat.Click += (sender, e) => {
            //    if (!filter)
            //    {
            //        RecursosV.Adapter = Cadapter;
            //        btncat.Text = "TODO";
            //        btnaut.Visibility = ViewStates.Invisible;
            //        RisShowed = false;
            //        filter = true;
            //        filterc = true;

            //    }
            //    else
            //    {
            //        btncat.Text = "Categorias";
            //        btnaut.Visibility = ViewStates.Visible;
            //        filter = false;
            //        filterc = false;
            //        setPAdapter();
            //    }
            //};

            sv2.QueryTextChange += svchange;
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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

        void setPAdapter()
        {
            RecursosV.Adapter = adapter;
            recursosF = recursos;
            RisShowed = true;

        }
        void clickAdapterItem()
        {
            RecursosV.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                if (!filtera && RisShowed && !filterc)
                {
                    int id = unchecked((int)e.Id);
                    Intent DRecursos = new Intent(this.BaseContext, typeof(DetalleRecursosActivity));
                    Predica selected = recursosF[id];
                    if (selected != ItemPlay.shareobj)
                        MediaPlayerService.ServiceActive = null;
                    ItemPlay.shareobj = selected;
                    StartActivity(DRecursos);
                }
                else if (filtera && !RisShowed)
                {
                    int id = unchecked((int)e.Id);
                    List<Predica> list = (from items in recursos
                                          where items.autor.Contains(authors[id].nombre)
                                          select items).ToList<Predica>();
                    btnaut.Text = "TODO";
                    btncat.Visibility = ViewStates.Invisible;

                    RisShowed = true;
                    filtera = false;
                    recursosF = list;
                    // bind the result with adapter  
                    RecursosV.Adapter = new RecursosAdapter(this.BaseContext, list);
                }
                else if (filterc && !RisShowed)
                {
                    int id = unchecked((int)e.Id);
                    List<Predica> list = (from items in recursos
                                          where items.categoria.Contains(categorias[id].nombre)
                                          select items).ToList<Predica>();
                    btncat.Text = "TODO";
                    btnaut.Visibility = ViewStates.Invisible;

                    RisShowed = true;
                    filterc = false;
                    recursosF = list;
                    // bind the result with adapter  
                    RecursosV.Adapter = new RecursosAdapter(this.BaseContext, list);
                }

            };
        }

        void cleanAll()
        {
            filter = false;
            filtera = false;
            filterc = false;
            RisShowed = true;

            recursosF = recursos;

            btnaut.Text = "Oradores";
            btncat.Visibility = ViewStates.Visible;

            btncat.Text = "Categorias";
            btnaut.Visibility = ViewStates.Visible;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }

        private void svchange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            if (filter)
                cleanAll();
            var searchText = e.NewText.ToLower();

            //Compare the entered text with List  
            List<Predica> list = (from items in recursos
                                  where items.autor.ToLower().Contains(searchText) ||
                                                 items.titulo.ToLower().Contains(searchText)
                                  select items).ToList<Predica>();
            recursosF = list;
            // bind the result with adapter  
            RecursosV.Adapter = new RecursosAdapter(this.BaseContext, list);
        }

        //private void svQueryChange(object sender, TextChangedEventArgs e)
        //{
        //    var searchText = sv.Text.ToLower();

        //    //Compare the entered text with List  
        //    List<Predica> list = (from items in recursos
        //                       where items.autor.ToLower().Contains(searchText) ||
        //                                      items.titulo.ToLower().Contains(searchText)
        //                       select items).ToList<Predica>();

        //    // bind the result with adapter  
        //    RecursosV.Adapter = new RecursosAdapter(this.Context, list);
        //}
    }



    public class RecursosAdapter : BaseAdapter<Predica>
    {
        public List<Predica> sList;
        public List<Autor> aList;
        public List<Categoria> cList;
        private Context sContext;
        public RecursosAdapter(Context context, List<Predica> list)
        {
            sList = list;
            sContext = context;
        }

        public override Predica this[int position]
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
                    row = LayoutInflater.From(sContext).Inflate(Resource.Layout.RecursosLayout, null, false);
                }
                TextView txtName = row.FindViewById<TextView>(Resource.Id.Name);
                TextView txtAutor = row.FindViewById<TextView>(Resource.Id.Autor);
                SearchView sv = row.FindViewById<SearchView>(Resource.Id.sv2);
                //Button btn = row.FindViewById<Button>(Resource.Id.autbtn);
                //Button btn2 = row.FindViewById<Button>(Resource.Id.catbtn);
                TextView tit = row.FindViewById<TextView>(Resource.Id.titlerec);
                Android.Support.V7.Widget.Toolbar toolbar = row.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                sv.Visibility = ViewStates.Gone;
                //btn.Visibility = ViewStates.Gone;
                //btn2.Visibility = ViewStates.Gone;
                toolbar.Visibility = ViewStates.Gone;
                tit.Visibility = ViewStates.Gone;

                txtName.Text = sList[position].titulo;
                txtAutor.Text = "Por: " + sList[position].autor;

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

    public class AutorAdapter : BaseAdapter<Autor>
    {
        public List<Autor> sList;
        private Context sContext;
        public AutorAdapter(Context context, List<Autor> list)
        {
            sList = list;
            sContext = context;
        }

        public override Autor this[int position]
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
                    row = LayoutInflater.From(sContext).Inflate(Resource.Layout.RecursosLayout, null, false);
                }
                TextView txtName = row.FindViewById<TextView>(Resource.Id.Name);
                TextView txtAutor = row.FindViewById<TextView>(Resource.Id.Autor);
                SearchView sv = row.FindViewById<SearchView>(Resource.Id.sv2);
                //Button btn = row.FindViewById<Button>(Resource.Id.autbtn);
                //Button btn2 = row.FindViewById<Button>(Resource.Id.catbtn);
                Android.Support.V7.Widget.Toolbar toolbar = row.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                txtAutor.Visibility = ViewStates.Gone;
                sv.Visibility = ViewStates.Gone;
                //btn.Visibility = ViewStates.Gone;
                //btn2.Visibility = ViewStates.Gone;
                toolbar.Visibility = ViewStates.Gone;

                txtName.Text = sList[position].nombre;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally { }
            return row;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }


    }

    public class CategoryAdapter : BaseAdapter<Categoria>
    {
        public List<Categoria> sList;
        private Context sContext;
        public CategoryAdapter(Context context, List<Categoria> list)
        {
            sList = list;
            sContext = context;
        }

        public override Categoria this[int position]
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
                    row = LayoutInflater.From(sContext).Inflate(Resource.Layout.RecursosLayout, null, false);
                }
                TextView txtName = row.FindViewById<TextView>(Resource.Id.Name);
                TextView txtAutor = row.FindViewById<TextView>(Resource.Id.Autor);
                SearchView sv = row.FindViewById<SearchView>(Resource.Id.sv2);
                //Button btn = row.FindViewById<Button>(Resource.Id.autbtn);
                //Button btn2 = row.FindViewById<Button>(Resource.Id.catbtn);
                Android.Support.V7.Widget.Toolbar toolbar = row.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                txtAutor.Visibility = ViewStates.Gone;
                sv.Visibility = ViewStates.Gone;
                //btn.Visibility = ViewStates.Gone;
                //btn2.Visibility = ViewStates.Gone;
                toolbar.Visibility = ViewStates.Gone;

                txtName.Text = sList[position].nombre;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally { }
            return row;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }


    }
}

