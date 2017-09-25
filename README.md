# Google-Phonelib-SQL-CLR
[![Twitter](https://img.shields.io/badge/twitter-@codingo__-blue.svg)](https://twitter.com/codingo_)
This is a SQL CLR port of libphonenumber, originally from: http://code.google.com/p/libphonenumber/. Original Java code is Copyright (C) 2009-2011 Google Inc. Depends on VS2015 port of the library, available here: https://github.com/codingo/googlephonelibcsharp-vs2015

## Example Usage

```SELECT dbo.GooglePhoneLibSqlFunction('test')
SELECT dbo.GoogleFindNumbers('test')
SELECT dbo.GoogleIsNumberMatch('test')
SELECT dbo.GoogleIsValidNumber('test')
SELECT dbo.GoogleIsPossibleNumber('test')```
