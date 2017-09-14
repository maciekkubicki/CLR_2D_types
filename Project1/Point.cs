using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
namespace PointNS
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
    public struct Point : INullable
    {
        private SqlSingle x;
        private SqlSingle y;
        private bool m_Null;

        public static bool areSame(Point p1, Point p2)
        {
            if (p1.GetX() == p2.GetX() && p1.GetY() == p2.GetY())
                return true;
            else return false;
        }

        public static bool onSegment(Point p, Point a, Point b)
        {
            if (Determinant(p, a, b) != 0)
                return false;
            if (Math.Min((Single)a.GetX(), (Single)b.GetX()) < p.GetX() && Math.Max((Single)a.GetX(), (Single)b.GetX()) > p.GetX() && Math.Min((Single)a.GetY(), (Single)b.GetY()) < p.GetY() && Math.Max((Single)a.GetY(), (Single)b.GetY()) > p.GetY())
                return true;
            return false;
        }

        public override string ToString()
        {

            return "(" + x.ToString() + "," + y.ToString() + ")";
        }

        public bool IsNull
        {
            get
            {

                return m_Null;
            }
        }

        public static Point Null
        {
            get
            {
                Point h = new Point();
                h.m_Null = true;
                return h;
            }
        }


        public Point(bool isNull)
        {
            this.x = 0;
            this.y = 0;
            this.m_Null = isNull;
        }

        public Point(SqlSingle _x, SqlSingle _y)
        {
            this.x = _x;
            this.y = _y;
            m_Null = false;
        }


        //format (x,y)
        public static Point Parse(SqlString s)
        {
            if (s.IsNull)
                return new Point(true);
       
            if (!s.Value.StartsWith("(") || !s.Value.EndsWith(")") || !s.Value.Contains(","))
                throw new ArgumentException("Bad argument format - not like (x,y)");

            string s2 = s.Value.Substring(1, s.Value.Length - 2);
            string[] answers = s2.Split(",".ToCharArray());

            return new Point(SqlSingle.Parse(answers[0]), SqlSingle.Parse(answers[1]));


        }



        [SqlFunction(Name = "MakePoint")]
        public static Point Create(SqlSingle _x, SqlSingle _y)
        {
            return new Point(_x, _y);
        }

        public SqlSingle GetX()
        {
            return this.x;
        }
        public SqlSingle GetY()
        {
            return this.y;
        }

        public SqlString asString()
        {
            return this.ToString();
        }
        public static float Determinant(Point _p, Point _a, Point _b)
        {
            return (float)(_a.GetX() * _b.GetY() + _b.GetX() * _p.GetY() + _p.GetX() * _a.GetY() - _p.GetX() * _b.GetY() - _a.GetX() * _p.GetY() - _b.GetX() * _a.GetY());
        }





    }


}