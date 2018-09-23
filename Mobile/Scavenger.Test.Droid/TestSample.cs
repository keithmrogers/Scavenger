using System;
using System.Threading;
using Android.Content;
using NUnit.Framework;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.Services;
using Scavenger.Data;
using Android.Locations;
using Scavenger.XForms;

namespace Scavenger.Test.Droid
{
    [TestFixture]
    public class TestsSample
    {
        ILocationService locationService;
        IScavengerService scavengerService;

        private int count = 0;

        [SetUp]
        public void Setup()
        {
            locationService = new LocationService();
            locationService.LocationChanged += LocationService_LocationChanged;
            scavengerService = new ScavengerService();
        }

        private void LocationService_LocationChanged(object sender, LocationEventArgs e)
        {
            count++;
        }

        [TearDown]
        public void Tear() { }

        [Test]
        public void Pass()
        {
            //locationService.StartTracking();

            while (count < 5 )
            {
                
            }

            Console.WriteLine("test1");
            Assert.True(true);
        }

        [Test]
        public void Fail()
        {
            Assert.False(true);
        }

        [Test]
        [Ignore("another time")]
        public void Ignore()
        {
            Assert.True(false);
        }

        [Test]
        public void Inconclusive()
        {
            Assert.Inconclusive("Inconclusive");
        }
    }
}