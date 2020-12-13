using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers.v1
{
  [ApiController]
  [ApiVersion("1.0")]
  [Route("api/v1/[controller]")]
  public class ValuesController : ControllerBase
  {
    #region value
    private readonly IMapper _mapper;
    private readonly DataDbContext _context;

    public ValuesController(IMapper mapper, DataDbContext context)
    {
      this._mapper = mapper;
      this._context = context;
    }

    [Route("get")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Value>>> Get()
    {
      var value = await _context.Values.ToListAsync();

      return Ok(value);
    }
    #endregion
  }
}