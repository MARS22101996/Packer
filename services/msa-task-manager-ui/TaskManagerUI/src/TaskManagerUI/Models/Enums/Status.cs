using System.ComponentModel.DataAnnotations;

namespace TaskManagerUI.Models.Enums
{
    public enum Status
    {
        [Display(Name = "Open")]
        Open = 1,

        [Display(Name = "In progress")]
        InProgress = 2,

        [Display(Name = "Done")]
        Done = 3
    }
}