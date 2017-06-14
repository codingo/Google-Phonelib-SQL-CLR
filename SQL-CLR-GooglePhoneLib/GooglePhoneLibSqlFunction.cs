using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GooglePhoneLibSqlFunction(string input)
    {

        
        
        return new SqlString (string.Empty);
    }

    public static SqlString IsPossibleNumber()
    {
        return new SqlString(string.Empty);
    }

    public static SqlString IsNumberMatch()
    {
        return new SqlString(string.Empty);
    }

    public static SqlString IsValidNumber()
    {
        return new SqlString(string.Empty);
    }

    public static SqlString FindNumbers()
    {
        return new SqlString(string.Empty);
    }
}
