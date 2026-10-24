using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleAppWatchesMovement
{
    internal class Program
    {
        const string FILE_WATCHES = "watches.csv";
        const string FILE_MOVEMENTS = "movements.dat";

        const string FILE_OUT = "REFERENCES_WATCHES.xml";

        static void Main(string[] args)
        {
            List<Movement> MOVEMENTS = 
                new List<Movement>();

            XmlDocument xd = new XmlDocument();
            XmlDeclaration xdd = xd.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode root = xd.CreateElement("REFERENCES");

            xd.AppendChild(xdd);
            xd.AppendChild(root);

            using (StreamReader sr = new StreamReader(FILE_MOVEMENTS))
            {
                int DIM_ID = 8;
                int DIM_CALIBER = 30;
                int DIM_COMPLICATIONS = 100;
                int DIM_PR = 10;

                bool is_declaration = true;
                while (!sr.EndOfStream)
                {
                    string string_tmp = sr.ReadLine();

                    if (!is_declaration)
                    {
                        string id_tmp = string_tmp.Substring(0, DIM_ID).Trim();
                        string caliber_tmp = string_tmp.Substring(DIM_ID, DIM_CALIBER).Trim();
                        List<string> complications_tmp = string_tmp.Substring(DIM_ID +  DIM_CALIBER, DIM_COMPLICATIONS).Trim().Split(';').ToList();
                        string power_reserve_tmp = string_tmp.Substring(DIM_ID + DIM_CALIBER + DIM_COMPLICATIONS, DIM_PR).Trim();

                        MOVEMENTS.Add(new Movement(id_tmp, caliber_tmp, complications_tmp, power_reserve_tmp));
                    }

                    is_declaration = false;
                }
            }

            using (StreamReader sr = new StreamReader(FILE_WATCHES))
            {
                bool is_declaration = true;
                while (!sr.EndOfStream)
                {
                    string[] string_tmp = sr.ReadLine().Split(',');

                    if (!is_declaration)
                    {
                        string id_tmp = string_tmp[0].Trim();
                        string brand_tmp = string_tmp[1].Trim();
                        string model_tmp = string_tmp[2].Trim();
                        string reference_tmp = string_tmp[3].Trim();
                        string production_year_tmp = string_tmp[4].Trim();
                        string price_tmp = string_tmp[5].Trim();

                        int j;
                        for (j = 0; j < MOVEMENTS.Count; j++)
                        {
                            Movement movement_tmp = MOVEMENTS[j];
                            if (movement_tmp.Id == id_tmp)
                            {
                                MOVEMENTS[j].Watches.Add(new Watch(brand_tmp, model_tmp, reference_tmp, production_year_tmp, price_tmp));
                            }
                        }
                    }

                    is_declaration = false;
                }
            }

            foreach (Movement movement in MOVEMENTS) 
            {
                XmlNode movement_node = xd.CreateElement("WATCH");
                root.AppendChild(movement_node);

                XmlNode id = xd.CreateElement("ID");
                id.InnerText = movement.Id;
                movement_node.AppendChild(id);

                XmlNode caliber = xd.CreateElement("CALIBER");
                caliber.InnerText = movement.Caliber;
                movement_node.AppendChild(caliber);

                XmlNode power_reserve = xd.CreateElement("POWER_RESERVE");
                power_reserve.InnerText = movement.PowerReserve;
                movement_node.AppendChild(power_reserve);

                XmlNode complications = xd.CreateElement("COMPLICATIONS");
                movement_node.AppendChild(complications);

                foreach (string tmp in movement.Complications) 
                {
                    XmlNode complication_node = xd.CreateElement("COMPLICATION");
                    complication_node.InnerText = tmp;
                    complications.AppendChild(complication_node);
                }

                XmlNode watches = xd.CreateElement("WATCHES");
                movement_node.AppendChild(watches);

                foreach (Watch tmp in movement.Watches)
                {
                    XmlNode watch_node = xd.CreateElement("WATCH");
                    watches.AppendChild(watch_node);

                        XmlNode brand = xd.CreateElement("BRAND");
                        brand.InnerText = tmp.Brand;
                        watch_node.AppendChild(brand);

                        XmlNode model = xd.CreateElement("MODEL");
                        model.InnerText = tmp.Model;
                        watch_node.AppendChild(model);

                        XmlNode reference = xd.CreateElement("REFERENCE");
                        reference.InnerText = tmp.Reference;
                        watch_node.AppendChild(reference);

                        XmlNode py = xd.CreateElement("PRODUCTION_YEAR");
                        py.InnerText = tmp.ProductionYear;
                        watch_node.AppendChild(py);

                        // 10000 = 10.000,00 $
                        XmlNode price = xd.CreateElement("PRICE");
                        price.InnerText = decimal.Parse(tmp.Price)
                            .ToString("N2", new System.Globalization.CultureInfo("it-IT")) + " $";
                        watch_node.AppendChild(price);
                }
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(System.Xml.Linq.XDocument.Parse(xd.OuterXml));
            xd.Save(FILE_OUT);
        }
    }

    public class Watch
    {
        private string _brand;

        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        private string _model;

        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string _reference;

        public string Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        private string _production_year;

        public string ProductionYear
        {
            get { return _production_year; }
            set { _production_year = value; }
        }

        private string _price;

        public string Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public Watch(string brand, string model, string reference, string production_year, string price)
        {
            _brand = brand;
            _model = model;
            _reference = reference;
            _production_year = production_year;
            _price = price;
        }
    }

    public class Movement
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _caliber;

        public string Caliber
        {
            get { return _caliber; }
            set { _caliber = value; }
        }

        private List<string> _complications;

        public List<string> Complications
        {
            get { return _complications; }
            set { _complications = value; }
        }

        private string _poser_reserve;

        public string PowerReserve
        {
            get { return _poser_reserve; }
            set { _poser_reserve = value; }
        }

        private List<Watch> _watches;

        public List<Watch> Watches
        {
            get { return _watches; }
            set { _watches = value; }
        }

        public Movement(string id, string caliber, List<string> complications, string power_reserve)
        {
            _id = id;
            _caliber = caliber;
            _complications = complications;
            _poser_reserve = power_reserve;
            _watches = new List<Watch>();
        }
    }
}

