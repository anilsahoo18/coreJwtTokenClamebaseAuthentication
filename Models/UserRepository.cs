using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreJwtTokenClamebaseAuthentication.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly List<UserDTO> users = new List<UserDTO>();
        private readonly List<stateList> statelist = new List<stateList>();
        public UserRepository()
        {
            users.Add(new UserDTO
            {
                UserName = "joydipkanjilal",
                Password = "joydip123",
                Role = "manager"
            });
            users.Add(new UserDTO
            {
                UserName = "michaelsanders",
                Password = "michael321",
                Role = "developer"
            });
            users.Add(new UserDTO
            {
                UserName = "stephensmith",
                Password = "stephen123",
                Role = "tester"
            });
            users.Add(new UserDTO
            {
                UserName = "rodpaddock",
                Password = "rod123",
                Role = "admin"
            });
            users.Add(new UserDTO
            {
                UserName = "rexwills",
                Password = "rex321",
                Role = "admin"
            });

            statelist.Add(new stateList { stateId = 1, stateName = "Odisha" });
            statelist.Add(new stateList { stateId = 2, stateName = "Bihar" });
            statelist.Add(new stateList { stateId = 3, stateName = "Gujurat" });
        }

        
        public UserDTO GetUser(Users userMode)
        {

            return users.Where(x => x.UserName == userMode.Name.ToLower() && x.Password == userMode.Password.ToLower()).FirstOrDefault();



        }

        public List<UserDTO> Getalluserdata()
        {
            return users.ToList();
        }
        public List<stateList> GetStateLists()
        {
            return statelist.ToList();
        }


    }
}
