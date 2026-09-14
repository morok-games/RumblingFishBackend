using RumblingFishBackend.Models.DTO.Profile;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RumblingFishBackend.Tests
{
    public class SetNicknameRequestTests
    {
        private static bool IsNicknameValid(string nickname)
        {
            var parameter = typeof(SetNicknameRequest)
                .GetConstructors()[0]
                .GetParameters()
                .Single(p => p.Name == nameof(SetNicknameRequest.Nickname));

            var attribute = parameter.GetCustomAttribute<StringLengthAttribute>()!;

            return attribute.IsValid(nickname);
        }

        [Fact]
        public void Nickname_TooShort_IsInvalid() => Assert.False(IsNicknameValid("Bo"));

        [Fact]
        public void Nickname_MinimumLength_IsValid() => Assert.True(IsNicknameValid("Boo"));

        [Fact]
        public void Nickname_MaximumLength_IsValid() => Assert.True(IsNicknameValid("Bo1234567890"));

        [Fact]
        public void Nickname_TooLong_IsInvalid() => Assert.False(IsNicknameValid("Boo1234567890"));
    }
}
