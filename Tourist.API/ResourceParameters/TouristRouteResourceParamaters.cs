using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tourist.API.ResourceParameters
{
    public class TouristRouteResourceParamaters
    {
        public string Keyword { get; set; }
        public string RatingOperator { get; set; } //比較運算的類型，用來儲存運算類型的字串
        public int? RatingValue { get; set; }

        private string _rating;
        public string Rating {
            get { return _rating; }
            set {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    //正則表達式
                    Regex regex = new Regex(@"([A-Za-z0-9\-]+)(\d+)");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        RatingOperator = match.Groups[1].Value;
                        RatingValue = Int32.Parse(match.Groups[2].Value);
                    }

                    _rating = value;
                    //Value 為 Rating內建的變數，負責接收外部的數據
                }

            } }
    }
}
