﻿/*
 * Copyright (C) 2009 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

namespace PhoneNumbers
{
    public class BuildMetadataFromXml
    {
        // String constants used to fetch the XML nodes and attributes.
        private static readonly string CARRIER_CODE_FORMATTING_RULE = "carrierCodeFormattingRule";
        private static readonly string COUNTRY_CODE = "countryCode";
        private static readonly string EMERGENCY = "emergency";
        private static readonly string EXAMPLE_NUMBER = "exampleNumber";
        private static readonly string FIXED_LINE = "fixedLine";
        private static readonly string FORMAT = "format";
        private static readonly string GENERAL_DESC = "generalDesc";
        private static readonly string INTERNATIONAL_PREFIX = "internationalPrefix";
        private static readonly string INTL_FORMAT = "intlFormat";
        private static readonly string LEADING_DIGITS = "leadingDigits";
        private static readonly string LEADING_ZERO_POSSIBLE = "leadingZeroPossible";
        private static readonly string MAIN_COUNTRY_FOR_CODE = "mainCountryForCode";
        private static readonly string MOBILE = "mobile";
        private static readonly string NATIONAL_NUMBER_PATTERN = "nationalNumberPattern";
        private static readonly string NATIONAL_PREFIX = "nationalPrefix";
        private static readonly string NATIONAL_PREFIX_FORMATTING_RULE = "nationalPrefixFormattingRule";
        private static readonly string NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING =
            "nationalPrefixOptionalWhenFormatting";
        private static readonly string NATIONAL_PREFIX_FOR_PARSING = "nationalPrefixForParsing";
        private static readonly string NATIONAL_PREFIX_TRANSFORM_RULE = "nationalPrefixTransformRule";
        private static readonly string NO_INTERNATIONAL_DIALLING = "noInternationalDialling";
        private static readonly string NUMBER_FORMAT = "numberFormat";
        private static readonly string PAGER = "pager";
        private static readonly string PATTERN = "pattern";
        private static readonly string PERSONAL_NUMBER = "personalNumber";
        private static readonly string POSSIBLE_NUMBER_PATTERN = "possibleNumberPattern";
        private static readonly string PREFERRED_EXTN_PREFIX = "preferredExtnPrefix";
        private static readonly string PREFERRED_INTERNATIONAL_PREFIX = "preferredInternationalPrefix";
        private static readonly string PREMIUM_RATE = "premiumRate";
        private static readonly string SHARED_COST = "sharedCost";
        private static readonly string TOLL_FREE = "tollFree";
        private static readonly string UAN = "uan";
        private static readonly string VOICEMAIL = "voicemail";
        private static readonly string VOIP = "voip";

        // Build the PhoneMetadataCollection from the input XML file.
        public static PhoneMetadataCollection BuildPhoneMetadataCollection(Stream input, bool liteBuild)
        {
            var document = new XmlDocument();
            document.Load(input);
            document.Normalize();
            var metadataCollection = new PhoneMetadataCollection.Builder();
            foreach (XmlElement territory in document.GetElementsByTagName("territory"))
            {
                string regionCode = "";
                // For the main metadata file this should always be set, but for other supplementary data
                // files the country calling code may be all that is needed.
                if (territory.HasAttribute("id"))
                     regionCode = territory.GetAttribute("id");
                PhoneMetadata metadata = LoadCountryMetadata(regionCode, territory, liteBuild);
                metadataCollection.AddMetadata(metadata);
            }
            return metadataCollection.Build();
        }

        // Build a mapping from a country calling code to the region codes which denote the country/region
        // represented by that country code. In the case of multiple countries sharing a calling code,
        // such as the NANPA countries, the one indicated with "isMainCountryForCode" in the metadata
        // should be first.
        public static Dictionary<int, List<string>> BuildCountryCodeToRegionCodeMap(
            PhoneMetadataCollection metadataCollection)
        {
            Dictionary<int, List<string>> countryCodeToRegionCodeMap =
                new Dictionary<int, List<string>>();
            foreach (PhoneMetadata metadata in metadataCollection.MetadataList)
            {
                string regionCode = metadata.Id;
                int countryCode = metadata.CountryCode;
                if (countryCodeToRegionCodeMap.ContainsKey(countryCode))
                {
                    if (metadata.MainCountryForCode)
                        countryCodeToRegionCodeMap[countryCode].Insert(0, regionCode);
                    else
                        countryCodeToRegionCodeMap[countryCode].Add(regionCode);
                }
                else
                {
                    // For most countries, there will be only one region code for the country calling code.
                    List<string> listWithRegionCode = new List<string>(1);
                    if(regionCode.Length > 0)
                        listWithRegionCode.Add(regionCode);
                    countryCodeToRegionCodeMap[countryCode] = listWithRegionCode;
                }
            }
            return countryCodeToRegionCodeMap;
        }

        public static string ValidateRE(string regex)
        {
            return ValidateRE(regex, false);
        }

        public static string ValidateRE(string regex, bool removeWhitespace)
        {
            // Removes all the whitespace and newline from the regexp. Not using pattern compile options to
            // make it work across programming languages.
            if (removeWhitespace)
                regex = Regex.Replace(regex, "\\s", "");
            new Regex(regex, RegexOptions.Compiled);
            // return regex itself if it is of correct regex syntax
            // i.e. compile did not fail with a PatternSyntaxException.
            return regex;
        }

        /**
        * Returns the national prefix of the provided country element.
        */
        // @VisibleForTesting
        public static string GetNationalPrefix(XmlElement element)
        {
            return element.HasAttribute(NATIONAL_PREFIX) ? element.GetAttribute(NATIONAL_PREFIX) : "";
        }

        public static PhoneMetadata.Builder LoadTerritoryTagMetadata(string regionCode, XmlElement element,
                                                        string nationalPrefix)
        {
            var metadata = new PhoneMetadata.Builder();
            metadata.SetId(regionCode);
            metadata.SetCountryCode(int.Parse(element.GetAttribute(COUNTRY_CODE)));
            if (element.HasAttribute(LEADING_DIGITS))
                metadata.SetLeadingDigits(ValidateRE(element.GetAttribute(LEADING_DIGITS)));
            metadata.SetInternationalPrefix(ValidateRE(element.GetAttribute(INTERNATIONAL_PREFIX)));
            if (element.HasAttribute(PREFERRED_INTERNATIONAL_PREFIX))
            {
                string preferredInternationalPrefix = element.GetAttribute(PREFERRED_INTERNATIONAL_PREFIX);
                metadata.SetPreferredInternationalPrefix(preferredInternationalPrefix);
            }
            if (element.HasAttribute(NATIONAL_PREFIX_FOR_PARSING))
            {
                metadata.SetNationalPrefixForParsing(
                    ValidateRE(element.GetAttribute(NATIONAL_PREFIX_FOR_PARSING), true));
                if (element.HasAttribute(NATIONAL_PREFIX_TRANSFORM_RULE))
                {
                    metadata.SetNationalPrefixTransformRule(
                    ValidateRE(element.GetAttribute(NATIONAL_PREFIX_TRANSFORM_RULE)));
                }
            }
            if (!string.IsNullOrEmpty(nationalPrefix))
            {
                metadata.SetNationalPrefix(nationalPrefix);
                if (!metadata.HasNationalPrefixForParsing)
                    metadata.SetNationalPrefixForParsing(nationalPrefix);
            }
            if (element.HasAttribute(PREFERRED_EXTN_PREFIX))
            {
                metadata.SetPreferredExtnPrefix(element.GetAttribute(PREFERRED_EXTN_PREFIX));
            }
            if (element.HasAttribute(MAIN_COUNTRY_FOR_CODE))
            {
                metadata.SetMainCountryForCode(true);
            }
            if (element.HasAttribute(LEADING_ZERO_POSSIBLE))
            {
                metadata.SetLeadingZeroPossible(true);
            }
            return metadata;
        }

        /**
        * Extracts the pattern for international format. If there is no intlFormat, default to using the
        * national format. If the intlFormat is set to "NA" the intlFormat should be ignored.
        *
        * @throws  RuntimeException if multiple intlFormats have been encountered.
        * @return  whether an international number format is defined.
        */
        // @VisibleForTesting
        public static bool LoadInternationalFormat(PhoneMetadata.Builder metadata,
            XmlElement numberFormatElement,
            string nationalFormat)
        {
            NumberFormat.Builder intlFormat = new NumberFormat.Builder();
            SetLeadingDigitsPatterns(numberFormatElement, intlFormat);
            intlFormat.SetPattern(numberFormatElement.GetAttribute(PATTERN));
            var intlFormatPattern = numberFormatElement.GetElementsByTagName(INTL_FORMAT);
            bool hasExplicitIntlFormatDefined = false;

            if (intlFormatPattern.Count > 1)
            {
                //LOGGER.log(Level.SEVERE,
                //          "A maximum of one intlFormat pattern for a numberFormat element should be " +
                //           "defined.");
                throw new Exception("Invalid number of intlFormat patterns for country: " +
                                    metadata.Id);
            }
            else if (intlFormatPattern.Count == 0)
            {
                // Default to use the same as the national pattern if none is defined.
                intlFormat.SetFormat(nationalFormat);
            }
            else
            {
                string intlFormatPatternValue = intlFormatPattern[0].InnerText;
                if (!intlFormatPatternValue.Equals("NA"))
                {
                    intlFormat.SetFormat(intlFormatPatternValue);
                }
                hasExplicitIntlFormatDefined = true;
            }

            if (intlFormat.HasFormat)
            {
                metadata.AddIntlNumberFormat(intlFormat);
            }
            return hasExplicitIntlFormatDefined;
        }

        /**
         * Extracts the pattern for the national format.
         *
         * @throws  RuntimeException if multiple or no formats have been encountered.
         * @return  the national format string.
         */
        // @VisibleForTesting
        public static string LoadNationalFormat(PhoneMetadata.Builder metadata, XmlElement numberFormatElement,
                                         NumberFormat.Builder format)
        {
            SetLeadingDigitsPatterns(numberFormatElement, format);
            format.SetPattern(ValidateRE(numberFormatElement.GetAttribute(PATTERN)));

            var formatPattern = numberFormatElement.GetElementsByTagName(FORMAT);
            if (formatPattern.Count != 1)
            {
                //LOGGER.log(Level.SEVERE,
                //           "Only one format pattern for a numberFormat element should be defined.");
                throw new Exception("Invalid number of format patterns for country: " +
                                    metadata.Id);
            }
            string nationalFormat = formatPattern[0].InnerText;
            format.SetFormat(nationalFormat);
            return nationalFormat;
        }

        /**
        *  Extracts the available formats from the provided DOM element. If it does not contain any
        *  nationalPrefixFormattingRule, the one passed-in is retained. The nationalPrefix,
        *  nationalPrefixFormattingRule and nationalPrefixOptionalWhenFormatting values are provided from
        *  the parent (territory) element.
        */
        // @VisibleForTesting
        public static void LoadAvailableFormats(PhoneMetadata.Builder metadata,
                                         XmlElement element, string nationalPrefix,
                                         string nationalPrefixFormattingRule,
                                         bool nationalPrefixOptionalWhenFormatting)
        {
            string carrierCodeFormattingRule = "";
            if (element.HasAttribute(CARRIER_CODE_FORMATTING_RULE))
            {
                carrierCodeFormattingRule = ValidateRE(
                    GetDomesticCarrierCodeFormattingRuleFromElement(element, nationalPrefix));
            }
            var numberFormatElements = element.GetElementsByTagName(NUMBER_FORMAT);
            bool hasExplicitIntlFormatDefined = false;

            int numOfFormatElements = numberFormatElements.Count;
            if (numOfFormatElements > 0)
            {
                foreach (XmlElement numberFormatElement in numberFormatElements)
                {
                    var format = new NumberFormat.Builder();

                    if (numberFormatElement.HasAttribute(NATIONAL_PREFIX_FORMATTING_RULE))
                    {
                        format.SetNationalPrefixFormattingRule(
                            GetNationalPrefixFormattingRuleFromElement(numberFormatElement, nationalPrefix));
                        format.SetNationalPrefixOptionalWhenFormatting(
                            numberFormatElement.HasAttribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING));

                    }
                    else
                    {
                        format.SetNationalPrefixFormattingRule(nationalPrefixFormattingRule);
                        format.SetNationalPrefixOptionalWhenFormatting(nationalPrefixOptionalWhenFormatting);
                    }
                    if (numberFormatElement.HasAttribute("carrierCodeFormattingRule"))
                    {
                        format.SetDomesticCarrierCodeFormattingRule(ValidateRE(
                            GetDomesticCarrierCodeFormattingRuleFromElement(
                                numberFormatElement, nationalPrefix)));
                    }
                    else
                    {
                        format.SetDomesticCarrierCodeFormattingRule(carrierCodeFormattingRule);
                    }

                    // Extract the pattern for the national format.
                    string nationalFormat =
                        LoadNationalFormat(metadata, numberFormatElement, format);
                    metadata.AddNumberFormat(format);

                    if (LoadInternationalFormat(metadata, numberFormatElement, nationalFormat))
                    {
                        hasExplicitIntlFormatDefined = true;
                    }
                }
                // Only a small number of regions need to specify the intlFormats in the xml. For the majority
                // of countries the intlNumberFormat metadata is an exact copy of the national NumberFormat
                // metadata. To minimize the size of the metadata file, we only keep intlNumberFormats that
                // actually differ in some way to the national formats.
                if (!hasExplicitIntlFormatDefined)
                {
                    metadata.ClearIntlNumberFormat();
                }
            }
        }

        public static void SetLeadingDigitsPatterns(XmlElement numberFormatElement, NumberFormat.Builder format)
        {
            foreach (XmlElement e in numberFormatElement.GetElementsByTagName(LEADING_DIGITS))
            {
                format.AddLeadingDigitsPattern(ValidateRE(e.InnerText, true));
            }
        }

        public static string GetNationalPrefixFormattingRuleFromElement(XmlElement element,
            string nationalPrefix)
        {
            string nationalPrefixFormattingRule = element.GetAttribute(NATIONAL_PREFIX_FORMATTING_RULE);
            // Replace $NP with national prefix and $FG with the first group ($1).
            nationalPrefixFormattingRule = ReplaceFirst(nationalPrefixFormattingRule, "$NP", nationalPrefix);
            nationalPrefixFormattingRule = ReplaceFirst(nationalPrefixFormattingRule, "$FG", "${1}");
            return nationalPrefixFormattingRule;
        }

        public static string GetDomesticCarrierCodeFormattingRuleFromElement(XmlElement element,
            string nationalPrefix)
        {
            string carrierCodeFormattingRule = element.GetAttribute(CARRIER_CODE_FORMATTING_RULE);
            // Replace $FG with the first group ($1) and $NP with the national prefix.
            carrierCodeFormattingRule = ReplaceFirst(carrierCodeFormattingRule, "$FG", "${1}");
            carrierCodeFormattingRule = ReplaceFirst(carrierCodeFormattingRule, "$NP", nationalPrefix);
            return carrierCodeFormattingRule;
        }

        // @VisibleForTesting
        public static bool IsValidNumberType(string numberType)
        {
            return numberType.Equals(FIXED_LINE) || numberType.Equals(MOBILE) ||
                 numberType.Equals(GENERAL_DESC);
        }

        /**
        * Processes a phone number description element from the XML file and returns it as a
        * PhoneNumberDesc. If the description element is a fixed line or mobile number, the general
        * description will be used to fill in the whole element if necessary, or any components that are
        * missing. For all other types, the general description will only be used to fill in missing
        * components if the type has a partial definition. For example, if no "tollFree" element exists,
        * we assume there are no toll free numbers for that locale, and return a phone number description
        * with "NA" for both the national and possible number patterns.
        *
        * @param generalDesc  a generic phone number description that will be used to fill in missing
        *                     parts of the description
        * @param countryElement  the XML element representing all the country information
        * @param numberType  the name of the number type, corresponding to the appropriate tag in the XML
        *                    file with information about that type
        * @return  complete description of that phone number type
        */
        public static PhoneNumberDesc ProcessPhoneNumberDescElement(PhoneNumberDesc generalDesc,
            XmlElement countryElement, string numberType, bool liteBuild)
        {
            if (generalDesc == null)
                generalDesc = new PhoneNumberDesc.Builder().Build();
            var phoneNumberDescList = countryElement.GetElementsByTagName(numberType);
            var numberDesc = new PhoneNumberDesc.Builder();
            if (phoneNumberDescList.Count == 0 && !IsValidNumberType(numberType))
            {
                numberDesc.SetNationalNumberPattern("NA");
                numberDesc.SetPossibleNumberPattern("NA");
                return numberDesc.Build();
            }
            numberDesc.MergeFrom(generalDesc);
            if (phoneNumberDescList.Count > 0)
            {
                XmlElement element = (XmlElement)phoneNumberDescList[0];
                var possiblePattern = element.GetElementsByTagName(POSSIBLE_NUMBER_PATTERN);
                if (possiblePattern.Count > 0)
                    numberDesc.SetPossibleNumberPattern(ValidateRE(possiblePattern[0].InnerText, true));

                var validPattern = element.GetElementsByTagName(NATIONAL_NUMBER_PATTERN);
                if (validPattern.Count > 0)
                    numberDesc.SetNationalNumberPattern(ValidateRE(validPattern[0].InnerText, true));

                if (!liteBuild)
                {
                    var exampleNumber = element.GetElementsByTagName(EXAMPLE_NUMBER);
                    if (exampleNumber.Count > 0)
                        numberDesc.SetExampleNumber(exampleNumber[0].InnerText);
                }
            }
            return numberDesc.Build();
        }

        private static string ReplaceFirst(string input, string value, string replacement)
        {
            var p = input.IndexOf(value);
            if (p >= 0)
                input = input.Substring(0, p) + replacement + input.Substring(p + value.Length);
            return input;
        }

        // @VisibleForTesting
        public static void LoadGeneralDesc(PhoneMetadata.Builder metadata, XmlElement element, bool liteBuild)
        {
            var generalDesc = ProcessPhoneNumberDescElement(null, element, GENERAL_DESC, liteBuild);
            metadata.SetGeneralDesc(generalDesc);

            metadata.SetFixedLine(ProcessPhoneNumberDescElement(generalDesc, element, FIXED_LINE, liteBuild));
            metadata.SetMobile(ProcessPhoneNumberDescElement(generalDesc, element, MOBILE, liteBuild));
            metadata.SetTollFree(ProcessPhoneNumberDescElement(generalDesc, element, TOLL_FREE, liteBuild));
            metadata.SetPremiumRate(ProcessPhoneNumberDescElement(generalDesc, element, PREMIUM_RATE, liteBuild));
            metadata.SetSharedCost(ProcessPhoneNumberDescElement(generalDesc, element, SHARED_COST, liteBuild));
            metadata.SetVoip(ProcessPhoneNumberDescElement(generalDesc, element, VOIP, liteBuild));
            metadata.SetPersonalNumber(ProcessPhoneNumberDescElement(generalDesc, element,
                                                                     PERSONAL_NUMBER, liteBuild));
            metadata.SetPager(ProcessPhoneNumberDescElement(generalDesc, element, PAGER, liteBuild));
            metadata.SetUan(ProcessPhoneNumberDescElement(generalDesc, element, UAN, liteBuild));
            metadata.SetVoicemail(ProcessPhoneNumberDescElement(generalDesc, element, VOICEMAIL, liteBuild));
            metadata.SetEmergency(ProcessPhoneNumberDescElement(generalDesc, element, EMERGENCY, liteBuild));
            metadata.SetNoInternationalDialling(ProcessPhoneNumberDescElement(generalDesc, element,
                                                                              NO_INTERNATIONAL_DIALLING, liteBuild));
            metadata.SetSameMobileAndFixedLinePattern(
                metadata.Mobile.NationalNumberPattern.Equals(
                metadata.FixedLine.NationalNumberPattern));
        }

        public static PhoneMetadata LoadCountryMetadata(string regionCode, XmlElement element, bool liteBuild)
        {
            string nationalPrefix = GetNationalPrefix(element);
            PhoneMetadata.Builder metadata =
                LoadTerritoryTagMetadata(regionCode, element, nationalPrefix);
            string nationalPrefixFormattingRule =
                GetNationalPrefixFormattingRuleFromElement(element, nationalPrefix);
            LoadAvailableFormats(metadata, element, nationalPrefix.ToString(),
                                 nationalPrefixFormattingRule.ToString(),
                                 element.HasAttribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING));
            LoadGeneralDesc(metadata, element, liteBuild);
            return metadata.Build();
        }

        public static Dictionary<int, List<string>> GetCountryCodeToRegionCodeMap(string filePrefix)
        {
            var asm = Assembly.GetExecutingAssembly();
            var name = asm.GetManifestResourceNames().Where(n => n.EndsWith(filePrefix)).FirstOrDefault() ?? "missing";
            using (var stream = asm.GetManifestResourceStream(name))
            {
                var collection = BuildPhoneMetadataCollection(stream, false);
                return BuildCountryCodeToRegionCodeMap(collection);
            }
        }
    }
}
