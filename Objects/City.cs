using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Airline
{
  public class City
  {
    private int _id;
    private string _name;
    public enum FlightType
    {
      Arrival,
      Departure
    };
    public City(string Name, int Id = 0)
    {
      _id = Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherCity)
    {
      if (!(otherCity is City))
      {
        return false;
      }
      else
      {
        City newCity = (City) otherCity;
        bool idEquality = this.GetId() == newCity.GetId();
        bool nameEquality = this.GetName() == newCity.GetName();
        return (idEquality && nameEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public static List<City> GetAll()
    {
      List<City> allCities = new List<City>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM cities;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int cityId = rdr.GetInt32(1);
        string cityName = rdr.GetString(0);
        City newCity = new City(cityName, cityId);
        allCities.Add(newCity);
      }


      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allCities;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO cities (name) OUTPUT INSERTED.id VALUES (@CityName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@CityName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      rdr = cmd.ExecuteReader();
      //this._id = 38;


      while(rdr.Read())
      {
        // Console.WriteLine(rdr.GetString(1));
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }


    public List<Flight> GetFlights(FlightType flightType)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand();
      if(flightType == FlightType.Arrival)
      {
        cmd = new SqlCommand("SELECT flights.* FROM cities JOIN arrivals ON (cities.id = arrivals.city.id) JOIN flights ON (arrivals.flight_id = flights.id) WHERE cities.id = @CityId", conn);
      }
      else
      {
        cmd = new SqlCommand("SELECT flights.* FROM cities JOIN departures ON (cities.id = departures.city.id) JOIN flights ON (departures.flight_id = flights.id) WHERE cities.id = @CityId", conn);
      }

      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "@CityId";
      cityIdParameter.Value = this.GetId();
      cmd.Parameters.Add(cityIdParameter);
      rdr = cmd.ExecuteReader();

      List<Flight> flights = new List<Flight> {};

        while(rdr.Read())
        {
          string airline = rdr.GetString(0);
          DateTime departureDateTime = rdr.GetDateTime(1);
          int thisFlightId = rdr.GetInt32(2);
          int statusId = rdr.GetInt32(3);
          int flightPath = rdr.GetInt32(4);
          Flight foundFlight = new Flight(airline, departureDateTime, thisFlightId, statusId, flightPath);
          flights.Add(foundFlight);
        }
        if (rdr != null)
        {
          rdr.Close();
        }


      if (conn != null)
      {
        conn.Close();
      }
      return flights;
    }

    public static City Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM cities WHERE id = @CityId;", conn);
      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "@CityId";
      cityIdParameter.Value = id.ToString();
      cmd.Parameters.Add(cityIdParameter);
      rdr = cmd.ExecuteReader();

      int foundCityId = 0;
      string foundCityDescription = null;

      while(rdr.Read())
      {
        foundCityId = rdr.GetInt32(1);
        foundCityDescription = rdr.GetString(0);
      }
      City foundCity = new City(foundCityDescription, foundCityId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCity;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM cities WHERE id = @CityId;", conn);
      SqlParameter cityIdParameter = new SqlParameter();
      cityIdParameter.ParameterName = "@CityId";
      cityIdParameter.Value = this.GetId();

      cmd.Parameters.Add(cityIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM cities;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}
