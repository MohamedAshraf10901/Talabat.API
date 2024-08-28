using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Repository.Repositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository , IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }


        [HttpGet] // GET : api/basket?id=1
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            if(basket is null) return Ok(new CustomerBasket() { Id = id });

            return Ok(basket);
        }



        [HttpPost] // POST : api/basket
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasket>(basket);


            var createdOrUpdatedbasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

            if (createdOrUpdatedbasket is null) return BadRequest(new ApiResponse(400));

            return Ok(createdOrUpdatedbasket);
        }




        [HttpDelete] // Delete : api/basket?id=1
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }


    }
}
