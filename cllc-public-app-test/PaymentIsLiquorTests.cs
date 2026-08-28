using System.Collections.Generic;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.ViewModels;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class PaymentIsLiquorTest
    {
        [Fact]
        public void IsLiquorTrueOneRecord()
        {
            var x = new List<ApplicationTypeCategory?> { ApplicationTypeCategory.Liquor };
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }

        [Fact]
        public void IsLiquorTrueTwoRecord()
        {
            var x = new List<ApplicationTypeCategory?> { ApplicationTypeCategory.Liquor, ApplicationTypeCategory.Liquor };
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }


        [Fact]
        public void IsLiquorFalseNoRecord()
        {
            var x = new List<ApplicationTypeCategory?>();
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.False(result);
        }

        [Fact]
        public void IsLiquorFalseOneRecord()
        {
            var x = new List<ApplicationTypeCategory?> { ApplicationTypeCategory.Cannabis };
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.False(result);
        }


        [Fact]
        public void IsLiquorFalseOneThird()
        {
            var x = new List<ApplicationTypeCategory?>
            {
                ApplicationTypeCategory.Cannabis,
                ApplicationTypeCategory.Liquor,
                ApplicationTypeCategory.Cannabis,
            };
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.False(result);
        }

        [Fact]
        public void IsLiquorTrueHalf()
        {
            var x = new List<ApplicationTypeCategory?> { ApplicationTypeCategory.Liquor, ApplicationTypeCategory.Cannabis };
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }


        [Fact]
        public void IsLiquorTrueTwoThirds()
        {
            var x = new List<ApplicationTypeCategory?>
            {
                ApplicationTypeCategory.Liquor,
                ApplicationTypeCategory.Cannabis,
                ApplicationTypeCategory.Liquor,
            };
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }


    }
}
