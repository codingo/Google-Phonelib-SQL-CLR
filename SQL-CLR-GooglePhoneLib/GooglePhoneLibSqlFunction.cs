using System.Data.SqlTypes;
using com.google.i18n.phonenumbers;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString GooglePhoneParseSqlString(SqlString input)
    {
        var phoneUtil = PhoneNumberUtil.getInstance();
        var numberProto = phoneUtil.parse(input.ToString(), "AU");

        return numberProto.ToString();
    }


    /// <summary>
    /// IsPossibleNumber - quickly guessing whether a number is a possible phonenumber by using 
    /// only the length information, much faster than a full validation.
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
    public static SqlBoolean GoogleIsValidNumber(string input)
    {

        return true;
        //var rawnumber = "02476 123987";
        //var cc = "GB";

        //var util = PhoneNumberUtil.Instance;


        //b.CountryCode = util.GetCountryCodeForRegion(cc);
        //b.RawInput = rawnumber;

        //var number = b.Build();

        //return util.IsValidNumber(number);
        //return new SqlString(string.Empty);
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
