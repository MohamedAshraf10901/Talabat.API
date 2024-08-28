using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Products_Specs;

namespace Talabat.APIs.Controllers
{    
    public class ProductsController : BaseApiController
    {
        private readonly IGenaricRepository<Product> _ProductRepo;
        private readonly IMapper _mapper;
        private readonly IGenaricRepository<ProductBrand> _brandsRepo;
        private readonly IGenaricRepository<ProductType> _categoriesRepo;

        public ProductsController(IGenaricRepository<Product> productRepo,
                                    IMapper mapper,
                                    IGenaricRepository<ProductBrand> brandsRepo,
                                    IGenaricRepository<ProductType> categoriesRepo)
        {
            _ProductRepo = productRepo;
            _mapper = mapper;
            _brandsRepo = brandsRepo;
            _categoriesRepo = categoriesRepo;
        }



        // GetAll

        //api/Products     Get
        //[Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productSpec)
        {
            /*//var products = await _ProductRepo.GetAllAsync();

            //JsonResult result = new JsonResult(products);            

            //OkResult result1 = new OkResult();

            //OkObjectResult result = new OkObjectResult(products);
            //result.ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection();
            //result.StatusCode = 200;

            //var spec = new BaseSpecifications<Product>();*/

            var spec = new ProductWithBrandAndCategorySpecifications(productSpec);

            var products = await _ProductRepo.GetAllWithSpecAsync(spec);

            var result = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithCountSpecifications(productSpec);

            var count = await _ProductRepo.GetCountAsync(countSpec);

            return Ok(new Pagination<ProductToReturnDto>(productSpec.PageIndex,productSpec.PageSize, count, result));

        }




        // GetById

        //api/Products     Get

        //[ProducesResponseType(typeof(ProductToReturnDto),200)]
       // [Authorize]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {
            //var product = await _ProductRepo.GetAsync(id);

            var spec = new ProductWithBrandAndCategorySpecifications(id);

            var product = await _ProductRepo.GetWithSpecAsync(spec);


            if (product is null)
                return NotFound(new ApiResponse(404));

            var result = _mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(result);            

        }




        //[Authorize]
        [HttpGet("brands")] // Get : /api/products/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _brandsRepo.GetAllAsync();

            return Ok(brands);
        }


        //[Authorize]
        [HttpGet("types")] // Get : /api/products/categories
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetCategories()
        {
            var categories = await _categoriesRepo.GetAllAsync();

            return Ok(categories);
        }


    }
}
