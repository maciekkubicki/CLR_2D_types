using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using PointNS;
using System.IO;




namespace PolygonNS
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 8000)]
    public struct Polygon : INullable, Microsoft.SqlServer.Server.IBinarySerialize
    {
        private IList<Point> points;
        private bool m_Null;
        private static bool HasDuplicatedPoints(IList<Point> points)
        {
            for (int i=0; i<points.Count;++i)
                for(int j=0;j<points.Count;++j)
                {
                    if (i != j && Point.areSame(points[i], points[j]))
                        return true;
                }
            return false;
        }
        private static float Determinant(Point _p, Point _a, Point _b)
        {
            return (float)(_a.GetX() * _b.GetY() + _b.GetX() * _p.GetY() + _p.GetX() * _a.GetY() - _p.GetX() * _b.GetY() - _a.GetX() * _p.GetY() - _b.GetX() * _a.GetY());
        }


        //Do przechodzenia po Polygonie w trakcie sprawdzania jego poprawnosci
        private static int Take(int _actual, int _verts)
        {
            if (_actual == _verts - 1)
                return 0;
            else if (_actual > _verts - 1)
                return _actual - (_verts - 1);
            else return ++_actual;
        }
        private static bool cuts(Point a, Point b, Point c, Point d)
        {
            if (Point.areSame(b, c) && Determinant(d, a, b) == 0)
                return true;
            else if (Point.areSame(d, a) && Determinant(c, a, b) == 0)
                return true;
            else if (Point.onSegment(c, a, b) || Point.onSegment(d, a, b))
                return true;
            else if (Determinant(c, a, b) * Determinant(d, a, b) >= 0)
                return false;
            return true;
        }
        private static bool ValidatePolygon(IList<Point> points)
        {
            bool valid = true;
            if (points.Count == 3)
                return valid;
            else
            {
                int verts = points.Count;
                for (int i = 0; i< verts; ++i)
                {
                    int actual = i;
                    int actual2 = Take(actual, verts);
                    for (int j =0; j<verts-1; ++j)
                    {
                        int next1 = Take(actual + j, verts);
                        int next2 = Take(actual2 + j, verts);
                        if (cuts(points[actual], points[actual2], points[next1], points[next2]))
                            return valid = false;

                    }
                }
                return valid;

            }
        }

        public IList<Point> GetPolygon()
        {
            return points;
        }

        [SqlMethod(OnNullCall = false, IsDeterministic = true, IsPrecise = true)]
        public Point GetPoint(int index)
        {
            if ((index >= 0) && (index < points.Count))
            {
                return points[index];
            }
            throw new ArgumentNullException("No such point in polygon");
        }


        public void Read(BinaryReader r)
        {
            if (r == null) throw new ArgumentNullException("reader problem");

            points = new List<Point>();
            string[] answers = r.ReadString().Split(';');

            foreach (string answer in answers)
            {
                if (answer != "")
                    points.Add(Point.Parse(answer));
            }
        }

        public void Write(BinaryWriter w)
        {
            if (w == null) throw new ArgumentNullException("writeer probrlem");
            string st = "";
            foreach (Point p in points)
                st += p.ToString() + ";";
            w.Write(st);
        }



        public override string ToString()
        {
            string polygon = "[";
            int i = 0;
            foreach (Point p in points)
            {
                if (i > 0) polygon += ",";
                polygon += p.ToString();
                ++i;
            }
            polygon += "]";
            return polygon;


        }


        public SqlString asString()
        {
            SqlString polygon = "[";
            int i = 0;
            foreach (Point p in points)
            {
                if (i > 0) polygon += ",";
                polygon += p.ToString();
                ++i;
            }
            polygon += "]";
            return polygon;


        }


        public SqlSingle NumberOfVerts()
        {
            return points.Count;
        }

        public int Len()
        {
            return points.Count;
        }

        public bool IsNull
        {
            get
            {
                return m_Null;
            }
        }

        public static Polygon Null
        {
            get
            {
                Polygon h = new Polygon();
                h.m_Null = true;
                return h;
            }
        }
        public Polygon(bool t)
        {

            this.m_Null = t;
            this.points = new List<Point>();

        }
        public static Polygon Parse(SqlString s)
        {
            if (s.IsNull)
                return new Polygon(true);

            if (!s.Value.Contains(";"))
                throw new ArgumentException("Bad argument format - not like Point1;Point2;...;PointN");

            string st = s.ToString();
            string[] answers = st.Split(';');
            if (answers.Length < 3)
                throw new ArgumentException("Not enough points to polygon - you need at least 3");

            Polygon u = new Polygon(false);
            foreach (string answer in answers)
            {
                u.points.Add(Point.Parse(answer));
            }
            if(Polygon.HasDuplicatedPoints(u.points))
                throw new ArgumentException("There are duplicated poinst - cannot create valid polygon");
            if (!Polygon.ValidatePolygon(u.points))
               throw new ArgumentException("Bad points order - cannot create valid polygon");
            return u;
        }



    }

}
