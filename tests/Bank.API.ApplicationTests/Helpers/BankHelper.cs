using AutoFixture;
using AutoFixture.AutoMoq;
using Bank.API.Application.DTO;
using Bank.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.ApplicationTests.Helpers
{
    public static class BankHelper
    { 
        public static BankEntity GetDefaultBankEntity()
        {
            IFixture _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize<DateOnly>(c => c.FromFactory(() => DateOnly.FromDateTime(DateTime.Now)));

            return _fixture.Build<BankEntity>()
                .With(b => b.ContactEmail, "test@example.com")
                .With(b => b.ContactPhone, "+1234567890")
                .With(b => b.PercentageCommissionForBuyingCurrency, 2.5)
                .With(b => b.PercentageCommissionForSellingCurrency, 2.5)
                .Create();
        }
        public static BankDto GetDefaultBankDto()
        {
            IFixture _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _fixture.Customize<DateOnly>(c => c.FromFactory(() => DateOnly.FromDateTime(DateTime.Now)));

            return _fixture.Build<BankDto>()
                .With(b => b.ContactEmail, "test@example.com")
                .With(b => b.ContactPhone, "+1234567890")
                .With(b => b.PercentageCommissionForBuyingCurrency, 2.5)
                .With(b => b.PercentageCommissionForSellingCurrency, 2.5)
                .Create();
        }

        public static Guid GetBankId()
        {
            return Guid.Parse("E2A4A522-8486-46F7-9437-5F5B7E539502");
        }
    }
}
