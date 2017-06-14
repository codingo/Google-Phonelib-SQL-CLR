using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using com.google.i18n.phonenumbers;
using libphonenumber;
using Microsoft.SqlServer.Server;
using PhoneNumberUtil = com.google.i18n.phonenumbers.PhoneNumberUtil;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GooglePhoneLibSqlFunction(string input)
    {       
        return new SqlString (string.Empty);
    }

    // IsPossibleNumber - quickly guessing whether a number is a possible phonenumber by using only the length information, much faster than a full validation.
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsPossibleNumber(string input)
    {
        return new SqlString(string.Empty);
    }

    // IsNumberMatch - gets a confidence level on whether two numbers could be the same.
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
