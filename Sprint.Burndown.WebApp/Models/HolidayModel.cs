using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Sprint.Burndown.WebApp.Abstractions;

namespace Sprint.Burndown.WebApp.Models
{
    [JsonObject]
    public class HolidayModel : IHasIdentifier, IEnumerable<DateTime>
    {
        [JsonIgnore]
        public string Id
        {
            get => Year.ToString();
            set { }
        }

        [JsonProperty("Год/Месяц")]
        public int Year { get; set; }

        [JsonProperty("Январь")]
        public String Month01 { get; set; }

        [JsonProperty("Февраль")]
        public String Month02 { get; set; }

        [JsonProperty("Март")]
        public String Month03 { get; set; }

        [JsonProperty("Апрель")]
        public String Month04 { get; set; }

        [JsonProperty("Май")]
        public String Month05 { get; set; }

        [JsonProperty("Июнь")]
        public String Month06 { get; set; }

        [JsonProperty("Июль")]
        public String Month07 { get; set; }

        [JsonProperty("Август")]
        public String Month08 { get; set; }

        [JsonProperty("Сентябрь")]
        public String Month09 { get; set; }

        [JsonProperty("Октябрь")]
        public String Month10 { get; set; }

        [JsonProperty("Ноябрь")]
        public String Month11 { get; set; }

        [JsonProperty("Декабрь")]
        public String Month12 { get; set; }

        [JsonProperty("Всего рабочих дней")]
        public int TotalWorkDays { get; set; }

        [JsonProperty("Всего праздничных и выходных дней")]
        public int TotalHolidays { get; set; }

        public IEnumerator<DateTime> GetEnumerator()
        {
            var allDates = ConvertToDays(1, Month01)
                .Concat(ConvertToDays(2, Month02))
                .Concat(ConvertToDays(3, Month03))
                .Concat(ConvertToDays(4, Month04))
                .Concat(ConvertToDays(5, Month05))
                .Concat(ConvertToDays(6, Month06))
                .Concat(ConvertToDays(7, Month07))
                .Concat(ConvertToDays(8, Month08))
                .Concat(ConvertToDays(9, Month09))
                .Concat(ConvertToDays(10, Month10))
                .Concat(ConvertToDays(11, Month11))
                .Concat(ConvertToDays(12, Month12));

            return allDates.GetEnumerator();
        }

        private IEnumerable<DateTime> ConvertToDays(int month, string monthDays)
        {
            var days = monthDays.Split(',');

            return days
                .Where(day => !day.Contains("*"))
                .Select(day => day.Replace("+", ""))
                .Where(day => Int32.TryParse(day, out var dayNo))
                .Select(day => new DateTime(Year, month, Int32.Parse(day)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
