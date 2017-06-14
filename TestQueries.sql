SELECT dbo.GooglePhoneLibSqlFunction('test')
SELECT dbo.GoogleFindNumbers('test')
SELECT dbo.GoogleIsNumberMatch('test')
SELECT dbo.GoogleIsValidNumber('test')
SELECT dbo.GoogleIsPossibleNumber('test')


--Parsing/formatting/validating phone numbers for all countries/regions of the world.
--GetNumberType - gets the type of the number based on the number itself; able to distinguish Fixed-line, Mobile, Toll-free, Premium Rate, Shared Cost, VoIP and Personal Numbers (whenever feasible).
--IsNumberMatch - gets a confidence level on whether two numbers could be the same.
--GetExampleNumber/GetExampleNumberByType - provides valid example numbers for 218 countries/regions, with the option of specifying which type of example phone number is needed.
--IsPossibleNumber - quickly guessing whether a number is a possible phonenumber by using only the length information, much faster than a full validation.
--AsYouTypeFormatter - formats phone numbers on-the-fly when users enter each digit.
--FindNumbers - finds numbers in text input