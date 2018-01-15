using System;
using System.Collections.Generic;
using Plugin.SecureStorage;
using RadiosFrater.Services;

namespace RadiosFrater
{
    public class ItemPlay
    {
        public static Predica shareobj;
        public static Versiculo versoactual;
        public static List<Predica> recursos;
        public static List<Categoria> categorias;
        public static List<Autor> authors;
        public static List<Versiculo> versos;
        public static List<string> horas;

        public static List<string> gethoras(){
            List<string> r = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                var hora = CrossSecureStorage.Current.GetValue("Hora" + i.ToString());
                if (hora == null)
                    hora = "00:00 --";
                r.Add(hora);
            }
            return r;
        }

    }
}
