using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Airline
{
  public class Flight
  {
    public static DateTime DefaultDate = new DateTime(2015, 1, 18);
    private string _airline;
    private int _id;
    private int _statusId;
    private int _flightPathId;
    private DateTime _departureTime;

    public Flight(string airline, DateTime departureTime , int id = 0, int statusId = 0, int flightPath = 0)
    {
      _id = id;
      _airline = airline;
      _departureTime = departureTime;
      _statusId = statusId;
      _flightPathId = flightPath;
    }


    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        bool idEquality = this.GetId() == newFlight.GetId();
        return (idEquality);
      }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetAirline()
    {
      return _airline;
    }
    public void SetAirline(string newAirline)
    {
      _airline = newAirline;
    }
    public DateTime GetDepartureTime()
    {
      return _departureTime;
    }
    public void SetDepartureTime(DateTime newDepartureTime)
    {
      _departureTime = newDepartureTime;
    }
    public int GetStatusId()
    {
      return _statusId;
    }
    public void SetStatusId(int newStatusId)
    {
      _statusId = newStatusId;
    }
    public string GetStatusString(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT name FROM statuses WHERE id = @statusId;", conn);

      SqlParameter statusParameter = new SqlParameter();
      statusParameter.ParameterName = "@statusId";
      statusParameter.Value = id;

      cmd.Parameters.Add(statusParameter);

      rdr = cmd.ExecuteReader();

      string flightStatus = "";
      while(rdr.Read())
      {
        flightStatus = rdr.GetString(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return flightStatus;
    }

    public string GetFullFlightString()
    {

      FlightPath thisFlightPath = FlightPath.Find(_flightPathId);

      string flightPathString = thisFlightPath.GetFlightPathString();

      return GetAirline() +" "+ GetId().ToString()+":  " + GetStatusString(_statusId) +"- "+ flightPathString;
    }

    // public string[] GetAllFlightPathstrings()
    // {
    //   List<FlightPath> paths = FlightPath.GetAll();
    //
    //   for(int i = 0; i < paths.Count; i++)
    //   {
    //
    //     paths.Add();
    //   }
    //   return paths;
    // }

    public int GetFlightPath()
    {
      return _flightPathId;
    }
    public void SetFlightPath(int newFlightPath)
    {
      _flightPathId = newFlightPath;
    }

    public static List<Flight> GetAll()
    {
      List<Flight> allFlights = new List<Flight>{};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flights ORDER BY departure_time ASC;", conn);


      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        string airline = rdr.GetString(0);
        int flightId = rdr.GetInt32(2);
        int statusId = rdr.GetInt32(3);
        int flightPathId = rdr.GetInt32(4);
        DateTime departureTime = rdr.GetDateTime(1);
        Flight newFlight = new Flight(airline, departureTime, flightId, statusId, flightPathId);
        allFlights.Add(newFlight);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allFlights;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO flights (airline, departure_time, status_id, flight_path_id) OUTPUT INSERTED.id VALUES (@airline, @departureTime, @statusId, @flightPathId);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@airline";
      descriptionParameter.Value = this.GetAirline();

      cmd.Parameters.Add(descriptionParameter);

      SqlParameter departureTimeParameter = new SqlParameter();
      departureTimeParameter.ParameterName = "@departureTime";
      departureTimeParameter.Value = this.GetDepartureTime();

      cmd.Parameters.Add(departureTimeParameter);

      SqlParameter statusIdParameter = new SqlParameter();
      statusIdParameter.ParameterName = "@statusId";
      statusIdParameter.Value = this.GetStatusId();

      cmd.Parameters.Add(statusIdParameter);

      SqlParameter flightPathIdParameter = new SqlParameter();
      flightPathIdParameter.ParameterName = "@flightPathId";
      flightPathIdParameter.Value = this.GetFlightPath();

      cmd.Parameters.Add(flightPathIdParameter);


      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static Flight Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM flights WHERE id = @FlightId;", conn);
      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = id.ToString();
      cmd.Parameters.Add(flightIdParameter);
      rdr = cmd.ExecuteReader();

      int foundFlightId = 0;
      int foundFlightPath = 0;
      int foundFlightStatus = 0;
      string foundAirline = null;
      DateTime departureTime = DefaultDate;

      while(rdr.Read())
      {
        foundFlightStatus = rdr.GetInt32(3);
        foundAirline = rdr.GetString(0);
        departureTime = rdr.GetDateTime(1);
        foundFlightId = rdr.GetInt32(2);
        foundFlightPath = rdr.GetInt32(4);
      }
      Flight foundFlight = new Flight(foundAirline, departureTime, foundFlightId, foundFlightStatus, foundFlightPath);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return foundFlight;
    }

    // public void AddCategory(Category newCategory)
    // {
    //   SqlConnection conn = DB.Connection();
    //   conn.Open();
    //
    //   SqlCommand cmd = new SqlCommand("INSERT INTO categories_flights (category_id, flight_id) VALUES (@CategoryId, @FlightId);", conn);
    //
    //   SqlParameter categoryIdParameter = new SqlParameter();
    //   categoryIdParameter.ParameterName = "@CategoryId";
    //   categoryIdParameter.Value = newCategory.GetId();
    //   cmd.Parameters.Add(categoryIdParameter);
    //
    //   SqlParameter flightIdParameter = new SqlParameter();
    //   flightIdParameter.ParameterName = "@FlightId";
    //   flightIdParameter.Value = this.GetId();
    //   cmd.Parameters.Add(flightIdParameter);
    //
    //   cmd.ExecuteNonQuery();
    //
    //   if (conn != null)
    //   {
    //     conn.Close();
    //   }
    // }


    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM flights WHERE id = @FlightId; DELETE FROM departures WHERE flight_id = @FlightId;DELETE FROM arrivals WHERE flight_id = @FlightId;", conn);

      SqlParameter flightIdParameter = new SqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();

      cmd.Parameters.Add(flightIdParameter);
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
      SqlCommand cmd = new SqlCommand("DELETE FROM flights; DELETE FROM departures;DELETE FROM arrivals;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}
