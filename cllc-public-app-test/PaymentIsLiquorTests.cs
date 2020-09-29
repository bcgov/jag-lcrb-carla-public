using System.Collections.Generic;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Xunit;

namespace Gov.Lclb.Cllb.Public.Test
{
    public class PaymentIsLiquorTest 
    {
		[Fact]
        public void IsLiquorTrueOneRecord()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory =(int?) ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }

        [Fact]
        public void IsLiquorTrueTwoRecord()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }


        [Fact]
        public void IsLiquorFalseNoRecord()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.False(result);
        }

        [Fact]
        public void IsLiquorFalseOneRecord()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Cannabis
                    }
                }
            );
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.False(result);
        }


        [Fact]
        public void IsLiquorFalseOneThird()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Cannabis
                    }
                }
            );
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Cannabis
                    }
                }
            );
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.False(result);
        }

        [Fact]
        public void IsLiquorTrueHalf()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Cannabis
                    }
                }
            );
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }


        [Fact]
        public void IsLiquorTrueTwoThirds()
        {
            // setup a scenario where liquor is true.
            var x = new List<MicrosoftDynamicsCRMadoxioLicences>();
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Cannabis
                    }
                }
            );
            x.Add(new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioLicenceType = new MicrosoftDynamicsCRMadoxioLicencetype()
                    {
                        AdoxioCategory = (int?)ViewModels.ApplicationTypeCategory.Liquor
                    }
                }
            );
            bool result = DynamicsExtensions.IsMostlyLiquor(x);

            Assert.True(result);
        }


    }
}
