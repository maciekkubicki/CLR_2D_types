using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using PointNS;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString PointsDistance(Point _p1, Point _p2)
    {
        double v = Math.Sqrt(Math.Pow((float)_p2.GetX() - (float)_p1.GetX(), 2) + Math.Pow((float)_p2.GetY() - (float)_p1.GetY(), 2));
        return new SqlString(v.ToString());
    }
}
