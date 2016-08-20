using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing;

namespace ImportData
{
    [XmlRoot(ElementName = "forecast", Namespace = "http://weather.yandex.ru/forecast")]
    public class Forecast
    {
        [XmlAttribute(AttributeName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName="fact")]
        public Fact Fact { get; set; }

        [XmlElement(ElementName = "yesterday")]
        public Fact Yesterday { get; set; }

        [XmlElement(ElementName = "day")]
        public Day[] Days { get; set; }
        public Forecast()
        {

        }
    }

    public class Day
    {
        [XmlAttribute(AttributeName = "date")]
        public DateTime date { get; set; }

        [XmlElement(ElementName = "day_part")]
        public DayParts[] dayParts { get; set; }

    }

}

