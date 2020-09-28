using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
   public  class PolygonModel
    {
        public const double PI = 3.14159265;
        public const double TWOPI = 2 * PI;
        public bool isInPolygonNew(ref List<Domain.Common.Polygon> p, Domain.Common.Polygon t)
        {
            bool flag = false;
            int pLen = p.Count();
            double[] lat = new double[pLen];
            double[] lng = new double[pLen];
            for (int i = 0; i < pLen; i++)
            {
                lat[i] = p[i].Latitude;
                lng[i] = p[i].Longitude;
            }
            flag = coordinate_is_inside_polygon(t.Latitude, t.Longitude, lat, lng);

            return flag;
        }
        public bool coordinate_is_inside_polygon(double latitude, double longitude, double[] lat_array, double[] long_array)
        {
            double angle = 0;
            double point1_lat;
            double point1_long;
            double point2_lat;
            double point2_long;
            int n = lat_array.Length;
            for (int i = 0; i < n; i++)
            {
                point1_lat = lat_array[i] - latitude;
                point1_long = long_array[i] - longitude;
                point2_lat = lat_array[((i + 1) % n)] - latitude;
                // point2_lat = lat_array.get((i + 1) % n) - latitude;
                //you should have paid more attention in high school geometry.
                point2_long = long_array[((i + 1) % n)] - longitude;
                // point2_long = long_array.get((i + 1) % n) - longitude;
                angle += Angle2D(point1_lat, point1_long, point2_lat, point2_long);
            }

            if (Math.Abs(angle) < PI)
                return false;
            else
                return true;
        }

        public double Angle2D(double y1, double x1, double y2, double x2)
        {
            double dtheta, theta1, theta2;

            theta1 = Math.Atan2(y1, x1);
            theta2 = Math.Atan2(y2, x2);
            dtheta = theta2 - theta1;
            while (dtheta > PI)
                dtheta -= TWOPI;
            while (dtheta < -PI)
                dtheta += TWOPI;

            return (dtheta);
        }
    }
}
