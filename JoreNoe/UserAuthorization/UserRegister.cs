using System;

namespace JoreNoe.UserAuthorization
{
    public class UserRegister
    {
        public static UserAuthenticationModel UserModel { get; set; }

        public static void Register(UserAuthenticationModel Model)
        {
            //进行Authentication
            if (Model == null)
                throw new ArgumentNullException(nameof(Model));

            //验证
            UserModel = UserModel;
        }
    }
}
