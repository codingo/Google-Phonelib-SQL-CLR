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

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsPossibleNumber(string input)
    {
        return new SqlString(string.Empty);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsNumberMatch(string input)
    {
        return new SqlString(string.Empty);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsValidNumber(string input)
    {
        return new SqlString(string.Empty);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleFindNumbers(string input)
    {
        return new SqlString(string.Empty);
    }
}
