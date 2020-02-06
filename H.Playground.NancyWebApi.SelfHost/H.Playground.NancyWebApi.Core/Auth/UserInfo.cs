using System;
using System.Linq;
using System.Security.Claims;

namespace H.Playground.NancyWebApi.Core.Auth
{
    public class UserInfo
    {
        public UserInfo(UserInfoDto dto)
        {
            UserID = dto.UserID;
            Nickname = dto.Nickname;
            FullName = dto.FullName;
            AvatarImageUrl = dto.AvatarImageUrl;
            LastChangedAt = dto.LastChangedAt;
            Email = dto.Email;
            IsEmailVerified = dto.IsEmailVerified;
        }

        public string UserID { get; }
        public string Nickname { get; }
        public string FullName { get; }
        public string AvatarImageUrl { get; }
        public DateTime LastChangedAt { get; }
        public string Email { get; }
        public bool IsEmailVerified { get; }

        public Claim[] ToClaims()
        {
            return
                new Claim[] {
                    string.IsNullOrEmpty(UserID) ? null : new Claim(nameof(UserID), UserID),
                    string.IsNullOrEmpty(Nickname) ? null : new Claim(nameof(Nickname), Nickname),
                    string.IsNullOrEmpty(FullName) ? null : new Claim(nameof(FullName), FullName),
                    string.IsNullOrEmpty(AvatarImageUrl) ? null : new Claim(nameof(AvatarImageUrl), AvatarImageUrl),
                    string.IsNullOrEmpty(Email) ? null : new Claim(nameof(Email), Email),
                    !IsEmailVerified ? null : new Claim(nameof(IsEmailVerified), IsEmailVerified.ToString()),
                }
                .Where(x => x != null)
                .ToArray();
        }
    }

    public class UserInfoDto
    {
        public string UserID { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string AvatarImageUrl { get; set; }
        public DateTime LastChangedAt { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
