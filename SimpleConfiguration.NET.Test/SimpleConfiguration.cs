using NUnit.Framework;
using System.Diagnostics;

namespace SimpleConfiguration.NET.Test
{
    public class Tests
    {
        public class Demo
        {
            public string String { get; set; }
            public bool Bool { get; set; }
            public int Int { get; set; }
        }
        public Configuration<Demo> Configuration { get; set; }

        [SetUp]
        public void SetUp()
        {
            Configuration = new Configuration<Demo>(new ConfigurationOptions()
            {
                ProgramName = "demoprogram",
                SettingsExtension = "test",
                SettingsName = "demo",
            });
        }

        [Test]
        public void ModifyData()
        {
            Configuration.Data.Bool = true;
            Assert.AreEqual(true, Configuration.Data.Bool);
        }

        [Test]
        public void ModifyDataEvent()
        {
            Configuration.OnDataChanged += delegate (Demo old, Demo latest) {
                Debug.WriteLine(old.Bool);
                Debug.WriteLine(latest.Bool);
                Assert.AreEqual(true, latest.Bool);
                Assert.AreEqual(false, old.Bool);
            };
            Configuration.Set(a => a.Bool = true);
        }

        [Test]
        public void SaveSomething()
        {
            Configuration.Set(a => a.Int = 55);
            Configuration.Save();
            Assert.IsTrue(Configuration.SaveExists());
        }

        [Test]
        public void LoadSomething()
        {
            Configuration.Set(a => a.Int = 55);
            Configuration.Save();
            Configuration.Set(a => a.Int = 22);
            Configuration.Load();
            Assert.IsTrue(Configuration.Data.Int == 55);
        }

        [Test]
        public void DeleteAll()
        {
            Configuration.Set(a => a.Int = 55);
            Configuration.Save();
            Configuration.Delete();
            Assert.IsFalse(Configuration.SaveExists());
        }
    }
}