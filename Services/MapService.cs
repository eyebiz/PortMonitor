using System.Drawing;
using System.Net.Http;

namespace PortMonitor.Services
{
    public class MapService
    {
        private const int TileSize = 256;

        public async Task<Image> RenderMapAsync(double lat, double lon, int zoom)
        {
            var (cx, cy) = LatLonToTile(lat, lon, zoom);

            Bitmap bmp = new Bitmap(TileSize * 3, TileSize * 3);
            using Graphics g = Graphics.FromImage(bmp);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int tx = cx + dx;
                    int ty = cy + dy;

                    try
                    {
                        Image tile = await DownloadTileAsync(tx, ty, zoom);
                        g.DrawImage(tile, (dx + 1) * TileSize, (dy + 1) * TileSize);
                    }
                    catch
                    {
                        g.FillRectangle(
                            Brushes.LightGray,
                            (dx + 1) * TileSize,
                            (dy + 1) * TileSize,
                            TileSize,
                            TileSize);
                    }
                }
            }

            g.DrawString("© OpenStreetMap contributors",
                new Font("Arial", 10),
                Brushes.Black,
                new PointF(5, bmp.Height - 20));

            int centerX = bmp.Width / 2;
            int centerY = bmp.Height / 2;

            using Pen pen = new Pen(Color.Red, 2);
            g.DrawEllipse(pen, centerX - 6, centerY - 6, 12, 12);

            return bmp;
        }

        public (int x, int y) LatLonToTile(double lat, double lon, int zoom)
        {
            double latRad = lat * Math.PI / 180.0;
            int n = 1 << zoom;

            int x = (int)((lon + 180.0) / 360.0 * n);
            int y = (int)((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n);

            return (x, y);
        }

        public async Task<Image> DownloadTileAsync(int x, int y, int zoom)
        {
            string url = $"https://tile.openstreetmap.de/{zoom}/{x}/{y}.png";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Port Monitor/1.0");

            var bytes = await client.GetByteArrayAsync(url);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }
    }
}