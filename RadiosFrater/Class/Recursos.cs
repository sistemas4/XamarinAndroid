using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RadiosFrater
{
    public class Recursos
    {
        public List<Predica> posts { get; set; }
    }

    public class Predica
    {
        [JsonProperty(PropertyName = "url")]
        public string url { get; set; }
        [JsonProperty(PropertyName = "urlpost")]
        public string urlpost { get; set; }
        [JsonProperty(PropertyName = "titulo")]
        public string titulo { get; set; }
        [JsonProperty(PropertyName = "fragmento")]
        public string descripcion { get; set; }
        [JsonProperty(PropertyName = "autor")]
        public string autor { get; set; }
        [JsonProperty(PropertyName = "img")]
        public string imgAutor { get; set; }
        [JsonProperty(PropertyName = "category_id")]
        public int idcategoria { get; set; }
        [JsonProperty(PropertyName = "category_name")]
        public string categoria { get; set; }
    }

    public class Predicadores
    {
        [JsonProperty(PropertyName = "posts")]
        public List<Autor> autores { get; set; }
    }

    public class Autor
    {
        [JsonProperty(PropertyName = "name")]
        public string nombre { get; set; }
    }

    public class Categorias
    {
        [JsonProperty(PropertyName = "posts")]
        public List<Categoria> categorias { get; set; }
    }

    public class Categoria
    {
        [JsonProperty(PropertyName = "category")]
        public string nombre { get; set; }
    }

    public class VersiculoList {
        [JsonProperty(PropertyName = "posts")]
        public List<Versiculo> versiculo { get; set; }
    }

    public class Versiculo
    {
        [JsonProperty(PropertyName = "ID")]
        public string id { get; set; }
        [JsonProperty(PropertyName = "post_title")]
        public string cita { get; set; }
        [JsonProperty(PropertyName = "post_content")]
        public string texto { get; set; }
        [JsonProperty(PropertyName = "post_excerpt")]
        public string semana { get; set; }
        [JsonProperty(PropertyName = "guid")]
        public string imagenv { get; set; }
        [JsonProperty(PropertyName = "meta_value")]
        public string fragverso { get; set; }
    }
}
