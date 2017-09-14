using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PolygonNS;
using PointNS;
using System.Collections;
using System.Collections.Generic;

namespace Function
{

    public partial class UserDefinedFunctions
    {
        //wyznacznik
        public static float Determinant(Point _p, Point _a, Point _b)
        {
            return (float)(_a.GetX() * _b.GetY() + _b.GetX() * _p.GetY() + _p.GetX() * _a.GetY() - _p.GetX() * _b.GetY() - _a.GetX() * _p.GetY() - _b.GetX() * _a.GetY());
        }


        public static bool OnTheEdge(Point _p, Point _a, Point _b)
        {
            float det = Determinant(_p, _a, _b);
            if (det != 0) return false;
            else if (_p.GetX() <= Math.Max((Single)_a.GetX(), (Single)_b.GetX()) && _p.GetX() >= Math.Min((Single)_a.GetX(), (Single)_b.GetX()) && _p.GetY() <= Math.Max((Single)_a.GetY(), (Single)_b.GetY()) && _p.GetY() >= Math.Min((Single)_a.GetY(), (Single)_b.GetY())) return true;
            else return false;

        }


        //drugi punkt odcina(potrzebny w algorytmie) - pierwszy to ten który sprawdzamy czy jest w danym obszarze
        public static Point GetSecEdge(IList<Point> _points, Point _p)
        {
            SqlSingle m = SqlSingle.MinValue;
            foreach (Point p in _points)
            {
                if (p.GetX() > m) m = p.GetX();
            }
            return new Point(m + 1, _p.GetY());
        }

        //funkcja sprawdza ile razy prosta miêdzy punktem sprawdzanym, a drugim poza zadanym obszarem przecina krawêdzie obszaru - jesli parzysta liczbe razy to punkt jest poza obszarem, jesli nie parzysta to wewnatrz
        //rozwazone s¹ przypadki szczegolne
        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlBoolean PointInPolygon(Polygon _polygon, Point _p)
        {
            IList<Point> _poly = _polygon.GetPolygon();
            Point _r = GetSecEdge(_poly, _p);
            Point tmp = new Point();
            int _cuts = 0;
            int index;
            int n = _poly.Count;
            int _verts = n;
            for (int i = 0; i < _verts; ++i)
            {
                index = i;
                if (OnTheEdge(_p, _poly[i], _poly[(i + 1) % n])) return new SqlBoolean(true);
                

                if (!OnTheEdge(_poly[index], _p, _r) && !OnTheEdge(_poly[(index + 1) % _verts], _p, _r))
                {
                    if (Math.Sign(Determinant(_poly[index], _p, _r)) != Math.Sign(Determinant(_poly[(index + 1) % _verts], _p, _r)) && Math.Sign(Determinant(_p, _poly[index], _poly[(index + 1) % _verts])) != Math.Sign(Determinant(_r, _poly[index], _poly[(index + 1) % _verts]))) ++_cuts;
                    else _cuts += 0;
                }
                else
                {
                    if (OnTheEdge(_poly[index], _p, _r) && OnTheEdge(_poly[(index + 1) % _verts], _p, _r))
                    {
                        if (Math.Sign(Determinant(_poly[(index - 1 + _verts) % _verts], _p, _r)) != Math.Sign(Determinant(_poly[(index + 2) % _verts], _p, _r)) && Determinant(_poly[(index - 1 + _verts) % _verts], _p, _r) != 0) _cuts += 0;
                        else _cuts++;
                    }
                    else if (OnTheEdge(_poly[(index - 1 + _verts) % _verts], _p, _r) || OnTheEdge(_poly[(index + 2) % _verts], _p, _r)) _cuts += 0;
                    else
                    {
                        if (OnTheEdge(_poly[((index + 1) % _verts)], _p, _r))
                        {
                            tmp = _poly[index];
                            _cuts += 0;
                        }
                        if (OnTheEdge(_poly[index], _p, _r))
                        {
                            if (Math.Sign(Determinant(tmp, _p, _r)) == Math.Sign(Determinant(_poly[(index + 1) % _verts], _p, _r)) && Math.Sign(Determinant(tmp, _p, _r)) != 0) _cuts += 0;
                            else ++_cuts;
                        }

                    }

                }
            }
            if (_cuts % 2 == 0) return new SqlBoolean(false);
            else return new SqlBoolean(true);
        }
    }

}