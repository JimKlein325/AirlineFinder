using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Airline
{
  public class FlightTest : IDisposable
  {
    public FlightTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=airline_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Flight.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Flight firstFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);
      Flight secondFlight = new Flight("Mow the lawn",  firstFlight.GetDepartureTime(), firstFlight.GetId(), 1, 1);

      //Assert
      Assert.Equal(firstFlight, secondFlight);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);
      testFlight.Save();

      //Act
      List<Flight> result = Flight.GetAll();
      List<Flight> testList = new List<Flight>{testFlight};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);
      testFlight.Save();

      //Act
      Flight savedFlight = Flight.GetAll()[0];

      int result = savedFlight.GetId();
      int testId = testFlight.GetId();

      //Assert
      Assert.Equal(testId, result);
    }

    [Fact]
    public void Test_FindFindsFlightInDatabase()
    {
      //Arrange
      Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);
      testFlight.Save();

      //Act
      Flight result = Flight.Find(testFlight.GetId());

      //Assert
      Assert.Equal(testFlight, result);
    }

    // [Fact]
    // public void Test_AddCity_AddsCityToFlight()
    // {
    //   //Arrange
    //   Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);
    //   testFlight.Save();
    //
    //   City testCity = new City("Home stuff");
    //   testCity.Save();
    //
    //   //Act
    //   testFlight.AddCity(testCity);
    //
    //   List<City> result = testFlight.GetCities();
    //   List<City> testList = new List<City>{testCity};
    //
    //   //Assert
    //   Assert.Equal(testList, result);
    // }
    //
    // [Fact]
    // public void Test_GetCities_ReturnsAllFlightCities()
    // {
    //   //Arrange
    //   Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);
    //   testFlight.Save();
    //
    //   City testCity1 = new City("Home stuff");
    //   testCity1.Save();
    //
    //   City testCity2 = new City("Work stuff");
    //   testCity2.Save();
    //
    //   //Act
    //   testFlight.AddCity(testCity1);
    //   List<City> result = testFlight.GetCities();
    //   List<City> testList = new List<City> {testCity1};
    //
    //   //Assert
    //   Assert.Equal(testList, result);
    // }
    //
    // [Fact]
    // public void Test_Delete_DeletesFlightAssociationsFromDatabase()
    // {
    //   //Arrange
    //   City testCity = new City("Home stuff");
    //   testCity.Save();
    //
    //   string testDescription = "Mow the lawn";
    //   Flight testFlight = new Flight(testDescription, Flight.DefaultDate, 0, 1, 1);
    //   testFlight.Save();
    //
    //   //Act
    //   testFlight.AddCity(testCity);
    //   testFlight.Delete();
    //
    //   List<Flight> resultCityFlights = testCity.GetFlights();
    //   List<Flight> testCityFlights = new List<Flight> {};
    //
    //   //Assert
    //   Assert.Equal(testCityFlights, resultCityFlights);
    // }

    [Fact]
    public void Test_Equals_DateTimeComparing()
    {
      //Arrange
      DateTime otherDate = new DateTime(2000, 1, 1);
      Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);

      // Act
      DateTime testFlightDepartureTime = testFlight.GetDepartureTime();

      //Assert
      Assert.NotEqual(otherDate, testFlightDepartureTime);
    }

    [Fact]
    public void Test_SetDepartureTime_DateTimeComparing()
    {
      //Arrange
      DateTime otherDate = new DateTime(2000, 1, 1);
      Flight testFlight = new Flight("Mow the lawn", Flight.DefaultDate, 0, 1, 1);

      // Act
      testFlight.SetDepartureTime(otherDate);
      DateTime testFlightDepartureTime = testFlight.GetDepartureTime();

      //Assert
      Assert.Equal(otherDate, testFlightDepartureTime);
    }


    public void Dispose()
    {
      Flight.DeleteAll();
      // City.DeleteAll();
    }

  }
}
