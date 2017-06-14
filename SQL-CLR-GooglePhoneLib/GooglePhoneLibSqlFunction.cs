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
    /// <summary>
    /// IsPossibleNumber - quickly guessing whether a number is a possible phonenumber by using only the length information, much faster than a full validation.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsPossibleNumber(string input)
    {
        return new SqlString(string.Empty);
    }

    /// <summary>
    /// IsNumberMatch - gets a confidence level on whether two numbers could be the same.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsNumberMatch(string input)
    {
        return new SqlString(string.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleIsValidNumber(string input)
    {
        return new SqlString(string.Empty);
    }

    /// <summary>
    /// FindNumbers - finds numbers in text input
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GoogleFindNumbers(string input)
    {
        return new SqlString(string.Empty);
    }
}
