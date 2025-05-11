using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VisitCountApi.Controllers;
using VisitCountApi.Data;

namespace VisitorCountApi.Tests.Controllers
{
    public class VisitControllerTest
    {
        private readonly VisitController _sut;
        private readonly VisitorCountContext _context;
        private readonly ILogger<VisitController> _loggerFake;
        private readonly ISession _sessionFake;
        private readonly DefaultHttpContext _httpContext;

        public VisitControllerTest()
        {
            // Fake logger
            _loggerFake = A.Fake<ILogger<VisitController>>();

            // In-memory EF Core context
            var options = new DbContextOptionsBuilder<VisitorCountContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new VisitorCountContext(options);

            // Fake session setup
            _sessionFake = A.Fake<ISession>();

            A.CallTo(() => _sessionFake.Get("VisitID")).Returns(null); // Simulate first-time visit

            A.CallTo(() => _sessionFake.Set("VisitID", A<byte[]>.Ignored))
                .Invokes((string key, byte[] value) => { /* simulate Set if needed */ });

            // Setup HttpContext with fake session
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = _sessionFake;

        }

        [Fact]
        public async Task VisitController_VisitCount_AddOrModifyVisitor_ReturnsOk()
        {
            //Arrange


            // Act
            var result = await _sut.VisitCount("fake-upload");

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be("Record Successfully Initialized");

            var visitor = await _context.Visitors.FirstOrDefaultAsync();
            visitor.Should().NotBeNull();
            visitor.VisitCount.Should().Be(1);

        }

    }
}
