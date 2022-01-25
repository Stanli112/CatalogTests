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

            Thread thread = new Thread(new ThreadStart(()=> {
                _mainView = new MainWindow();
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        [Test]
        public void CreateTables_Test()
        {
            Assert.IsTrue(_service.CreateTables(), "Failed to create tables in database");
        }

        [Test]
        public void WorkWithSectorTable_Test()
        {
            Sector sector = new Sector();
            Assert.IsTrue(_service.AddSectorToDB(new Sector()), "Data was not added to database");
            Assert.IsTrue(_service.GetSectorFromDB().Count > 0, "Invalid data value");
            Assert.IsTrue(_service.DeleteSectorFromDB(sector), "Failed to delete data from table");
        }

    }
}