using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreJwtTokenClamebaseAuthentication.Models
{
  public interface IUserRepository
    {
        UserDTO GetUser(Users userMode);

        List<UserDTO> Getalluserdata();
    }
}
