using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 课程表UWP.Models
{


    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class GeocoderSearchResponse
    {
        /// 查询状态
        public string status { get; set; }

        /// <remarks/>
        public GeocoderSearchResponseResult result { get; set; }
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class GeocoderSearchResponseResult
    {

        /// <remarks/>
        public GeocoderSearchResponseResultLocation location { get; set; }

        /// <remarks/>
        public string formatted_address { get; set; }

        /// <remarks/>
        public string business { get; set; }

        /// <remarks/>
        public GeocoderSearchResponseResultAddressComponent addressComponent { get; set; }

        /// <remarks/>
        public string cityCode { get; set; }
    }


    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class GeocoderSearchResponseResultLocation
    {

        /// <remarks/>
        public decimal lat { get; set; }

        /// <remarks/>
        public decimal lng { get; set; }
    }


    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class GeocoderSearchResponseResultAddressComponent
    {

        /// <remarks/>
        public string streetNumber { get; set; }

        /// <remarks/>
        public string street { get; set; }

        /// <remarks/>
        public string district { get; set; }

        /// <remarks/>
        public string city { get; set; }

        /// <remarks/>
        public string province { get; set; }
    }



}
