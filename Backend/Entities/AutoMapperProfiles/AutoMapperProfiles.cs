using AutoMapper;
using Backend.Entities.DataTransferObjects;

namespace Backend.Entities.AutoMapperProfiles
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<Shop, ShopDto>()
                .ForMember(shopDto => shopDto.CurrentApiProvider, 
                    shop => shop.MapFrom(shop => 
                        shop.OAuthAccess == null 
                        ? null 
                        : shop.OAuthAccess.ShopApiType.ToString()))                
                .ReverseMap();

            CreateMap<Order, OrderDto>().ReverseMap();

            CreateMap<LineItem, LineItemDto>().ReverseMap();
                
            CreateMap<ApplicationUser, AppUserDto>().ReverseMap();

            CreateMap<UserSettings, UserSettingsDto>().ReverseMap();

        }
    }
}
