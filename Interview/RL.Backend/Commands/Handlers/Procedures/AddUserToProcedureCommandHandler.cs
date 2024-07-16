using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Procedures;

public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AddUserToProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

            var plan = await _context.Plans
                .Include(p => p.UserProcedures)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
            var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == request.UserId);
            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);

            if (plan is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
            if (user is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

            //Already has the Used, so just succeed
            if (plan.UserProcedures.Any(p => p.UserId == user.UserId && p.ProcedureId == request.ProcedureId))
            {
                UserProcedure userProcedure = new UserProcedure();
                userProcedure = plan.UserProcedures.Where(p => p.UserId == request.UserId && p.ProcedureId == request.ProcedureId && p.PlanId == request.PlanId).FirstOrDefault();

                var planUsers = plan.UserProcedures;
                plan.UserProcedures.Remove(userProcedure);
                _context.Entry(plan).State = EntityState.Modified;
            }
            else
            {
                plan.UserProcedures.Add(new UserProcedure
                {
                    UserId = user.UserId,
                    ProcedureId = procedure.ProcedureId,
                });
            }

            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception e)
        {
            return ApiResponse<Unit>.Fail(e);
        }
    }
}