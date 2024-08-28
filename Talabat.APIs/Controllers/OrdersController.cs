using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [ProducesResponseType(typeof(OrderToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]  // POST  : /api/Orders
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto model)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDto, Address>(model.shipToAddress);
            var order = await _orderService.CreateOrderAsync(buyerEmail,model.BasketId,model.DeliveryMethodId, address);
            if (order is null) return BadRequest(new ApiResponse(400, "There is a Problem with Your Order!."));

            var result = _mapper.Map<Core.Entities.Order.Order, OrderToReturnDto>(order);

            return Ok(result);
            
        }



        [ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]  // GET  : /api/Orders
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _orderService.GetOrdersForSpecificUserAsync(buyerEmail);            
            if (orders is null) return NotFound(new ApiResponse(404, "There is No Order for User"));

            var result = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);

            return Ok(result);

        }



        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]  // GET  : /api/Orders/1
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var order = await _orderService.GetOrderByIdForSpecificUserAsync(buyerEmail,id);

            if (order is null) return NotFound(new ApiResponse(404,$"There is not Order with id : {id} for You"));

            var result = _mapper.Map<OrderToReturnDto>(order);

            return Ok(result);
        }



        [HttpGet("DeliveryMethods")]   // GET  : /api/Orders/DeliveryMethods
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            if (deliveryMethods is null) return NotFound(new ApiResponse(404,"There are not Any Delivery Methods."));

            return Ok(deliveryMethods);
        }




    }
}
