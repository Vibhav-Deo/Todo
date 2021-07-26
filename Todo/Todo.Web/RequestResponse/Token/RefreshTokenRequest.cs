using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Web.RequestResponse.Token
{
    public class RefreshTokenRequest
    {

        [Required]
        public string RefreshToken { get; set; }
    }
}
