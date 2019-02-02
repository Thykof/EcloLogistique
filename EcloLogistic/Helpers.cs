using System;
using System.IO;

using Newtonsoft.Json; // NuGet package

namespace EcloLogistic
{
    /// <summary>
    /// This static class define some fonctions.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Use to sort the TODO ListView
        /// </summary>
        /// <param name="s_x">The first <c>string</c></param>
        /// <param name="s_y">The second <c>string</c></param>
        /// <returns></returns>
        public static int Compare(string s_x, string s_y)
        {
            int x;
            int y;
            int result;

            if (s_x == "Today")
                s_x = "-1";

            if (s_y == "Today")
                s_y = "-1";

            if (s_x == "Whenever")
                s_x = Int32.MaxValue.ToString();

            if (s_y == "Whenever")
                s_y = Int32.MaxValue.ToString();

            x = Int32.Parse(s_x);
            y = Int32.Parse(s_y);

            result = ((x < y) ? -1 : 1);
            return result;
        }

        /// <summary>
        /// Check if the Collect form is valide.
        /// </summary>
        /// <param name="s_ProductivityWeight"></param>
        /// <param name="s_ProductivityUnit"></param>
        /// <param name="ProductivityWeight"></param>
        /// <param name="ProductivityUnit"></param>
        /// <returns></returns>
        public static bool ValidateCollectForm(string s_ProductivityWeight, string s_ProductivityUnit, out double ProductivityWeight, out int ProductivityUnit)
        {
            bool ProductivityUnit_is_valide = false;
            string productivity_unit = s_ProductivityUnit.Replace(".", ",");
            int result_productivity_unit = 0;
            if (productivity_unit != "")
            {
                ProductivityUnit_is_valide = Int32.TryParse(productivity_unit, out result_productivity_unit);
            }
            ProductivityUnit = result_productivity_unit;

            bool ProductivityWeight_is_valide = false;
            string productivity_weight = s_ProductivityWeight.Replace(".", ",");
            double result_productivity_weight = 0;
            if (productivity_weight != "")
            {
                ProductivityWeight_is_valide = double.TryParse(productivity_weight, out result_productivity_weight);
            }
            ProductivityWeight = result_productivity_weight;

            if (ProductivityUnit_is_valide && ProductivityWeight_is_valide)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Save the process file server into the config file: <c>elologistic.json</c>.
        /// </summary>
        /// <remarks>
        /// http://www.newtonsoft.com/json/help/html/SerializingJSON.htm
        /// </remarks>
        /// <param name="server_lovation">string</param>
        public static void SaveServerLocation(string server_lovation)
        {
            string user_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string app_conf_path = user_path + @"\EcloLogistic";
            if (!Directory.Exists(app_conf_path))
            {
                Directory.CreateDirectory(app_conf_path);
            }
            string file_conf = app_conf_path + @"\eclologistic.json";
            
            Conf conf = new Conf() { ServerLocation = server_lovation };

            // Serialize the Conf to json:
            string json = JsonConvert.SerializeObject(conf);

            //write string to file:
            System.IO.File.WriteAllText(file_conf, json);

        }

        /// <summary>
        /// Read the configuration file: <c>eclologistic.json</c>.
        /// If the Json file is corrupted
        /// </summary>
        /// <remarks>The configuration is in <c>c:/users/UserName/AppData/Roaming/EcloLogisitc</c>.</remarks>
        /// <returns></returns>
        public static string GetServerLocation()
        {
            string user_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string app_conf_path = user_path + @"\EcloLogistic";
            string file_conf = app_conf_path + @"\eclologistic.json";
            if (System.IO.File.Exists(file_conf))
            {
                Conf deserializedProduct = new Conf();
                string content = System.IO.File.ReadAllText(file_conf);
                try
                {
                    deserializedProduct = JsonConvert.DeserializeObject<Conf>(content);
                }
                catch (JsonReaderException){}
                return deserializedProduct.ServerLocation;
            }
            else
            {
                // Return a default server locaiton:
                return @"c:\MongoDB\Server\3.4.5\bin"; // @"" prevents need for backslashes
            }
        }

    }
    /// <summary>
    /// Define the structure of the json configuration file.
    /// </summary>
    public class Conf
    {
        public string ServerLocation { get; set; }
        // TODO: public string DatabaseLocation { get; set; } (save database location on the disk)
        // Other configuration strings...
    }
}
