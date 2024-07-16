using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Commands.Handlers.Procedures;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests
{
    [TestClass]
    public class AddNamesToProceduresTests
    {


        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddNamesToPlanTests_InvalidPlanId_ReturnsBadRequest(int planId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                UserId = 1,
                PlanId = planId,
                ProcedureId = 1,
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddNamesToProcedureTests_InvalidProcedure_ReturnsBadRequest(int procedureId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                UserId = 1,
                PlanId = 1,
                ProcedureId = procedureId,
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }


        [TestMethod]
        [DataRow(-1,-1)]
        [DataRow(0,0)]
        [DataRow(int.MinValue, int.MinValue)]
        public async Task AddNamesToProcedureTests_InvalidPlanId_ProcedureId_ReturnsBadRequest(int planId, int procedureId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                UserId = 1,
                PlanId = planId,
                ProcedureId = procedureId,
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddNameToProcedureTests_InvalidUserId_ReturnsBadRequest(int userId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = userId
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddUserToPlanTests_PlanIdNotFound_ReturnsNotFound(int planId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
            {
                UserId = 1,
                PlanId = planId,
                ProcedureId = 1
                
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId + 1
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddUserToProcedureTests_UserIdNotFound_ReturnsNotFound(int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
            {
                PlanId = 1,
                UserId = userId,
                ProcedureId = 1
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = userId + 1
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = userId + 1,
                ProcedureTitle = "Test Procedure"
            });
            context.Users.Add(new Data.DataModels.User
            {
               UserId = userId + 1,
                Name = "Test User"
            });

           
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(19, 1010, 29)]
        [DataRow(35, 69, 34)]
        public async Task AddUserToPlanTests_AlreadyContainsUser_ReturnsSuccess(int planId, int procedureId, int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToProcedureCommandHandler(context);
            var request = new AddUserToProcedureCommand()
            {
                UserId = userId,
                PlanId = planId,
                ProcedureId = procedureId
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = procedureId,
                ProcedureTitle = "Test Procedure"
            });
            context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
            });

            context.Users.Add(new Data.DataModels.User
            {
                UserId = userId,
                Name = "Test Name"
            });
            context.UserProcedures.Add(new Data.DataModels.UserProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId,
                UserId = userId
            });



            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }
      
    }
}
