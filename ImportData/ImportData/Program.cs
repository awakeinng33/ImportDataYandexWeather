using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
namespace ImportData
{
    class Program
    {
        private const string URL = "https://export.yandex.ru/weather-ng/forecasts/27612.xml";
        private static Forecast ReadInternet()
        {
            //Из-за профилактик на Яндексе, пришлось скачать базу себе и парсить ее из файла =/


            //Создание объекта дял выполнения десериальзации
            XmlSerializer serializer = new XmlSerializer(typeof(Forecast));
            //Реализация
            using (FileStream reader = new FileStream("27612.xml", FileMode.Open)) //Открытие или создание файла для чтения
            {
                Forecast F = (Forecast)serializer.Deserialize(reader);
                //
                Console.WriteLine("{0} {1} {2} {3} Время: {4}", F.City, F.Fact.Weather, F.Fact.Tempa.color, F.Fact.Tempa.tempa, F.Fact.Time);
                Console.WriteLine("{0} {1} {2} {3} Время: {4}", F.City, F.Yesterday.Weather, F.Yesterday.Tempa.color, F.Yesterday.Tempa.tempa, F.Yesterday.Time);
                //Возврат результата
                return F;
            }
            //Forecast F = (Forecast)serializer.Deserialize(reader);
            //
            //Возврат результата
            //return F;

        }
        private static void WriteData(Forecast F, Fact FF)
        {
            using (SqlConnection c = new SqlConnection(@"Data Source=АЛЕКСАНДР-ПК;Initial Catalog=Kalinin;User=student;Password=student;"))
            {

                c.Close();
                c.Open();
                try
                {
                    SqlCommand cmd = c.CreateCommand();
                    //Parametrichescy zapros
                    cmd.CommandText = "SELECT Time FROM Kalinin WHERE Time = @time";
                    string s = FF.Time.ToString("yyyy-MM-dd HH:mm:ss");
                    cmd.Parameters.AddWithValue("time", s);
                    //vipolnenie scalarnoy komandi
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        //Текст комманды
                        cmd.CommandText = "INSERT INTO Kalinin (City, Weather, Temperature, Color, Time) VALUES ('"
                            + F.City + "', '"
                            + FF.Weather + "', '"
                            + FF.Tempa.tempa + "', '"
                            + FF.Tempa.color + "', '"
                            + s + "')";
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE Kalinin SET Weather = @weather, Temperature = @temperature, Color = @color, City = @city WHERE Time = @time";
                        cmd.Parameters.AddWithValue("weather", FF.Weather);
                        cmd.Parameters.AddWithValue("temperature", FF.Tempa.tempa);
                        cmd.Parameters.AddWithValue("color", FF.Tempa.color);
                        cmd.Parameters.AddWithValue("city", F.City);
                    }
                    //Vipolnenie
                    int n = cmd.ExecuteNonQuery();
                    c.Close();
                }
                catch
                {
                    c.Close();
                }
            }
            
        }
        private static ConsoleColor SearchColor(Color col)
        {
            ConsoleColor[] conColors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            ConsoleColor currentColor = ConsoleColor.White;

            var colR = col.R;
            var colG = col.G;
            var colB = col.B;


            if (colR > colG && colR > colB)
                currentColor = ConsoleColor.Yellow;
            else if (colG > colR && colG > colB)
                currentColor = ConsoleColor.DarkGreen;
            else if (colB > colR && colB > colG)
                currentColor = ConsoleColor.Cyan;
            else if (colB == colR && colB == colG && colR == colG)
                currentColor = ConsoleColor.DarkGray;
            return currentColor;
        }
        private static void WriteAllData(Forecast F)
        {
            using (SqlConnection c = new SqlConnection(@"Data Source=АЛЕКСАНДР-ПК;Initial Catalog=Kalinin;User=student;Password=student;"))
            {
                c.Close();
                c.Open();
                try
                {
                    foreach(Day day in F.Days)
                    {
                        int i = 0;
                        foreach(DayParts dayParts in day.dayParts)
                        {
                            var NewColor = SearchColor(dayParts.temperature.avg.Colorus);


                            i++;
                            SqlCommand cmd = c.CreateCommand();

                            cmd.CommandText = "SELECT COUNT(*) Time FROM Kalinin WHERE Time = @time";
                            DateTime temp = day.date.AddHours(4 * i);
                            string s = temp.ToString("yyyy-MM-dd HH:mm:ss");
                            cmd.Parameters.AddWithValue("time", s);

                            object result = cmd.ExecuteScalar();
                            int R = Convert.ToInt32(result);
                            Console.ForegroundColor = NewColor;
                            Console.WriteLine("{0}|{1,24}|{2,3}|{3,11}|{4}", F.City, dayParts.weatherType, dayParts.temperature.avg.avgValue, dayParts.dateType, s);
                            Console.ForegroundColor = ConsoleColor.White;
                            if (R == 0)
                            {
                                cmd.CommandText = "INSERT INTO Kalinin (City, Weather, Temperature, Color, PartDay, Time) VALUES ('"
                                    + F.City + "', '"
                                    + dayParts.weatherType + "', '"
                                    + dayParts.temperature.avg.avgValue + "', '"
                                    + dayParts.temperature.avg.color + "', '"
                                    + dayParts.dateType + "', '"
                                    + s + "')";
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE Kalinin SET Weather = @weather, Temperature = @temperature, Color = @color, PartDay = @partDay, City = @city WHERE Time = @time";
                                cmd.Parameters.AddWithValue("weather", dayParts.weatherType);
                                cmd.Parameters.AddWithValue("temperature", dayParts.temperature.avg.avgValue);
                                cmd.Parameters.AddWithValue("color", dayParts.temperature.avg.color);
                                cmd.Parameters.AddWithValue("partDay", dayParts.dateType);
                                cmd.Parameters.AddWithValue("city", F.City);
                            }
                            int n = cmd.ExecuteNonQuery();

                        }
                    }
                    c.Close();
                    //Vipolnenie
                }
                catch (Exception ex)
                {
                    c.Close();
                    Console.WriteLine(ex.Message);
                }
            }
            
        }
        // 172.26.30.153
        //https://export.yandex.ru/weather-ng/forecasts/27612.xml
        /// <summary>
        /// Главная функция программы
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                var f = ReadInternet();
                WriteAllData(f);
                WriteData(f, f.Fact);
                WriteData(f, f.Yesterday);
                Console.WriteLine("Привет!");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
