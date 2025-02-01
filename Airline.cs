//==========================================================
// Student Number : S10266942
// Student Name :  Pugazhenthi Dharundev
// Partner Name : T Venkatesh
//==========================================================

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

    public Airline(string name, string code, Dictionary<String, Flight> flights)
    {
        Name = name;
        Code = code;
        Flights = flights;
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

    public bool RemoveFlight(Flight flight)
    {
        if (Flights.ContainsKey(flight.FlightNumber))
        {
            Flights.Remove(flight.FlightNumber);
            return true;
        }
        else
        {
            return false;
        }
    }

    public double CalculateFees()
    {
        double totalFees = 0;
        int arrivingFlights = 0;
        int departingFlights = 0;
        bool hasFlightBeforeElevenOrAfterNine = false;
        bool hasDxbBkkOrNrtFlight = false;

        foreach (Flight flight in flights.Values)
        {
            // Since we can't access private members directly, we'll need to use ToString()
            // Assuming ToString() returns a formatted string with all flight information
            string flightInfo = flight.ToString();

            // Base fees for arriving/departing
            if (flightInfo.Contains("Destination: SIN"))
            {
                totalFees += 500; // Arriving flight fee
                arrivingFlights++;
            }
            if (flightInfo.Contains("Origin: SIN"))
            {
                totalFees += 800; // Departing flight fee
                departingFlights++;
            }

            // Check origin for DXB, BKK, or NRT
            if (flightInfo.Contains("Origin: DXB") ||
                flightInfo.Contains("Origin: BKK") ||
                flightInfo.Contains("Origin: NRT"))
            {
                hasDxbBkkOrNrtFlight = true;
            }

            // Parse time from the flight info string
            // This assumes the time format is included in ToString()
            if (flightInfo.Contains("Time:"))
            {
                string timeStr = flightInfo.Split("Time:")[1].Trim().Split()[0];
                if (DateTime.TryParse(timeStr, out DateTime flightTime))
                {
                    if (flightTime.Hour < 11 || flightTime.Hour >= 21)
                    {
                        hasFlightBeforeElevenOrAfterNine = true;
                    }
                }
            }
        }

        // Apply discounts
        int totalFlights = arrivingFlights + departingFlights;

        // Discount for every 5 flights
        int fiveFlightSets = totalFlights / 5;
        double discount = fiveFlightSets * 350;

        // Time-based discount
        if (hasFlightBeforeElevenOrAfterNine)
        {
            discount += 110;
        }

        // Origin-based discount
        if (hasDxbBkkOrNrtFlight)
        {
            discount += 25;
        }

        // Additional discount for airlines with more than 5 flights
        if (totalFlights > 5)
        {
            discount += (totalFees - discount) * 0.03; // 3% additional discount
        }

        return totalFees - discount;
    }




public override string ToString()
    {
        return "Airline: " + Name + " (" + Code + ")";
    }

}
