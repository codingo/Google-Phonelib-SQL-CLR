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

        //var phoneUtil = PhoneNumberUtil.getInstance();
        //bool isValid = phoneUtil.isValidNumber(swissNumberStr);

        //try
        //{
        //    Phonenumber numberInput = phoneUtil.parse(input, "CH");
        //}
        return new SqlString(string.Empty);
    }

    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleFindNumbers(string input)
    {
        return new SqlString(string.Empty);
    }
}
