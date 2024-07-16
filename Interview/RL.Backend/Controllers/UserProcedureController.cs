using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserProcedureController : ControllerBase
{
    private readonly ILogger<UserProcedureController> _logger;
    private readonly RLContext _context;

    public UserProcedureController(ILogger<UserProcedureController> logger, RLContext context)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<UserProcedure> Get()
    {
        return _context.UserProcedures;
    }
}