using Microsoft.VisualStudio.TestTools.UnitTesting;
using AxisBIWebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AxisBIWebAPI.Tests
{
    [TestClass()]
    public class HoursTests
    {
        [TestMethod()]
        public void CreateTaskTest()
        {
            Hours h = new Hours();
            h.TeamProject = "TestScrum";
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            var actual = h.CreateTask("ROSS123456", new DateTime(2016, 1, 1), 1, "Alexandre Campos Silva <alexde@microsoft.com>", 3);

            Assert.AreEqual(actual.rev, 1);
            //Assert.AreEqual(actual.fields[);
        }

        [TestMethod()]
        public void UpdateRemainigWorkTest()
        {

            Hours h = new Hours();
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            string json = h.UpdateRemainigWork(81, 8);
            bool contains = json.IndexOf("\"Microsoft.VSTS.Scheduling.RemainingWork\":8") > 0;
            var wi = JsonConvert.DeserializeObject<WorkItem>(json);

            Assert.AreEqual("8", wi.fields["Microsoft.VSTS.Scheduling.RemainingWork"].ToString());


            Assert.IsTrue(contains);


        }

        [TestMethod()]
        public void DoneTaskTest()
        {
            Hours h = new Hours();
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            var wi = h.DoneTask(81);

            Assert.AreEqual("Done", wi.fields["System.State"].ToString());



        }

        [TestMethod()]
        public void HorasNaoLogadasTest_true()
        {
            Hours h = new Hours();
            h.TeamProject = "TestScrum";
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            var actual = h.HorasNaoLogadas("ross", new DateTime(2016, 1, 1));

            Assert.AreEqual(true, actual);
        }


        [TestMethod()]
        public void HorasNaoLogadasTest_false()
        {
            Hours h = new Hours();
            h.TeamProject = "TestScrum";
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            var actual = h.HorasNaoLogadas("ROSS123456", new DateTime(2016, 1, 1));

            Assert.AreEqual(false, actual);
        }

        [TestMethod()]
        public void FindTaksTest()
        {
            Hours h = new Hours();
            h.TeamProject = "TestScrum";
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            var actual = h.FindTaks("ROSS123456", "Alexandre Campos Silva <alexde@microsoft.com>");

            Assert.AreEqual(83, actual.id);
        }

        [TestMethod()]
        public void LogarHoras()
        {

            Hours h = new Hours();
            h.TeamProject = "TestScrum";
            h.url = "https://axisbi.VisualStudio.com/DefaultCollection/";
            h.LogarHoras("ROSS123456", new DateTime(2016,1,3), 8, "Alexandre Campos Silva <alexde@microsoft.com>");

            Assert.IsTrue(true);
        }
    }

}