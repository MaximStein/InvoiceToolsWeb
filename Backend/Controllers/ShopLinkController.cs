using AutoMapper;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.DataTransferObjects;
using Backend.Extensions;
using Backend.JwtFeatures;
using Backend.Services;
using DotNetAuth.OAuth1a;
using DotNetAuth.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotNetAuth.OAuth1a;
using TinyOAuth1;
using System.Net;
using System.Web;
using Backend.Services.ShopApis;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ShopLinkController : Controller
    {

        private readonly ShopService _shopService;
        private readonly ShopApiProvider _apiProvider;
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly ApplicationUser _currentUser;

        public ShopLinkController(

            IMapper mapper,
            UserService accService,    
            ShopApiProvider apiProvider,
            ShopService shopService,
            IHttpContextAccessor contextAccessor
            )
        {
            _mapper = mapper;
            _apiProvider = apiProvider;
            _shopService = shopService;
            _userService = accService;
            _currentUser = _userService.Get(contextAccessor.GetUserId());
        }


        [HttpGet("import-orders/{shopId}")]
        public async Task<IActionResult> ImportOrders(string shopId)
        {
            var shop = await _shopService.GetShopAsync(_currentUser.Id.ToString(), shopId);
            
            if (shop.OAuthAccess == null)
                return BadRequest();

            var api = _apiProvider.GetShopAssociatedApiService(shop, _currentUser.Id.ToString());

            var start = shop.LastImport == null ? DateTime.Now.AddDays(-90) : shop.LastImport.Time.AddDays(-30);
            var end = DateTime.Now;

            var result = await api.ImportOrdersAsync(start, end);

            shop.LastImport = result;
            await _shopService.UpdateShopAsync(_currentUser.Id.ToString(), shop);

            if (!result.Successful)
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<ShopDto>(shop));
        }


        [HttpGet("auth-url")]
        public async Task<IActionResult> GetOAuthAuthorizationUrl([FromQuery]string shopId, [FromQuery] Enums.ShopApiType shopApiType)
        {
            var api = _apiProvider.GetApiService(shopApiType, _currentUser.Id.ToString());

            return Ok(api.GetAuthorizationUrl(shopId));
        } 


        [HttpPut("process-auth-response")]
        public async Task<IActionResult> ProcessAuthResponse(ShopAuthResponseDto authResponse)
        {
            var api = _apiProvider.GetApiServiceByReferrer(authResponse.Referrer, _currentUser.Id.ToString());

            var shop = await _shopService.GetShopAsync(_currentUser.Id.ToString(), authResponse.State);
            if (!_currentUser.HasShop(authResponse.State))
                return Unauthorized();
            api.Shop = shop;

            if(await api.ProcessAuthorizationResultAsync(authResponse.Code, authResponse.State))
                return Ok(_mapper.Map<ShopDto>(shop));

            return BadRequest();
        }


    }
}
