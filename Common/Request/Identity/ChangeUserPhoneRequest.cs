using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Request.Identity;

public class ChangeUserPhoneRequest
{
    public string UserId { get; init; }
    public string PhoneNumber { get; init; }
}