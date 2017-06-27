﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Snowflake.Utility;

namespace Snowflake.Romfile
{
    //todo: verify filename convention better
    public partial class StructuredFilename : IStructuredFilename
    {
        private static readonly TextInfo textInfo = new CultureInfo("en-US").TextInfo;
        public StructuredFilenameConvention NamingConvention { get; private set; }

        public string RegionCode { get; }

        public string Title { get; }

        public string Year { get; }

        public string OriginalFilename { get; }

        public StructuredFilename(string originalFilename)
        {
            this.OriginalFilename = Path.GetFileName(originalFilename);
            this.RegionCode = this.ParseRegion();
            this.Title = Path.GetFileNameWithoutExtension(this.ParseTitle());
            this.Year = this.ParseYear();
        }

        private string ParseTitle()
        {
            string rawTitle = Regex.Match(this.OriginalFilename, @"(\([^]]*\))*(\[[^]]*\])*([\w\+\~\@\!\#\$\%\^\&\*\;\,\'\""\?\-\.\-\s]+)").Groups[3].Value.Trim();
            //Invert ending articles
            if (!rawTitle.EndsWith(", The", StringComparison.OrdinalIgnoreCase) &&
                !rawTitle.EndsWith(", A", StringComparison.OrdinalIgnoreCase) &&
                !rawTitle.EndsWith(", Die", StringComparison.OrdinalIgnoreCase) &&
                !rawTitle.EndsWith(", De", StringComparison.OrdinalIgnoreCase) &&
                !rawTitle.EndsWith(", La", StringComparison.OrdinalIgnoreCase) &&
                !rawTitle.EndsWith(", Le", StringComparison.OrdinalIgnoreCase) &&
                !rawTitle.EndsWith(", Les", StringComparison.OrdinalIgnoreCase))
                return rawTitle;

            string[] splitString = rawTitle.Split(',');
            string endingArticle = splitString.Last().Trim();
            string withoutArticle = String.Join(",", splitString.Reverse().Skip(1).Reverse());
            return $"{endingArticle} {withoutArticle}";
        }

        private string ParseYear()
        {
            if (this.NamingConvention == StructuredFilenameConvention.NoIntro) return null;
            var tagData = Regex.Matches(this.OriginalFilename,
                @"(\()([^)]+)(\))");
            return (from Match tagMatch in tagData
                let match = tagMatch.Groups[2].Value.Trim()
                where match.Length == 4 && (match.StartsWith("19") || match.StartsWith("20"))
                select match).FirstOrDefault();
        }

        private string ParseRegion()
        {
            var tagData = Regex.Matches(this.OriginalFilename,
               @"(\()([^)]+)(\))");
            var validMatch = (from Match tagMatch in tagData
                             let match = tagMatch.Groups[2].Value
                             from regionCode in (from regionCode in match.Split(',', '-') select regionCode.Trim())
                             where regionCode.Length != 2 || regionCode.ToLower().ToTitleCase() != regionCode
                             let isoRegion = StructuredFilename.ConvertToRegionCode(regionCode.ToUpperInvariant())
                             where isoRegion.regionCode != null
                             select isoRegion).ToList();
            if (!validMatch.Any())
            {
                this.NamingConvention = StructuredFilenameConvention.Unknown;
                return "ZZ";
            }
            this.NamingConvention = validMatch.First().convention;
            return String.Join("-", from regionCode in validMatch select regionCode.regionCode);
        }

        private static (string regionCode, StructuredFilenameConvention convention) ConvertToRegionCode(string unknownRegion)
        {
            if (StructuredFilename.goodToolsLookupTable.ContainsKey(unknownRegion))
            {
                return (StructuredFilename.goodToolsLookupTable[unknownRegion], StructuredFilenameConvention.GoodTools);
            }
            if (StructuredFilename.nointroLookupTable.ContainsKey(unknownRegion))
            {
                return (StructuredFilename.nointroLookupTable[unknownRegion], StructuredFilenameConvention.NoIntro);
            }
            if (StructuredFilename.tosecLookupTable.Contains(unknownRegion))
            {
                return (unknownRegion, StructuredFilenameConvention.TheOldSchoolEmulationCenter);
            }
            return (null, StructuredFilenameConvention.Unknown);
        }

    }
}
