using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PointNS;

namespace SegmentNS
{

    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
    public struct Segment : INullable
    {
        private Point start;
        private Point end;

        private bool _null;


        public override string ToString()
        {
            return "[" + this.start.ToString() + "->" + this.end.ToString() + "]";
        }

        public bool IsNull
        {
            get
            {
                return _null;
            }
        }

        public static Segment Null
        {
            get
            {
                Segment h = new Segment(true);
                return h;
            }
        }

        public Segment(bool t)
        {
            this._null = t;
            this.start = new Point(true);
            this.end = new Point(true);

        }
        public SqlString asString()
        {
            return this.ToString();
        }

        public static Segment Parse(SqlString s)
        {
            if (s.IsNull)
                return Null;
            if (!s.Value.Contains(";"))
                throw new ArgumentException("Bad argument format - not like StartPoint;EndPoint");

            string st = s.ToString();
            string[] answers = st.Split(';');
            if (answers.Length > 2)
                throw new ArgumentException("Too much points for segment - you need 2");

            Segment u = new Segment(false);
            u.start = Point.Parse(answers[0]);
            u.end = Point.Parse(answers[1]);
            if(Point.areSame(u.start,u.end))
                throw new ArgumentException("Points are the same - cannot valid segment");
            return u;
        }

    }
}