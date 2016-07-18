using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Airline
{
  public class FlightPath
  {
    private int _id;
    private int _arrivalCityId;
    private int _departureCityId;
    private float _duration;

    public FlightPath(int arrivalCity, int departureCity, float duration, int id = 0)
    {
      _arrivalCityId = arrivalCity;
      _departureCityId = departureCity;
      _duration = duration;
      _id = id;
    }

    public override bool Equals(System.Object otherFlightPath)
    {
      if (!(otherFlightPath is FlightPath))
      {
        return false;
      }
      else
      {
        FlightPath newFlightPath = (FlightPath) otherFlightPath;
        bool idEquality = this.GetId() == newFlightPath.GetId();
        return (idEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }
    public void SetId(int newId)
    {
      _id = newId;
    }

    public int GetArrivalCity()
    {
      return _arrivalCityId;
    }
    public void SetArrivalCity(int newArrivalCity)
    {
      _arrivalCityId = newArrivalCity;
    }

    public int GetDepartureCity()
    {
      return _departureCityId;
    }
    public void SetDepartureCity(int newDepartureCity)
    {
      _departureCityId = newDepartureCity;
    }

    public float GetDuration()
    {
      return _duration;
    }
    public void SetDuration(float newDuration)
    {
      _duration = newDuration;
    }

    public static List<FlightPath> GetAll()
    {
      List<FlightPath> allFlightPaths = new List<FlightPath>{};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flight_paths;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int arrivalCityId = rdr.GetInt32(0);
        int departureCityId = rdr.GetInt32(1);
        int duration = rdr.GetInt32(2);
        int flightPathId = rdr.GetInt32(3);
        FlightPath newFlightPath = new FlightPath(arrivalCityId, departureCityId, duration, flightPathId);
        allFlightPaths.Add(newFlightPath);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allFlightPaths;
    }

    public static FlightPath Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flight_paths WHERE id = @FlightPathId;", conn);
      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightPathId";
      flightIdParameter.Value = id.ToString();
      cmd.Parameters.Add(flightIdParameter);
      rdr = cmd.ExecuteReader();

      int foundFlightPathId = 0;
      int foundFlightPathDepartureCity = 0;
      int foundFlightPathArrivalCity = 0;
      float foundDuration = 0;


      while(rdr.Read())
      {
        foundFlightPathId = rdr.GetInt32(3);
        foundFlightPathDepartureCity = rdr.GetInt32(0);
        foundFlightPathArrivalCity = rdr.GetInt32(1);
        foundDuration = rdr.GetFloat(2);

      }
      FlightPath foundFlightPath = new FlightPath(foundFlightPathArrivalCity, foundFlightPathDepartureCity, foundDuration, foundFlightPathId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundFlightPath;
    }

    public static string GetCityString(int cityId)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT cities.name From cities WHERE cities.id = @cityId;", conn);

      SqlParameter cityParameter = new SqlParameter();
      cityParameter.ParameterName = "@cityId";
      cityParameter.Value = cityId;

      cmd.Parameters.Add(cityParameter);

      rdr = cmd.ExecuteReader();

      string cityName = "";
      while(rdr.Read())
      {
        cityName = rdr.GetString(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return cityName;
    }

    public string GetFlightPathString()
    {
      return FlightPath.GetCityString(_arrivalCityId) + " - " + FlightPath.GetCityString(_departureCityId);
    }

  }
}
