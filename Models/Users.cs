using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreJwtTokenClamebaseAuthentication.Models
{
    public class Users
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class UserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }


    public class Outputclass
    {
        public int Status { get; set; }
        public string OutMessage { get; set; }
        public List<UserDTO> userRecord { get; set; }
    }


    public class stateList
    {
        public string stateName { get; set; }

        public int stateId { get; set; }
    }
}
