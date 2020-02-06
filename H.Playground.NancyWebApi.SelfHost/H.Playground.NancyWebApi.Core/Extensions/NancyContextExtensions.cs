using H.Playground.NancyWebApi.Core.Auth;
using Nancy;

namespace H.Playground.NancyWebApi.Core.Extensions
{
    public static class NancyContextExtensions
    {
        public const string ItemsKeyCurrentUserInfo = "CurrentUserInfo";

        public static void SetCurrentUserInfo(this NancyContext context, UserInfo userInfo)
        {
            context.Items.Add(ItemsKeyCurrentUserInfo, userInfo);
        }

        public static UserInfo GetCurrentUserInfo(this NancyContext context)
        {
            if (!context?.Items?.ContainsKey(ItemsKeyCurrentUserInfo) ?? true)
                return null;

            return context.Items[ItemsKeyCurrentUserInfo] as UserInfo;
        }
    }
}
