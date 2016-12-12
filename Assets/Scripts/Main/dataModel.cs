using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Main
{
    public class User
    {
        public int Id { get; set; }
        
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }
        public int Maps { get; set; }
        public bool IsUsingSound { get; set; }
        public int ControllerType { get; set; }
        
        public FacebookApp FacebookApp { get; set; }

        public List<FacebookApp> FacebookApps { get; set; }
        public string FUniqueId { get; set; }

        public override string ToString()
        {
            return string.Format("User { Id={0}, Name={1}, Maps={2}, IsUsingSound={3}, ControllerType={4}, FacebookId={5} }", Id, Name, Maps, IsUsingSound, ControllerType, FacebookApp.FacebookId);
        }

        public static User PhpFillData(string properties)
        {
            return new User
            {
                Id = utils.GetIntDataValue(properties, "ID:"),
                Name = utils.GetDataValue(properties, "Name:"),
                Maps = utils.GetIntDataValue(properties, "Maps:"),
                IsUsingSound = utils.GetBoolDataValue(properties, "IsUsingSound:"),
                ControllerType = utils.GetIntDataValue(properties, "ControllerType:"),
                FacebookApp = new FacebookApp
                {
                    FacebookId = utils.GetLongDataValue(properties, "FacebookId:")
                }
            };
        }
    }

    public class FacebookApp
    {
        //[PrimaryKey, AutoIncrement]   // sqLite
        public int Id { get; set; }

        public long FacebookId { get; set; }

        public string Name { get; set; }

        public int ConnectId { get; set; }

        public string BToken { get; set; }
    }
}