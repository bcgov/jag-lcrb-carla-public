using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Test.Helpers
{
    /// <summary>
    /// Fluent builder for IDataverseClient mocks in unit tests.
    /// Integration tests should continue using ApiIntegrationTestBase (real TestServer).
    /// </summary>
    public class MockDataverseClientBuilder
    {
        private readonly Mock<IDataverseClient> _mock = new();

        public MockDataverseClientBuilder WithAccount(Guid id, string name)
        {
            var account = new Account { AccountId = id };
            account["name"] = name;
            _mock.Setup(c => c.GetAccountByIdAsync(id.ToString(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(account);
            return this;
        }

        public MockDataverseClientBuilder WithApplication(Guid id, Action<adoxio_application>? configure = null)
        {
            var app = new adoxio_application { adoxio_applicationId = id };
            configure?.Invoke(app);
            _mock.Setup(c => c.GetApplicationByIdAsync(id.ToString(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(app);
            return this;
        }

        public MockDataverseClientBuilder WithAnnotations(Guid objectId, params Annotation[] notes)
        {
            _mock.Setup(c => c.GetAnnotationsByObjectIdAsync(objectId.ToString(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((IList<Annotation>)notes.ToList());
            return this;
        }

        public IDataverseClient Build() => _mock.Object;
        public Mock<IDataverseClient> Mock => _mock;
    }
}
