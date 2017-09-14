using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.Native)]
public struct Straight: INullable
{
    private SqlSingle a;
    private SqlSingle b;
    private bool _null;

    public override string ToString()
    {
        return "y = " + a.ToString() + "x + " + b.ToString();
    }
    
    public bool IsNull
    {
        get
        {
            return _null;
        }
    }
    public Straight(SqlSingle _a, SqlSingle _b)
    {
        this.a = _a;
        this.b = _b;
        _null = false;
    }
    public static Straight Null
    {
        get
        {
            Straight h = new Straight();
            h._null = true;
            return h;
        }
    }
    
    public static Straight Parse(SqlString s)
    {
        if (s.IsNull)
           return Null;

        if (!s.Value.StartsWith("(") || !s.Value.EndsWith(")") || !s.Value.Contains(","))
            throw new ArgumentException("Bad argument format - not like (a,b)");

        string s2 = s.Value.Substring(1, s.Value.Length - 2);
        string[] answers = s2.Split(",".ToCharArray());

        if (SqlSingle.Parse(answers[0])==0 && SqlSingle.Parse(answers[1])==0)
            throw new ArgumentException("Bad values - a and b cannot equal 0 at the same time.");
        return new Straight(SqlSingle.Parse(answers[0]), SqlSingle.Parse(answers[1]));
    }
    public SqlSingle GetA()
    {
        return this.a;
    }
    public SqlSingle GetB()
    {
        return this.b;
    }

    public SqlString asString()
    {
        return this.ToString();
    }
   
}