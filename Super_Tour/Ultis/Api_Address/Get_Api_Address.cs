﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Ultis.Api_Address
{
    internal class Get_Api_Address
    {
        string link = "city_province.json";
        public static List<District> getDistrict(string city_name)
        {
            string url = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())), @"/Ultis", "City_province.json");
            //Console.WriteLine(url);
            //Console.ReadLine();
            WebRequest request = WebRequest.Create(url);
            WebResponse respone = request.GetResponse();
            using (Stream dataStream = respone.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responeFromServer = reader.ReadToEnd();
                // Console.WriteLine(responeFromServer);
                Country country = JsonConvert.DeserializeObject<Country>(responeFromServer);
                City a = country.city.Where(p => p.codename == city_name).Single();
                return a.districts;
            }
        }
    }
}
