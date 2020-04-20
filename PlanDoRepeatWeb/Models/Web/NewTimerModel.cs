using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PlanDoRepeatWeb.Models.Web
{
    public class NewTimerModel
    {
        [Required(ErrorMessage = "Не указано название таймера!")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Не задан период!")]
        [JsonProperty("period")]
        public int Period { get; set; }
    }
}