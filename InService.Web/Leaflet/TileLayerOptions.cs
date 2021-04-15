using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Web.Leaflet
{
    public class TileLayerOptions : GridLayerOptions
    {
        public const string OSMURL = "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
        public int? minZoom { get; set; }
        public int? maxZoom { get; set; }
        public dynamic subdomains { get; set; }
        public string errorTileUrl { get; set; }
        public int? zoomOffset { get; set; }
        public bool? tms { get; set; }
        public bool? zoomReverse { get; set; }
        public bool? detectRetina { get; set; }
        public bool? crossOrigin { get; set; }

        public static TileLayerOptions OSMTileLayer = new TileLayerOptions()
        {
            maxZoom = 18,
            attribution = "Map data &copy; <a href='http://openstreetmap.org'>OpenStreetMap</a>",
            id = "mapbox.streets"
        };
    }
}
