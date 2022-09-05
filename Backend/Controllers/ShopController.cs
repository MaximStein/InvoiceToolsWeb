using AutoMapper;
using Backend.DTOs;
using Backend.Entities;
using Backend.Entities.DataTransferObjects;
using Backend.Extensions;
using Backend.JwtFeatures;
using Backend.Services;
using Backend.Services.DTOs;
using Backend.Services.ShopApis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace Backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ShopController : Controller
    {


        private readonly ShopService _shopService;
        private readonly UserService _userService;
        private readonly ShopApiProvider _shopApiProvider;
        private readonly IMapper _mapper;

        private ApplicationUser _currentUser;

        public ShopController(

            IMapper mapper,
            UserService accService,
            ShopService orderService,
            ShopApiProvider shopApiProvider,
            IHttpContextAccessor contextAccessor
            )
        {
            _mapper = mapper;
            _userService = accService;
            _shopService = orderService;
            _currentUser = _userService.Get(contextAccessor.GetUserId());
            _shopApiProvider = shopApiProvider;
        }

        [HttpGet("shops")]
        public async Task<IActionResult> GetShopsAsync()
        {
            //var groups = await _shopService.GetShopsAsync(User.GetId());
            var result = _mapper.Map<IList<Shop>, List<ShopDto>>(_currentUser.Shops);
            result.ForEach(dto =>
            {
                dto.OrdersCount = _shopService.GetOrdersCount(dto.Id);
            });

            return Ok(result);
        }

        [HttpGet("update-label")]
        public async Task<IActionResult> UpdateShopLabel([FromQuery] string shopId, [FromQuery] string label)
        {
            var shop = await _shopService.GetShopAsync(_currentUser.Id.ToString(), shopId);

            if (shop == null) {
                return BadRequest("Shop mit diesem Namen existiert bereits");
            }

            shop.Label = label;
            await _shopService.UpdateShopAsync(_currentUser.Id.ToString(), shop);

            return NoContent();
        }

        [HttpPost]
        [Route("upsert-order")]
        public async Task<IActionResult> UpsertOrder([FromBody] OrderDto newOrder)
        {
            var order = _mapper.Map<OrderDto, Order>(newOrder);

            if (String.IsNullOrEmpty(newOrder.ShopId))
                return BadRequest("Bestellung muss einem Shop zugewiesen sein");

            if (!_currentUser.HasShop(newOrder.ShopId))
                return Forbid();

            order.ShopId = newOrder.ShopId;

            if (!_currentUser.Shops.Any(s => s.Id == order.ShopId))
                return Unauthorized();

            if (newOrder.Id != null)
            {
                await _shopService.UpdateOrderAsync(order);
            }
            else
            {
                order.Id = ObjectId.GenerateNewId().ToString();
                await _shopService.InsertOrderAsync(order);
            }

            return Ok(order);
        }


        [HttpPut("make-invoices")]
        public async Task<IActionResult> MakeOrderInvoices(string[] ids)
        {
            var orders = await _shopService.GetOrdersAsync(ids);

            if (!orders.All(o => _currentUser.HasShop(o.ShopId)))
                return Forbid();

            orders.ForEach(async o =>
            {
                if(o.InvoiceNumber == null)
                {
                    while(_shopService.IsInvoiceNumberExisting(_currentUser.Shops.Select(s => s.Id!), _currentUser.UserSettings.NextInvoiceNumber)) 
                    {
                        _currentUser.UserSettings.NextInvoiceNumber++;
                    }

                    o.InvoiceNumber = _currentUser.UserSettings.NextInvoiceNumber++;
                    o.InvoiceDate = DateTime.Now;

                    await _shopService.UpdateOrderAsync(o);                    
                    await _userService.UpdateAsync(_currentUser);
                }
            });

            return Ok(_mapper.Map<List<Order>, List<OrderDto>>(orders));
        }

       [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] string[] ids)
        {
            var orders = await _shopService.GetOrdersAsync(ids);
                
            if(!orders.All(o => _currentUser.HasShop(o.ShopId)))
                return Forbid();
            
            return Ok(_mapper.Map<List<Order>, List<OrderDto>>(orders));
        }

        [HttpPost("orders")]
        public async Task<IActionResult> GetOrdersPageAsync(OrdersFilterDto filterDto, [FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            if (!_currentUser.HasShops(filterDto.ShopIds))
                return Forbid();

            var filter = new OrdersFilter { ShopIds = filterDto.ShopIds, 
                HasInvoice = filterDto.OnlyWithoutInvoice ? false : null, 
                IsPaid =  filterDto.OnlyPaid ? true : null };
            var orders = await _shopService.GetOrdersPageAsync(filter, pageIndex, pageSize);
            var totalCount = await _shopService.GetOrdersCountAsync(filter);

            var result = _mapper.Map<List<Order>, List<OrderDto>>(orders);

            return Ok(new ListResultDto<OrderDto> { Items = result, TotalItemsCount = totalCount });
        }

        [HttpDelete("orders")]
        public async Task<IActionResult> DeleteOrders(string[] orderIds)
        {
            await _shopService.DeleteOrdersAsync(orderIds);
            return Ok();
        }

        [HttpDelete("clear-orders/{shopId}")]
        public async Task<IActionResult> ClearShopOrders(string shopId)
        {
            await _shopService.DeleteOrdersAsync(new OrdersFilter { ShopIds = new[] { shopId } });
            await _shopService.ClearLastImport(_currentUser.Id.ToString(), shopId);
            return Ok();
        }

        [HttpDelete("{shopId}")]
        public async Task<IActionResult> DeleteShop(string? shopId)
        {
            await _shopService.RemoveShopAsync(_currentUser.Id.ToString(),shopId);
            return NoContent();
        }

        [HttpPost]
        [Route("create-shop")]
        public async Task<IActionResult> CreateShop([FromBody] ShopDto newShop)
        {
            var shopToInsert = _mapper.Map<ShopDto, Shop>(newShop);
            await _shopService.CreateShopAsync(_currentUser.Id.ToString(), shopToInsert);

            return Ok(_mapper.Map<Shop, ShopDto>(shopToInsert));
        }

        [HttpGet("supported-apis")]
        public IActionResult GetSupportedShopApis()
        {
            return Ok(Enum.GetValues<Enums.ShopApiType>().Select(v => v.ToString()));
        }


       
    }
}
