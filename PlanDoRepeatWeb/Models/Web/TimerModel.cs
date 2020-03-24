using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace PlanDoRepeatWeb.Models.Web
{
    public class TimerModel
    {
        [Required(ErrorMessage = "Не указано название таймера!")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Не задан период!")]
        public int Period { get; set; }
    }
}