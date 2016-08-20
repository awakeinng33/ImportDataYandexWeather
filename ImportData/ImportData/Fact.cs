using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing;
namespace ImportData
{
    public class Fact
    {
        [XmlElement(ElementName = "weather_type")]
        public string Weather { get; set; }

        [XmlElement(ElementName = "temperature")]
        public Temperature Tempa { get; set; }
        [XmlElement(ElementName = "observation_time")]
        public DateTime Time { get; set; }
    }
    public class Temperature
    {
        [XmlIgnore]
        public Color Colorus { get; set; }
        [XmlAttribute(AttributeName = "color")]
        public string color
        {
            get
            {
                return ColorTranslator.ToHtml(Colorus);
            }
            set
            {
                Colorus = ColorTranslator.FromHtml("#" + value);
            }
        }
        [XmlText]
        public string tempa { get; set; }
    }
    public class DayParts
    {
        [XmlAttribute(AttributeName = "type")]
        public string dateType { get; set; }
        [XmlElement(ElementName = "temperature-data")]
        public Avg temperature { get; set; }
        [XmlElement(ElementName = "weather_type")]
        public string weatherType { get; set; }
    }
    public class Avg
    {
        [XmlElement(ElementName = "avg")]
        public ColorTemperature avg { get; set; }
    }
    public class ColorTemperature
    {
        [XmlText]
        public string avgValue { get; set; }
        [XmlIgnore]
        public Color Colorus { get; set; }
        [XmlAttribute(AttributeName = "bgcolor")]
        public string color
        {
            get
            {
                return ColorTranslator.ToHtml(Colorus);
            }
            set
            {
                Colorus = ColorTranslator.FromHtml("#" + value);
            }
        }
    }
}
