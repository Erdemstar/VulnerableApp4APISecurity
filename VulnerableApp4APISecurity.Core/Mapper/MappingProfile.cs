using System;
using AutoMapper;
using VulnerableApp4APISecurity.Core.DTO.Account;
using VulnerableApp4APISecurity.Core.DTO.Card;
using VulnerableApp4APISecurity.Core.DTO.Profile;
using VulnerableApp4APISecurity.Core.Entities.Account;
using VulnerableApp4APISecurity.Core.Entities.Card;
using VulnerableApp4APISecurity.Core.Entities.Profile;

namespace VulnerableApp4APISecurity.Core.Mapping;

	public class MappingProfile: Profile
	{
    public MappingProfile()
    {
        //Account
        CreateMap<AccountEntity, AccountResponse>();

        //Card
        CreateMap<CardCreateRequest, CardEntity>();
        CreateMap<CardEntity, GetCardResponse>();

        //Profile
        CreateMap<ProfileRequest, ProfileEntity>();
        CreateMap<ProfileEntity, ProfileResponse>();
        CreateMap<ProfileRequest, ProfileResponse>();
    }
}

