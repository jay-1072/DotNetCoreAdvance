using DotNetCoreAdvance.Enums;

namespace DotNetCoreAdvance.Models
{
    public class EditUserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public List<string>? Skills { get; set; }
    }
}
