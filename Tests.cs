using NUnit.Framework;
using catalog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using catalog.Models;
using System.Data;

namespace CatalogTests
{
    public class Tests
    {
        private MainWindow _mainView;
        private DBService _service;

        [SetUp]
        public void Setup()
        {
            _service = new DBService("test_db");
            Assert.AreEqual(_service.GetServerState(), ConnectionState.Open);
        }

        [TearDown]
        public void Tearndown()
        {
            _service.Close();
        }


        [Test]
        public void CreateWindow_Test()
        {
            Thread thread = new Thread(new ThreadStart(() => {
                _mainView = new MainWindow();

                Assert.IsTrue(_mainView != null, "Failed to MainWindow create");
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [Test]
        public void CreateTables_Test()
        {
            Assert.IsTrue(_service.CreateTables(), "Failed to create tables in database");
        }

        [Test]
        public void Throw_Test()
        {
            _service.Close();
            Assert.Throws<InvalidOperationException>(() => _service.GetCategoryFromDB());
        }

        [Test]
        public void WorkWithSectorTable_Test()
        {
            Sector sector;
            Assert.IsTrue(_service.AddSectorToDB(new Sector()), "Data was not added to database");
            Assert.IsTrue(_service.GetSectorFromDB().Count > 0, "Invalid data value");
            sector = _service.GetSectorFromDB()[0];
            Assert.IsTrue(_service.DeleteSectorFromDB(sector), "Invalid request");
            Assert.IsTrue(_service.GetSectorFromDB().Count == 0, "Failed to delete data from table");
        }

        [Test]
        public void WorkWithCategoryTable_Test()
        {
            Category category;
            Assert.IsTrue(_service.AddCategoryToDB(new Category()), "Data was not added to database");
            Assert.IsTrue( _service.GetCategoryFromDB().Count > 0, "Invalid data value");
            category = _service.GetCategoryFromDB()[0];
            Assert.IsTrue(_service.DeleteCategoryFromDB(category), "Invalid request");
            Assert.IsTrue(_service.GetCategoryFromDB().Count == 0, "Failed to delete data from table");
        }

        [Test]
        public void WorkWithDeviceTable_Test()
        {
            Device device, second_device;
            Assert.IsTrue(_service.AddDeviceToDB(new Device()), "Data was not added to database");
            Assert.IsTrue(_service.GetDevicesFromDB().Count > 0, "Invalid data value");
            device = _service.GetDevicesFromDB()[0];

            device.Name = "test_name";
            device.Model = "test_model";
            device.Description = "test_description";
            Assert.IsTrue(_service.UpdateDeviceInDB(device), "Invalid request");

            second_device = _service.GetDevicesFromDB()[0];
            Assert.IsTrue(device.Name == second_device.Name &&
                device.Model == second_device.Model &&
                device.Description == second_device.Description,
                "Invalid update request");

            Assert.IsTrue(_service.DeleteDeviceFromDB(device), "Failed to delete data from table");
            Assert.IsTrue(_service.GetDevicesFromDB().Count == 0, "Failed to delete data from table");
        }

    }
}