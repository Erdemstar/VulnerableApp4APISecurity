using AutoMapper;
using VulnerableApp4APISecurity.Core.DTO.Account;
using VulnerableApp4APISecurity.Core.DTO.Card;
using VulnerableApp4APISecurity.Core.DTO.Profile;
using VulnerableApp4APISecurity.Core.DTO.RefreshToken;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Entities.Card;
using VulnerableApp4APISecurity.Core.Entities.Profile;
using VulnerableApp4APISecurity.Core.Entities.RefreshToken;

namespace VulnerableApp4APISecurity.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Account
        CreateMap<AccountEntity, AccountResponse>().ReverseMap();

        //Card
        CreateMap<CardCreateRequest, CardEntity>().ReverseMap();
        CreateMap<CardEntity, GetCardResponse>().ReverseMap();

        //Profile
        CreateMap<ProfileRequest, ProfileEntity>().ReverseMap();
        CreateMap<ProfileEntity, ProfileResponse>().ReverseMap();
        CreateMap<ProfileRequest, ProfileResponse>().ReverseMap();

        //RefreshToken
        CreateMap<CreateRefreshToken, RefreshTokenEntity>().ReverseMap();
    }
}