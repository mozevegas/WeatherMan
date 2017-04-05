using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static WeatherMan.weatherMap;

namespace WeatherMan
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, what is your name?");
            var userName = Console.ReadLine();
            Console.WriteLine($"{userName}, what is your zip code?");
            var userZip = Console.ReadLine();

            var url = $"http://api.openweathermap.org/data/2.5/weather?zip={userZip},us&units=imperial&id=524901&APPID=bd508481a5bb649d36da6be5710e7f1c";
            var requesting = WebRequest.Create(url);
            var responsing = requesting.GetResponse();
            var rawResponsing = String.Empty;
            using (var reader = new StreamReader(responsing.GetResponseStream()))
            {
                rawResponsing = reader.ReadToEnd();
                Console.WriteLine(rawResponsing);
                //var describer = JsonConvert.DeserializeObject<weatherMap.RootObject>(rawResponsing);
            }
            var describer = JsonConvert.DeserializeObject<RootObject>(rawResponsing);
            Console.WriteLine($"{userName}");
            //Console.WriteLine(describer.main.temp);
            Console.WriteLine($"The temperature is: {(describer.main.temp)}");
            Console.WriteLine($"The current conditions: {(describer.weather[0].description)}");


            // Insert into SQL

            const string connectionString =
                    @"Server=localhost\SQLEXPRESS;Database=WeatherMan;Trusted_Connection=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                Console.WriteLine("Connected!");
                var sqlCommand = new SqlCommand();
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = System.Data.CommandType.Text;

                Console.WriteLine("connected");

                var text = @"INSERT INTO WeatherBase (UserName, CurrentConditions, Temperature)" +
                "Values (@UserName, @CurrentConditions, @Temperature)";
                var cmd = new SqlCommand(text, connection);
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@CurrentConditions", (describer.weather[0].description));
                cmd.Parameters.AddWithValue("@Temperature", (describer.main.temp));
                connection.Open();
                cmd.ExecuteNonQuery();

                connection.Close();
            }

            // INSERT COMMANDS

            //connection.Close();



        }
    }
}
