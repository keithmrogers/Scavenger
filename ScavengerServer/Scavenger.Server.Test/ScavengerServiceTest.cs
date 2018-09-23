using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scavenger.Server.Test.TestClient;

namespace Scavenger.Server.Test
{
    [TestClass]
    public class ScavengerServiceTest
    {
        private string IpAddress { get { return ConfigurationManager.AppSettings["IpAddress"]; } }
        

        [TestMethod]
        public void StartLobby()
        {
            IGuideService guideService = new GuideService(IpAddress);
            IScavengerService scavengerService = new ScavengerService(IpAddress);

            var guideClient = new TestGuideClient();
            var scavengerClient = new TestScavengerClient();

            var guideReady = false;
            var scavengerReady = false;


            guideClient.OnLobbyReady += guid => guideReady = true;
            scavengerClient.OnLobbyReady += guid => scavengerReady = true;

            guideService.Start(guideClient);
            scavengerService.Start(scavengerClient);

            while (!guideReady && !scavengerReady)
            {
            }

            Assert.IsTrue(guideReady && scavengerReady);
        }
    }
}
