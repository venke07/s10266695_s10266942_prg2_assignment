using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s10266695_s10266942_prg2_assignment;

internal class Airline
{
    private string name;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    private string code;

    public string Code
    {
        get { return code; }
        set { code = value; }
    }

    private Dictionary<String, Flight> flights;

    public Dictionary<String, Flight> Flights
    {
        get { return flights; }
        set { flights = value; }
    }

    public Airline() { }

    public Airline(string name, string code, Dictionary<String, Flight> flights)
    {
        Name = name;
        Code = code;
        Flights = flights;
    }

    public void CalculateFees() { }

    public override string ToString()
    {
        return "Airline: " + Name + " (" + Code + ")";
    }

    public bool AddFlight(Flight flight)
    {
    
        if (Flights.ContainsKey(flight.FlightNumber))
        {
            return false;
        }
        else
        {
            Flights.Add(flight.FlightNumber, flight);
            return true;
        }
    }

    public void RemoveFlight(Flight flight)
    {
        if (Flights.ContainsKey(flight.FlightNumber))
        {
            Flights.Remove(flight.FlightNumber, flight);
            return true;
        }
        else
        {
            return false;
        }
    }

}
