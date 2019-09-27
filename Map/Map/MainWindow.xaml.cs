using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Map.Converter;
using Map.Model;
using Map.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Map
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NetworkModel model { get; set; }
        public XmlParser parser = new XmlParser("../../Data/data.xml");
        public bool IsDataLoaded { get; set; }
        public GMapOverlay Markers { get; set; }
        public GMapOverlay Routes { get; set; }


        public MainWindow()
        {
            Markers = new GMapOverlay("markers");
            Routes = new GMapOverlay("routes");
            IsDataLoaded = false;
            ReadAndConvert();
            InitializeComponent();
        }

        private void Gmap_Load(object sender, EventArgs e)
        {
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.MinZoom = 1;
            gmap.MaxZoom = 18;
            gmap.Zoom = 12;
            gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            gmap.CanDragMap = true;
            gmap.DragButton = System.Windows.Forms.MouseButtons.Left;
            gmap.Position = new PointLatLng(45.267136, 19.833549);
        }

        public async Task ReadAndConvert()
        {
            Task read = Task.Run(() =>
            {
                ReadXml();
            });

            await read.ContinueWith(x =>
            {
                ConvertUTMToLonLat();
                IsDataLoaded = true;
            });
        }

        public void ReadXml()
        {
            model = parser.DeSerialize<NetworkModel>();
        }

        public void ConvertUTMToLonLat()
        {
            foreach (var substation in model.Substations)
            {
                UTMToLonLatConverter.ToLatLon(substation.X, substation.Y, 34, out double lat, out double lon);
                substation.X = lon;
                substation.Y = lat;
                GMapMarker marker = new GMarkerGoogle(new GMap.NET.PointLatLng(substation.Y, substation.X), GMarkerGoogleType.green);
                marker.ToolTipText = $"Id: {substation.Id}, Name:{substation.Name}";
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                Markers.Markers.Add(marker);
            }

            foreach (var node in model.Nodes)
            {
                UTMToLonLatConverter.ToLatLon(node.X, node.Y, 34, out double lat, out double lon);
                node.X = lon;
                node.Y = lat;
                GMapMarker marker = new GMarkerGoogle(new GMap.NET.PointLatLng(node.Y, node.X), GMarkerGoogleType.blue);
                marker.ToolTipText = $"Id: {node.Id}, Name:{node.Name}";
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                Markers.Markers.Add(marker);
            }

            foreach (var sw in model.Switches)
            {
                UTMToLonLatConverter.ToLatLon(sw.X, sw.Y, 34, out double lat, out double lon);
                sw.X = lon;
                sw.Y = lat;
                GMapMarker marker = new GMarkerGoogle(new GMap.NET.PointLatLng(sw.Y, sw.X), GMarkerGoogleType.red);
                marker.ToolTipText = $"Id: {sw.Id}, Name:{sw.Name}";
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                Markers.Markers.Add(marker);
            }

            foreach (var line in model.Lines)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                foreach (var point in line.Vertices)
                {
                    UTMToLonLatConverter.ToLatLon(point.X, point.Y, 34, out double lat, out double lon);
                    point.X = lon;
                    point.Y = lat;
                    points.Add(new PointLatLng(point.Y, point.X));
                }
                GMapRoute route = new GMapRoute(points, "");
                route.Stroke = new System.Drawing.Pen(System.Drawing.Color.Black, 3);
                Routes.Routes.Add(route);
            }
        }

        private void LoadMarkers_Click(object sender, RoutedEventArgs e)
        {
            if (IsDataLoaded)
            {
                if (gmap.Overlays.Count > 0)
                {
                    gmap.Overlays.Clear();
                }
                gmap.Overlays.Add(Markers);
                gmap.Overlays.Add(Routes);
            }
        }
    }
}
