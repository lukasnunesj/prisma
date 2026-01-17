using System.ComponentModel.DataAnnotations;

namespace backend.Application.DTO
{
    public class GoogleLoginDTO
    {
        [Required(AllowEmptyStrings = false)]
        public string IdToken { get; set; } = null!;
    }
}
